using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class CardiovascularScreeningViewModel
    {
        public int OrderNumber { get; set; }
        public string Question { get; set; }
        public string Questions { get; set; }
        public string Answer { get; set; }
    }
}