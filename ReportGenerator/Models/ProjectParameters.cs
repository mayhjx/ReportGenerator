using System;
using System.ComponentModel.DataAnnotations;


namespace ReportGenerator.Models
{
    public class ProjectParameter
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "项目名称")]
        public string Name { get; set; }

        [Display(Name = "判断标准一适用范围(<=)")]
        public double? SpecificationOneConcRange { get; set; }

        [Display(Name = "判断标准一(差值)")]
        //[DisplayFormat(DataFormatString = "{0:P0}")]
        public double? SpecificationOne { get; set; }

        [Display(Name = "判断标准二适用范围(>)")]
        public double? SpecificationTwoConcRange { get; set; }

        [Display(Name = "判断标准二（%）")]
        //[DisplayFormat(DataFormatString = "{0:P0}")]
        public double? SpecificationTwo { get; set; }

        [Display(Name = "医学决定水平一")]
        public double Xc1 { get; set; }

        [Display(Name = "医学决定水平二")]
        public double? Xc2 { get; set; }

        [Display(Name = "有效数字位数")]
        [Range(0, 10, ErrorMessage = "有效数字位数范围：0~10")]
        public int SignificantDigits { get; set; }

        [Required]
        [Display(Name = "单位")]
        public string Unit { get; set; }

        public double LOQ { get; set; }

        [Display(Name = "ALE(%)")]
        //[DisplayFormat(DataFormatString = "{0:P0}")]
        public double ALE { get; set; }
    }
}
