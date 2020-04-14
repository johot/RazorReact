using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
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

            InitializeRazorReact();
        }

        private static void InitializeRazorReact()
        {
            //var reactBundle = new ReactBundle(@"~\ReactScripts\main.bundle.js");
            var reactBundle = new ReactBundle(null, new[] { @"http://localhost:5000/main.bundle.js", @"http://localhost:5000/runtime.bundle.js", @"http://localhost:5000/vendors~main.bundle.js", });

            RazorReactConfiguration.AddReactManager(new RazorReactManager(reactBundle, reactBundle, new ChakraCoreJsEngineFactory(), new RazorReactOptions() { LiveReloadDevMode = true }));
            RazorReactConfiguration.Initialize();
        }
    }
}
