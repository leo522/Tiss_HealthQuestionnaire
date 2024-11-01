using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class ConcussionScreeningViewModel //腦震盪篩檢（選手自填）
    {
        public int OrderNumber { get; set; }  // 問卷項次
        public string Question { get; set; }  // 問卷問題
        public string Answer { get; set; }
        public string Notes { get; set; }
    }
}