using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Tiss_HealthQuestionnaire.Controllers
{
    public class BaseAuthController : Controller
    {
        protected virtual string[] AllowedRoles { get; set; } = new string[0];

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userRole = Session["UserRole"]?.ToString();

            if (string.IsNullOrEmpty(userRole))
            {
                // 尚未登入
                filterContext.Result = RedirectToLogin("尚未登入，請先選擇身份");
                return;
            }

            if (AllowedRoles.Length > 0 && Array.IndexOf(AllowedRoles, userRole.ToLower()) == -1)
            {
                // 沒有足夠權限
                filterContext.Result = RedirectToLogin("您沒有權限存取此頁面");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private RedirectToRouteResult RedirectToLogin(string message)
        {
            TempData["AccessDeniedMessage"] = message;
            return new RedirectToRouteResult(
                new RouteValueDictionary {
                    { "controller", "Account" },
                    { "action", "SelectRole" }
                });
        }
    }
}