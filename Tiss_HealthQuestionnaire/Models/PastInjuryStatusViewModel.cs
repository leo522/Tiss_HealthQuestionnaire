using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class PastInjuryStatusViewModel
    {
        public int Id { get; set; }
        public string PastInjuryPart { get; set; }
        public bool PastIsSingleSelect { get; set; }
    }
}