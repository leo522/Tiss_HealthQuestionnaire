using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class DelayedRecallViewModel
    {
        public int OrderNumber { get; set; } //項次，用於顯示題目的順序

        public string Word { get; set; } //詞彙內容
    }
}