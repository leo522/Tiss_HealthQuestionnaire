using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 批次建立選手資料
    public class BatchAthleteCreateViewModel
    {
        public List<SingleAthleteViewModel> Athletes { get; set; }
    }

    public class SingleAthleteViewModel
    {
        public string AthleteNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string SportSpecialization { get; set; }
        public DateTime? BirthDate { get; set; }
        public int GenderID { get; set; }
        public string Password { get; set; }
    }

    public class GenderViewModel
    {
        public int GenderID { get; set; }
        public string GenderName { get; set; }
    }
    #endregion
}