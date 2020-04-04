using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ReportGenerator.Models
{
    public class Report
    {
        public enum ReportStatus
        {
            Submitted, // 待审核
            Approved, // 已审核
            Rejected // 拒绝
        }
        public int ID { get; set; }

        [Display(Name = "状态")]
        public ReportStatus Status { get; set; } // 报告状态（待审核/已审核）

        [Display(Name = "比对项目")]
        public string Item { get; set; } // 检测项目

        public string TargetInstrumentName { get; set; } // 靶仪器名称

        public string MatchInstrumentName { get; set; } // 比对仪器名称

        public string TargetReagentLot { get; set; } // 靶仪器检测试剂/批号

        public string MatchReagentLot { get; set; } // 比对仪器检测试剂/批号

        [DataType(DataType.Date)]
        public DateTime StartTestDate { get; set; } // 开始检测日期

        [DataType(DataType.Date)]
        public DateTime EndTestDate { get; set; } // 结束检测日期

        [DataType(DataType.Date)]
        public DateTime EvaluationDate { get; set; } // 评估日期

        public string Technician { get; set; } // 检测人员

        public string SampleName { get; set; } // 实验号（逗号分隔）

        public string TargetResult { get; set; } // 靶仪器数据（逗号分隔）

        public string MatchResult { get; set; } // 比对仪器数据（逗号分隔）

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
        public string Investigator { get; set; } // 调查者

        [DataType(DataType.Date)]
        public DateTime InvestigationDate { get; set; } // 调查日期

        [Display(Name = "审核者")]
        public string Approver { get; set; } // 学科主任(Section director)

        [DataType(DataType.Date)]
        public DateTime ApprovalDate { get; set; } // 审批日期
    }
}
