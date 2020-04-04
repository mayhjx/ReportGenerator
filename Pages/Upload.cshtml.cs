using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ReportGenerator.Models;
using ReportGenerator.Data;
using System.IO;
using RDotNet;

namespace ReportGenerator.Pages
{
    public class UploadModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly ProjectParametersContext _projectParametersContext;

        public UploadModel(ILogger<IndexModel> logger,
                        IWebHostEnvironment webHostEnvironment,
                        ProjectParametersContext projectParametersContext)
        {
            _logger = logger;
            _WebHostEnvironment = webHostEnvironment;
            _projectParametersContext = projectParametersContext;
        }

        [BindProperty]
        public UploadForm UploadForm { get; set; }

        [BindProperty]
        public Report ReportData { get; set; }

        // 提示信息
        public string Message { get; private set; }

        [BindProperty]
        public IEnumerable<string> options { get; private set; }

        public void OnGet()
        {
            // 从项目参数数据库中获取检测项目名称
            options = from project in _projectParametersContext.ProjectParameter.ToList()
                      select project.Name;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 仪器编号从原始数据文件名称中提取
            string targetFileName = Path.GetFileNameWithoutExtension(UploadForm.TargetFile.FileName);
            string matchFileName = Path.GetFileNameWithoutExtension(UploadForm.MatchFile.FileName);
            string template = UploadForm.Template;

            // 读取数据 包括不同编码格式 岛津是ansi格式, decode?
            // 不同平台要使用不同的模块读取，增加一个输入框用于输入平台

            // 暂时使用csv格式的数据进行测试

            // 读取靶仪器数据
            using var targetFileStream = UploadForm.TargetFile.OpenReadStream();
            string targetDataStream;
            using (StreamReader sr = new StreamReader(targetFileStream))
            {
                targetDataStream = sr.ReadToEnd();
            }

            // 读取比对仪器数据
            using var matchFileStream = UploadForm.MatchFile.OpenReadStream();
            string matchDataStream;
            using (StreamReader sr = new StreamReader(matchFileStream))
            {
                matchDataStream = sr.ReadToEnd();
            }

            // 查找BD编号的样本结果
            var targetResult = new Dictionary<string, double>();
            var matchResult = new Dictionary<string, double>();

            foreach (var line in targetDataStream.Split("\r\n"))
            {
                var data = line.Split(",");
                if (data[0].StartsWith("BD"))
                {
                    if (targetResult.ContainsKey(data[0]))
                    {
                        Message = "重复的实验号: " + data[0];
                        return Page();
                    }
                    try
                    {
                        targetResult.Add(data[0], double.Parse(data[1]));
                    }
                    catch
                    {
                        Message = $"无法识别的输入: {data[0]} {data[1]}";
                        return Page();
                    }
                }
            }

            foreach (var line in matchDataStream.Split("\r\n"))
            {
                var data = line.Split(",");
                if (data[0].StartsWith("BD"))
                {
                    if (matchResult.ContainsKey(data[0]))
                    {
                        Message = "重复的实验号: " + data[0];
                        return Page();
                    }
                    try
                    {
                        matchResult.Add(data[0], double.Parse(data[1]));
                    }
                    catch
                    {
                        Message = $"无法识别的输入: {data[0]} {data[1]}";
                        return Page();
                    }
                }
            }

            if (targetResult.Keys.Count < 20 || matchResult.Keys.Count < 20)
            {
                Message = $"样品数量小于20个！";
                return Page();
            }

            if (targetResult.Keys.Count != matchResult.Keys.Count)
            {
                Message = "样品数量不一致，请确认！";
                return Page();
            }

            ReportData.Item = UploadForm.Item;
            ReportData.TargetInstrumentName = targetFileName;
            ReportData.MatchInstrumentName = matchFileName;

            // 根据项目从appsettings.json获取Xc和ALE
            // 要先将appsettings另存为UTF-8格式，否则中文会乱码
            // 添加个性化命令：Tools -> Customize -> Commmands -> Menu(file) -> Add Commands
            var project = _projectParametersContext.ProjectParameter.FirstOrDefault(m => m.Name == UploadForm.Item);
            ReportData.ALE = project.ALE;
            ReportData.Xc1 = project.Xc1;
            ReportData.Xc2 = project.Xc2;

            int significantDigit = project.SignificantDigits;

            // 结果保留n位有效数字后以逗号分隔的字符串保存到数据库
            ReportData.SampleName = string.Join(",", targetResult.Select(kv => kv.Key).ToArray());
            ReportData.TargetResult = string.Join(",", targetResult.Select(kv => SignificantDigits.Reserved(kv.Value, significantDigit)).ToArray());
            ReportData.MatchResult = string.Join(",", matchResult.Select(kv => SignificantDigits.Reserved(kv.Value, significantDigit)).ToArray());

            // 离群值列表
            IntegerVector OutliersList;
            try
            {
                CallRSource(out OutliersList);
            }
            catch (Exception e)
            {
                Message = "无法调用R语言模块，数据未提交成功！" + e.Message;
                return Page();
            }

            // 各参数条件判断 不通过则停止程序并提示
            if (OutliersList != null && OutliersList.Count() > 0)
            {
                Message = "离群值: ";
                Message += string.Join(',', from i in OutliersList
                                           let samplename = targetResult.ElementAt(i - 1).Key
                                           select samplename);
                return Page();
            }
            // P值小于0.1
            if (ReportData.P < 0.1)
            {
                Message = $"两组数据线性相关性差: P<0.10";
                return Page();
            }
            // 最大SE/Xc < 1/2ALE
            // TODO

            return RedirectToPage("QuantitativeReports/Create", "Generate", ReportData);
        }

