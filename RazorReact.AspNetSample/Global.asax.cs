using RazorReact.AspNet;
using RazorReact.Core;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RazorReact.AspNetSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var reactBundle = new ReactBundle(@"~\ReactScripts\main.bundle.js");
            var reactBundle = new ReactBundle(@"http://localhost:5000/main.bundle.js");

            RazorReactManager.Initialize(reactBundle);
        }
    }
}
