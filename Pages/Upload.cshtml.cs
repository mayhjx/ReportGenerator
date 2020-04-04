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

        // ��ʾ��Ϣ
        public string Message { get; private set; }

        [BindProperty]
        public IEnumerable<string> options { get; private set; }

        public void OnGet()
        {
            // ����Ŀ�������ݿ��л�ȡ�����Ŀ����
            options = from project in _projectParametersContext.ProjectParameter.ToList()
                      select project.Name;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ������Ŵ�ԭʼ�����ļ���������ȡ
            string targetFileName = Path.GetFileNameWithoutExtension(UploadForm.TargetFile.FileName);
            string matchFileName = Path.GetFileNameWithoutExtension(UploadForm.MatchFile.FileName);
            string template = UploadForm.Template;

            // ��ȡ���� ������ͬ�����ʽ ������ansi��ʽ, decode?
            // ��ͬƽ̨Ҫʹ�ò�ͬ��ģ���ȡ������һ���������������ƽ̨

            // ��ʱʹ��csv��ʽ�����ݽ��в���

            // ��ȡ����������
            using var targetFileStream = UploadForm.TargetFile.OpenReadStream();
            string targetDataStream;
            using (StreamReader sr = new StreamReader(targetFileStream))
            {
                targetDataStream = sr.ReadToEnd();
            }

            // ��ȡ�ȶ���������
            using var matchFileStream = UploadForm.MatchFile.OpenReadStream();
            string matchDataStream;
            using (StreamReader sr = new StreamReader(matchFileStream))
            {
                matchDataStream = sr.ReadToEnd();
            }

            // ����BD��ŵ��������
            var targetResult = new Dictionary<string, double>();
            var matchResult = new Dictionary<string, double>();

            foreach (var line in targetDataStream.Split("\r\n"))
            {
                var data = line.Split(",");
                if (data[0].StartsWith("BD"))
                {
                    if (targetResult.ContainsKey(data[0]))
                    {
                        Message = "�ظ���ʵ���: " + data[0];
                        return Page();
                    }
                    try
                    {
                        targetResult.Add(data[0], double.Parse(data[1]));
                    }
                    catch
                    {
                        Message = $"�޷�ʶ�������: {data[0]} {data[1]}";
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
                        Message = "�ظ���ʵ���: " + data[0];
                        return Page();
                    }
                    try
                    {
                        matchResult.Add(data[0], double.Parse(data[1]));
                    }
                    catch
                    {
                        Message = $"�޷�ʶ�������: {data[0]} {data[1]}";
                        return Page();
                    }
                }
            }

            if (targetResult.Keys.Count < 20 || matchResult.Keys.Count < 20)
            {
                Message = $"��Ʒ����С��20����";
                return Page();
            }

            if (targetResult.Keys.Count != matchResult.Keys.Count)
            {
                Message = "��Ʒ������һ�£���ȷ�ϣ�";
                return Page();
            }

            ReportData.Item = UploadForm.Item;
            ReportData.TargetInstrumentName = targetFileName;
            ReportData.MatchInstrumentName = matchFileName;

            // ������Ŀ��appsettings.json��ȡXc��ALE
            // Ҫ�Ƚ�appsettings���ΪUTF-8��ʽ���������Ļ�����
            // ��Ӹ��Ի����Tools -> Customize -> Commmands -> Menu(file) -> Add Commands
            var project = _projectParametersContext.ProjectParameter.FirstOrDefault(m => m.Name == UploadForm.Item);
            ReportData.ALE = project.ALE;
            ReportData.Xc1 = project.Xc1;
            ReportData.Xc2 = project.Xc2;

            int significantDigit = project.SignificantDigits;

            // �������nλ��Ч���ֺ��Զ��ŷָ����ַ������浽���ݿ�
            ReportData.SampleName = string.Join(",", targetResult.Select(kv => kv.Key).ToArray());
            ReportData.TargetResult = string.Join(",", targetResult.Select(kv => SignificantDigits.Reserved(kv.Value, significantDigit)).ToArray());
            ReportData.MatchResult = string.Join(",", matchResult.Select(kv => SignificantDigits.Reserved(kv.Value, significantDigit)).ToArray());

            // ��Ⱥֵ�б�
            IntegerVector OutliersList;
            try
            {
                CallRSource(out OutliersList);
            }
            catch (Exception e)
            {
                Message = "�޷�����R����ģ�飬����δ�ύ�ɹ���" + e.Message;
                return Page();
            }

            // �����������ж� ��ͨ����ֹͣ������ʾ
            if (OutliersList != null && OutliersList.Count() > 0)
            {
                Message = "��Ⱥֵ: ";
                Message += string.Join(',', from i in OutliersList
                                           let samplename = targetResult.ElementAt(i - 1).Key
                                           select samplename);
                return Page();
            }
            // PֵС��0.1
            if (ReportData.P < 0.1)
            {
                Message = $"����������������Բ�: P<0.10";
                return Page();
            }
            // ���SE/Xc < 1/2ALE
            // TODO

            return RedirectToPage("QuantitativeReports/Create", "Generate", ReportData);
        }

        /// <summary>
        /// Rģ��-Passing-Bablok regession
        /// </summary>
        /// <param name="OutliersList"></param>
        public void CallRSource(out IntegerVector OutliersList)
        {
            // Ҫ�� "C:\R-3.5.3\bin\x64 ��ӵ�ϵͳ����·����
            // The problem is actually not that stats.dll could not be found, 
            // but that a.dll that stats.dll depends on could not be loaded - 
            // I've found this out by inspecting the stats.dll file using a tool called Dependency Walker. 
            // In fact, while loading the stats package, the files "R.dll", "Rblas.dll" and
            // "Rlapack.dll" could not be found - they all lie in the "bin" directory of the R installation, e.g. "R-3.5.3\bin\x64".
            // https://github.com/NetLogo/R-Extension/issues/5 @gunnardressler

            REngine engine;
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();

            // ����ͼ����·��
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
            engine.SetSymbol("���ϵͳA���", engine.CreateNumericVector(ReportData.TargetResult.Split(",").Select(x => double.Parse(x)).ToList()));
            engine.SetSymbol("���ϵͳB���", engine.CreateNumericVector(ReportData.MatchResult.Split(",").Select(x => double.Parse(x)).ToList()));

            // ���� R
            string RSourcePath = Path.Combine(_WebHostEnvironment.WebRootPath, "�����ȶԱ�����.R").Replace("\\", "/");
            engine.Evaluate("source('" + RSourcePath + "')");

            // ��Ⱥֵ�����±��б�
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
    /// ��num����nλ��Ч����
    /// �������������忼�ǣ����ǿվͽ�һ�����Ϊ�տ���ż����ǰΪżӦ��ȥ����ǰΪ��Ҫ��һ
    /// </summary>
    public class SignificantDigits
    {
        public static string Reserved(double num, int n)
        {
            if (num == 0) return "0";

            if (n <= 0 || n > 28)
                throw new ArgumentOutOfRangeException("��Ч����λ������С�ڵ���0�����28");

            decimal result, number;
            try
            {
                number = decimal.Parse(num.ToString());
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("�Ƿ�����");
            }

            int time = 0;   // number����10�����10�Ĵ���
            int pointIndex = number.ToString().IndexOf('.');  // С����λ��(������-1, ��С����>=1, ������2��
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
                // num����Ϊ1.000ʱ���Զ�ת��Ϊ1
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
