using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 防護員對應的選手清單與隊伍資訊
    public class TrainerTeamInfoViewModel
	{
        public int TrainerID { get; set; }
        public string TrainerName { get; set; }

        public List<TeamAthletesViewModel> Teams { get; set; } = new List<TeamAthletesViewModel>();
    }

    public class TeamAthletesViewModel
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public List<string> AthleteNames { get; set; } = new List<string>();
    }
    #endregion
}