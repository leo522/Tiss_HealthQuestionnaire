using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    public class AdminLogFilterViewModel
    {
        public string UserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<AdminLogItemViewModel> Results { get; set; } = new List<AdminLogItemViewModel>();
    }

    public class AdminLogItemViewModel
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
        public DateTime LogDate { get; set; }
    }
}