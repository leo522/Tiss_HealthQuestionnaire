using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class InjuryStatusViewModel
    {
        public int Id { get; set; }
        public string InjuryPart { get; set; }
        public bool IsSingleSelect { get; set; }
    }
}