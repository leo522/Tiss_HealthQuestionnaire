using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
    #region 使用者清單總覽
    public class AdminUserViewModel
	{
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string RoleName { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
    }
    #endregion
}