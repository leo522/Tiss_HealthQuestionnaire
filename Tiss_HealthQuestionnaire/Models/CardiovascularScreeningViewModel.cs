using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class CardiovascularScreeningViewModel //心血管篩檢
    {
        public int OrderNumber { get; set; }  // 問卷項次
        public string Question { get; set; }  // 問卷問題
        public string Questions { get; set; }
        public string Answer { get; set; }
    }
}