        /// <summary>
        /// R模块-Passing-Bablok regession
        /// </summary>
        /// <param name="OutliersList"></param>
        public void CallRSource(out IntegerVector OutliersList)
        {
            // 要将 "C:\R-3.5.3\bin\x64 添加到系统环境路径中
            // The problem is actually not that stats.dll could not be found, 
            // but that a.dll that stats.dll depends on could not be loaded - 
            // I've found this out by inspecting the stats.dll file using a tool called Dependency Walker. 
            // In fact, while loading the stats package, the files "R.dll", "Rblas.dll" and
            // "Rlapack.dll" could not be found - they all lie in the "bin" directory of the R installation, e.g. "R-3.5.3\bin\x64".
            // https://github.com/NetLogo/R-Extension/issues/5 @gunnardressler

            REngine engine;
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();

            // 线形图保存路径
            var PictureDir = Path.Combine(_WebHostEnvironment.WebRootPath, "Pictures");
            if (!Directory.Exists(PictureDir))
            {
                Directory.CreateDirectory(PictureDir);
            }
            string imagePath = Path.Combine(PictureDir, ReportData.Item + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + System.Guid.NewGuid().ToString() + ".png").Replace("\\", "/");

            ReportData.PicturePath = Path.Combine(@"\Pictures", Path.GetFileName(imagePath));

            engine.SetSymbol("target", engine.CreateCharacter(ReportData.TargetInstrumentName));
            engine.SetSymbol("match", engine.CreateCharacter(ReportData.MatchInstrumentName));
            engine.SetSymbol("filename", engine.CreateCharacter(imagePath));
            engine.SetSymbol("检测系统A结果", engine.CreateNumericVector(ReportData.TargetResult.Split(",").Select(x => double.Parse(x)).ToList()));
            engine.SetSymbol("检测系统B结果", engine.CreateNumericVector(ReportData.MatchResult.Split(",").Select(x => double.Parse(x)).ToList()));

            // 调用 R
            string RSourcePath = Path.Combine(_WebHostEnvironment.WebRootPath, "仪器比对报告后端.R").Replace("\\", "/");
            engine.Evaluate("source('" + RSourcePath + "')");

            // 离群值数据下标列表
            OutliersList = engine.GetSymbol("ID").AsInteger();

            ReportData.P = engine.GetSymbol("p").AsNumeric()[0];
            ReportData.b = double.Parse(engine.GetSymbol("b").AsNumeric()[0].ToString("F4"));
            ReportData.bUCI = double.Parse(engine.GetSymbol("b.upper").AsNumeric()[0].ToString("F4"));
            ReportData.bLCI = double.Parse(engine.GetSymbol("b.lower").AsNumeric()[0].ToString("F4"));
            ReportData.a = double.Parse(engine.GetSymbol("a").AsNumeric()[0].ToString("F4"));
            ReportData.aUCI = double.Parse(engine.GetSymbol("a.upper").AsNumeric()[0].ToString("F4"));
            ReportData.aLCI = double.Parse(engine.GetSymbol("a.lower").AsNumeric()[0].ToString("F4"));
        }
    }


    /// <summary>
    /// 将num保留n位有效数字
    /// 规则：四舍六入五考虑，五后非空就进一，五后为空看奇偶，五前为偶应舍去，五前为奇要进一
    /// </summary>
    public class SignificantDigits
    {
        public static string Reserved(double num, int n)
        {
            if (num == 0) return "0";

            if (n <= 0 || n > 28)
                throw new ArgumentOutOfRangeException("有效数字位数不能小于等于0或大于28");

            decimal result, number;
            try
            {
                number = decimal.Parse(num.ToString());
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("非法输入");
            }

            int time = 0;   // number除以10或乘以10的次数
            int pointIndex = number.ToString().IndexOf('.');  // 小数点位置(正数：-1, 含小数：>=1, 负数：2）
            int negative = number.ToString().StartsWith('-') ? 1 : 0;

            if (Math.Abs(number) > 1)
            {
                if (n >= pointIndex && pointIndex > 0)
                {
                    result = Math.Round(number, n - pointIndex + negative, MidpointRounding.ToEven);
                    return result.ToString($"F{n - pointIndex + negative}");
                }
                else
                {
                    while (Math.Abs(number) > 1)
                    {
                        number *= (decimal)0.1;
                        time++;
                    }
                    result = Math.Round(number, n, MidpointRounding.ToEven) * (decimal)Math.Pow(10, time);
                    return result.ToString($"G{n}");
                }
            }
            else if (Math.Abs(number) == 1)
            {
                // num传入为1.000时，自动转换为1
                return number.ToString($"F{n + pointIndex}");
            }
            else if (Math.Abs(number) >= (decimal)0.1)
            {
                result = Math.Round(number, n, MidpointRounding.ToEven);
                return result.ToString($"F{n}");
            }
            else
            {
                while (Math.Abs(number) < (decimal)0.1)
                {
                    number *= 10;
                    time--;
                }
                result = Math.Round(number, n, MidpointRounding.ToEven) * (decimal)Math.Pow(10, time);
                return result.ToString($"F{n - time}");
            }
        }
    }
}
