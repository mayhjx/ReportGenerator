using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ReportGenerator.Models
{
    public class UploadForm
    {
        [Required(ErrorMessage = "请选择检测项目")]
        public string Item { get; set; }

        [Required(ErrorMessage = "请提交靶仪器数据文件")]
        public IFormFile TargetFile { get; set; }

        [Required(ErrorMessage = "请提交比对仪器数据文件")]
        public IFormFile MatchFile { get; set; }

        [Required(ErrorMessage = "请选择一个报告模板")]
        public string Template { get; set; }
    }
}
