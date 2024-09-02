using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiss_HealthQuestionnaire.Models
{
    public class AccountViewModel
    {
        public class ResetPasswordViewModel
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }
    }
}