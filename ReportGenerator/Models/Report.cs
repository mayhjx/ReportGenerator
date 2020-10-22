using System;
using System.ComponentModel.DataAnnotations;

namespace ReportGenerator.Models
{
    public class Report
    {
        public int ID { get; set; }

        [Display(Name = "状态")]
        public string Status { get; set; } // 待审核/已审核/未通过？

        [Display(Name = "比对项目")]
        public string Item { get; set; }

        [Display(Name = "单位")]
        public string Unit { get; set; }

        [Display(Name = "靶仪器")]
        public string TargetInstrumentName { get; set; }

        [Display(Name = "比对仪器")]
        public string MatchInstrumentName { get; set; }

        public string TargetReagentLot { get; set; } // 靶仪器检测试剂/批号

        public string MatchReagentLot { get; set; } // 比对仪器检测试剂/批号

        [DataType(DataType.Date)]
        public DateTime? StartTestDate { get; set; } // 开始检测日期

        [DataType(DataType.Date)]
        public DateTime? EndTestDate { get; set; } // 结束检测日期

        [DataType(DataType.Date)]
        public DateTime? EvaluationDate { get; set; } // 评估日期

        public string Technician { get; set; } // 检测人员

        public string SampleName { get; set; } // 实验号（逗号分隔）

        public string TargetResult { get; set; } // 靶仪器数据（逗号分隔）

        public string MatchResult { get; set; } // 比对仪器数据（逗号分隔）

        public string Bias { get; set; } // 偏倚（逗号分隔）

        public string YorN { get; set; } // 一致性判断（逗号分隔）

        [Display(Name = "ALE(%)")]
        public double ALE { get; set; } // 最大允许误差

        public double Xc1 { get; set; } // 医学决定水平一

        public double Xc2 { get; set; } // 医学决定水平二

        public double a { get; set; } // 截距

        public double aUCI { get; set; }

        public double aLCI { get; set; }

        public double b { get; set; } // 斜率

        public double bUCI { get; set; }

        public double bLCI { get; set; }

        public double P { get; set; }

        public string PicturePath { get; set; }

        [Display(Name = "调查者")]
        public string Investigator { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "调查日期")]
        public DateTime? InvestigationDate { get; set; }

        [Display(Name = "审核者")]
        public string Approver { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "审核日期")]
        public DateTime? ApprovalDate { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
