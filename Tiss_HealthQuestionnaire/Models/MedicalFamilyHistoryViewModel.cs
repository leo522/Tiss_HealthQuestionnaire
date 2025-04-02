using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class MedicalFamilyHistoryViewModel
    {
        public string Question { get; set; }
        public List<string> Symptoms { get; set; }
    }
}