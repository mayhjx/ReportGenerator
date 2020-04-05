using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace ReportGenerator.Models
{
    public class ProjectParameter
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "项目名称")]
        public string Name { get; set; }

        [Display(Name = "最大允许误差")]
        public double ALE { get; set; }

        [Display(Name = "医学决定水平一")]
        public double Xc1 { get; set; }

        [Display(Name = "医学决定水平二")]
        public double Xc2 { get; set; }

        [Display(Name = "结果有效数字位数")]
        public int SignificantDigits { get; set; }

    }
}
