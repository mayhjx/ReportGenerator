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

        [Required]
        [Display(Name = "最大允许误差")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public double ALE { get; set; }

        [Required]
        [Display(Name = "医学决定水平一")]
        public double Xc1 { get; set; }

        [Required]
        [Display(Name = "医学决定水平二")]
        public double Xc2 { get; set; }

        [Required]
        [Display(Name = "结果有效数字位数")]
        [Range(0,10,ErrorMessage ="有效数字位数范围：0~10")]
        public int SignificantDigits { get; set; }

    }
}
