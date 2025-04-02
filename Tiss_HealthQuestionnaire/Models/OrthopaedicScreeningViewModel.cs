using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class OrthopaedicScreeningViewModel
    {
        public int OrderNumber { get; set; }
        public string Instructions { get; set; }
        public string ObservationPoints { get; set; }
    }
}