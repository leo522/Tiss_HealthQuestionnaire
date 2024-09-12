using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class CardiovascularScreeningViewModel
    {
        public int OrderNumber { get; set; }  // 問卷項次
        public string Question { get; set; }  // 問卷問題
    }
}