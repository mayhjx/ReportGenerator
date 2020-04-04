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
    }
}
