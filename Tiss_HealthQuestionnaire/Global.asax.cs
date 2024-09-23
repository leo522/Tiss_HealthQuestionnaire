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

        // �ϥ� Application_AcquireRequestState �Ӷi��n�J�ˬd
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // �ˬd��e�� Session ���A�O�_�i��
            if (HttpContext.Current.Session != null)
            {
                // ���o��e�ШD�� URL
                var url = Request.Url.AbsolutePath.ToLower();

                // �P�_�O�_���n�J�ε��U����
                bool isLoginPage = url.Contains("/account/login");
                bool isRegisterPage = url.Contains("/account/register");

                // �p�G���O�n�J�ε��U�����A�h�ˬd�O�_�w�n�J
                if (!isLoginPage && !isRegisterPage)
                {
                    // �ˬd Session ���A�O�_���w�n�J
                    if (Session["LoggedIn"] == null || !(bool)Session["LoggedIn"])
                    {
                        // �p�G���n�J�A�h���w�V�ܵn�J����
                        Response.Redirect("~/Account/Login");
                    }
                }
            }
        }
    }
}
