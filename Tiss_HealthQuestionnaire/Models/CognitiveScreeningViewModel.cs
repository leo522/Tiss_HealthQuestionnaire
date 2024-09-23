using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class CognitiveScreeningViewModel
    {
        public int OrderNumber { get; set; }  // 項次
        public string Question { get; set; }  // 問題文字
        public int OrientationScore { get; set; } // 定位分數
        public int ImmediateMemoryScore { get; set; } // 短期記憶分數
        public int ConcentrationScore { get; set; } // 專注力分數
        public int DelayedRecallScore { get; set; } // 延遲記憶分數
        public int TotalScore { get; set; } // 總分
    }
}