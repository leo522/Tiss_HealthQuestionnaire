using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.ViewModels
{
	public class ChangeUserPwdViewModel
	{
        public string SelectedRole { get; set; }
        public string SelectedAccount { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}