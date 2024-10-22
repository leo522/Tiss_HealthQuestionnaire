using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class ConcentrationViewModel
    {
        public string ListA { get; set; }
        public string ListB { get; set; }
        public string ListC { get; set; }
        public int OrderNumber { get; set; } // 項次
    }
}