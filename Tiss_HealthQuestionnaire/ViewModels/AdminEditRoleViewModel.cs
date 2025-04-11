using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 使用者角色編輯
    public class AdminEditRoleViewModel
	{
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int RoleID { get; set; }
    }
    #endregion
}