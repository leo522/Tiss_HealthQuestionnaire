using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 隊伍管理 ViewModel
    public class TeamViewModel
	{
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public int? SportTypeID { get; set; }
        public string SportType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    #endregion
}