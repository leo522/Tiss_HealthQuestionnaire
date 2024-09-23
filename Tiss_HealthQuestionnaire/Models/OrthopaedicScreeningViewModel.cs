using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class OrthopaedicScreeningViewModel
    {
        public int OrderNumber { get; set; }  // 項次
        public string Instructions { get; set; }  // 指示操作
        public string ObservationPoints { get; set; }  // 觀察重點
    }
}