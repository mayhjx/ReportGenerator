using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReportGenerator.Data;
using ReportGenerator.Models;
using ReportGenerator.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReportGenerator.Pages.QuantitativeReports
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly ProjectParametersContext _projectParametersContext;
        private readonly ReportContext _context;

        public CreateModel(ReportContext context,
                        ILogger<IndexModel> logger,
                        IWebHostEnvironment webHostEnvironment,
                        ProjectParametersContext projectParametersContext)
        {
            _context = context;
            _logger = logger;
            _WebHostEnvironment = webHostEnvironment;
            _projectParametersContext = projectParametersContext;
            // 从项目参数表中获取检测项目名称，在前端生成datalist
            Options = from project in _projectParametersContext.ProjectParameter.ToList()
                      select project.Name;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "请选择检测项目")]
            public string Item { get; set; }

            [Required(ErrorMessage = "请输入靶仪器编号")]
            public string TargetNum { get; set; }

            [Required(ErrorMessage = "请提交仪器数据文件")]
            public IFormFile DataFile { get; set; }

            [Required(ErrorMessage = "请输入比对仪器编号")]
            public string MatchNum { get; set; }

            [Required(ErrorMessage = "请选择一个报告模板")]
            public string Template { get; set; }
        }

        [BindProperty]
        public InputModel Upload { get; set; }

        [BindProperty]
        public Report Report { get; set; }

        // 提示信息
        public string Message { get; private set; } = "";

        [BindProperty]
        public IEnumerable<string> Options { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var project = await _projectParametersContext.ProjectParameter.FirstOrDefaultAsync(m => m.Name == Upload.Item);
            if (project == null)
            {
                Message = $"未找到{Upload.Item}的项目参数！";
                return Page();
            }

            Report.Status = "待审核";
            Report.Item = Upload.Item;
            Report.TargetInstrumentName = Upload.TargetNum;
            Report.MatchInstrumentName = Upload.MatchNum;

            Report.ALE = project.ALE / 100;
            Report.Xc1 = project.Xc1;
            Report.Xc2 = (double)project.Xc2;
            Report.Unit = project.Unit;

            // 读取仪器数据
            using var DataFileStream = Upload.DataFile.OpenReadStream();
            string DataStream;
            using (StreamReader sr = new StreamReader(DataFileStream))
            {
                DataStream = await sr.ReadToEndAsync();
            }

            // 查找BD编号的样本结果
            var targetResult = new Dictionary<string, double>();
            var matchResult = new Dictionary<string, double>();

            foreach (var line in DataStream.Split("\r\n"))
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
                        Message = $"无法识别的靶仪器数据: {data[0]} {data[1]}";
                        return Page();
                    }

                    try
                    {
                        matchResult.Add(data[0], double.Parse(data[2]));
                    }
                    catch
                    {
                        Message = $"无法识别的比对仪器数据: {data[0]} {data[2]}";
                        return Page();
                    }
                }
            }

            var sampleNameList = new List<string>();
            var targetResultList = new List<double>();
            var matchResultList = new List<double>();
            sampleNameList.AddRange(targetResult.Keys);
            targetResultList.AddRange(targetResult.Values);
            matchResultList.AddRange(matchResult.Values);

            var comparison = new Comparison(project,
                sampleNameList,
                Upload.TargetNum,
                targetResultList,
                Upload.MatchNum,
                matchResultList,
                _WebHostEnvironment.WebRootPath);

            if (!comparison.NumberIsEqual())
            {
                Message = "样品数量不一致，请确认！";
                return Page();
            }

            if (!comparison.NumberIsLargerThan19())
            {
                Message = "样品数量小于20个！";
                return Page();
            }

            if (!comparison.VerifySampleResultIsDifferent())
            {
                Message = "靶仪器和比对仪器的结果一样，请确认原始数据是否正确";
                return Page();
            }

            try
            {
                comparison.Run();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Page();
            }

            Report.SampleName = comparison.GetSampleNameList();
            Report.TargetResult = comparison.GetTargetSampleList();
            Report.MatchResult = comparison.GetMatchSampleList();
            Report.Bias = comparison.GetBias();
            Report.YorN = comparison.GetYorN();

            Report.P = comparison.P;
            Report.b = comparison.b;
            Report.bUCI = comparison.bUCI;
            Report.bLCI = comparison.bLCI;
            Report.a = comparison.a;
            Report.aUCI = comparison.aUCI;
            Report.aLCI = comparison.aLCI;

            Report.PicturePath = comparison.PicturePath;
            Report.Remark = comparison.Remark;
            Report.Status = comparison.Status;

            _context.Report.Add(Report);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = Report.ID });
        }
    }
}
