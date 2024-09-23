using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Tiss_HealthQuestionnaire
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // 使用 Application_AcquireRequestState 來進行登入檢查
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // 檢查當前的 Session 狀態是否可用
            if (HttpContext.Current.Session != null)
            {
                // 取得當前請求的 URL
                var url = Request.Url.AbsolutePath.ToLower();

                // 判斷是否為登入或註冊頁面
                bool isLoginPage = url.Contains("/account/login");
                bool isRegisterPage = url.Contains("/account/register");

                // 如果不是登入或註冊頁面，則檢查是否已登入
                if (!isLoginPage && !isRegisterPage)
                {
                    // 檢查 Session 狀態是否為已登入
                    if (Session["LoggedIn"] == null || !(bool)Session["LoggedIn"])
                    {
                        // 如果未登入，則重定向至登入頁面
                        Response.Redirect("~/Account/Login");
                    }
                }
            }
        }
    }
}
