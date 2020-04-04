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
        public Report Report { get; set; }

        // 提示信息
        public string Message { get; private set; } = "";

        [BindProperty]
        public IEnumerable<string> Options { get; private set; }

        public void OnGet()
        {
            // 从项目参数数据库中获取检测项目名称
            Options = from project in _projectParametersContext.ProjectParameter.ToList()
                      select project.Name;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string template = UploadForm.Template;

            Report.Item = UploadForm.Item;
            Report.TargetInstrumentName = Path.GetFileNameWithoutExtension(UploadForm.TargetFile.FileName);
            Report.MatchInstrumentName = Path.GetFileNameWithoutExtension(UploadForm.MatchFile.FileName);

            var project = _projectParametersContext.ProjectParameter.FirstOrDefault(m => m.Name == Report.Item);
            Report.ALE = project.ALE;
            Report.Xc1 = project.Xc1;
            Report.Xc2 = project.Xc2;

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
                        Message = $"实验号重复: {data[0]}";
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
                        Message = $"实验号重复: {data[0]}";
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

            foreach (var key in targetResult.Keys)
            {
                if (!matchResult.ContainsKey(key))
                {
                    Message = $"实验号不一致: {key}";
                    return Page();
                }
                if ((targetResult.GetValueOrDefault(key) / matchResult.GetValueOrDefault(key) - 1) * 2 > Report.ALE)
                {
                    Message = $"结果一致性未通过：{key}";
                    return Page();
                }
            }

            int significantDigit = project.SignificantDigits;

            // 结果保留n位有效数字后以逗号分隔的字符串保存到数据库
            Report.SampleName = string.Join(",", targetResult.Select(kv => kv.Key).ToArray());
            Report.TargetResult = string.Join(",", targetResult.Select(kv => SignificantDigits.Reserved(kv.Value, significantDigit)).ToArray());
            Report.MatchResult = string.Join(",", matchResult.Select(kv => SignificantDigits.Reserved(kv.Value, significantDigit)).ToArray());
            Report.Bias = string.Join(",", from key in targetResult.Keys
                                           let bias = matchResult.GetValueOrDefault(key) / targetResult.GetValueOrDefault(key) - 1
                                           select SignificantDigits.Reserved(bias, significantDigit));

            // 各参数条件判断 不通过则停止程序并提示

            // 获取离群值
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

            // 返回离群值
            if (OutliersList != null && OutliersList.Count() > 0)
            {
                Message = "离群值: ";
                Message += string.Join(',', from i in OutliersList
                                            let samplename = targetResult.ElementAt(i - 1).Key
                                            select samplename);
                return Page();
            }

            // 当P值小于0.1
            if (Report.P <= 0.1)
            {
                Message = $"两组数据线性相关性差: P<=0.10";
                return Page();
            }

            // 最大SE/Xc > 1/2ALE
            if (MaxSEDivXc(Report.Xc1) * 2 > Report.ALE)
            {
                Message = $"医学决定水平一处最大SE/Xc={MaxSEDivXc(Report.Xc1)} >1/2ALE";
                return Page();
            }
            if (MaxSEDivXc(Report.Xc2) * 2 > Report.ALE)
            {
                Message = $"医学决定水平二处最大SE/Xc={MaxSEDivXc(Report.Xc2)} >1/2ALE";
                return Page();
            }

            return RedirectToPage("QuantitativeReports/Create", "Generate", Report);
        }

        // 返回最大SE/Xc
        double MaxSEDivXc(double Xc)
        {
            return Math.Max(Math.Abs((Report.bLCI * Xc + Report.aLCI) / Xc - 1),
                            Math.Abs((Report.bUCI * Xc + Report.aUCI) / Xc - 1));
        }

        /// <summary>
        /// R模块-Passing-Bablok regession
        /// </summary>
        /// <param name="OutliersList">返回离群值的下标, 从1开始<param>
        void CallRSource(out IntegerVector OutliersList)
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
            string imagePath = Path.Combine(PictureDir, Report.Item + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + System.Guid.NewGuid().ToString() + ".png").Replace("\\", "/");

            Report.PicturePath = Path.Combine(@"\Pictures", Path.GetFileName(imagePath));

            engine.SetSymbol("target", engine.CreateCharacter(Report.TargetInstrumentName));
            engine.SetSymbol("match", engine.CreateCharacter(Report.MatchInstrumentName));
            engine.SetSymbol("filename", engine.CreateCharacter(imagePath));
            engine.SetSymbol("检测系统A结果", engine.CreateNumericVector(Report.TargetResult.Split(",").Select(x => double.Parse(x)).ToList()));
            engine.SetSymbol("检测系统B结果", engine.CreateNumericVector(Report.MatchResult.Split(",").Select(x => double.Parse(x)).ToList()));

            // 调用 R
            string RSourcePath = Path.Combine(_WebHostEnvironment.WebRootPath, "仪器比对报告后端.R").Replace("\\", "/");
            engine.Evaluate("source('" + RSourcePath + "')");

            // 离群值数据下标列表
            OutliersList = engine.GetSymbol("ID").AsInteger();

            Report.P = engine.GetSymbol("p").AsNumeric()[0];
            Report.b = double.Parse(engine.GetSymbol("b").AsNumeric()[0].ToString("F4"));
            Report.bUCI = double.Parse(engine.GetSymbol("b.upper").AsNumeric()[0].ToString("F4"));
            Report.bLCI = double.Parse(engine.GetSymbol("b.lower").AsNumeric()[0].ToString("F4"));
            Report.a = double.Parse(engine.GetSymbol("a").AsNumeric()[0].ToString("F4"));
            Report.aUCI = double.Parse(engine.GetSymbol("a.upper").AsNumeric()[0].ToString("F4"));
            Report.aLCI = double.Parse(engine.GetSymbol("a.lower").AsNumeric()[0].ToString("F4"));
        }
    }
}