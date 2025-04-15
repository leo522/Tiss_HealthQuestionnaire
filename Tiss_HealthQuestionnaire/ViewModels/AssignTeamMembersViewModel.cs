using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tiss_HealthQuestionnaire.Models;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 指派選手與防護員至隊伍
    public class AssignTeamMembersViewModel
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }

        public List<int> SelectedAthleteIDs { get; set; } = new List<int>();
        public List<int> SelectedTrainerIDs { get; set; } = new List<int>();

        public List<AthleteOption> AllAthletes { get; set; } = new List<AthleteOption>();
        public List<TrainerOption> AllTrainers { get; set; } = new List<TrainerOption>();
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