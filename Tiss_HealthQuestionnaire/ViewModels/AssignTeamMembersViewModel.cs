using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 刪除隊伍
    public class AssignTeamMembersViewModel
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }

        public List<int> SelectedAthleteIDs { get; set; }
        public List<int> SelectedTrainerIDs { get; set; }

        public List<AthleteProfile> AllAthletes { get; set; } = new List<AthleteProfile>();
        public List<TrainerProfile> AllTrainers { get; set; } = new List<TrainerProfile>();
    }

    public class AthleteOption
    {
        public int AthleteID { get; set; }
        public string AthleteName { get; set; }
    }

    public class TrainerOption
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; }
    }
    #endregion
}