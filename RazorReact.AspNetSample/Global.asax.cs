using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using RazorReact.AspNet;
using RazorReact.Core;
using System.IO;
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
            var distPath = Path.GetFullPath(Path.Combine(HttpContext.Current.Server.MapPath(""), "..", "react-sample-app", "dist"));

            var reactServerSideBundle = new ReactBundle(null, new[] { @"~/Scripts/react/server/main.bundle.js", @"~/Scripts/react/server/runtime.bundle.js", @"~/Scripts/react/server/vendors~main.bundle.js", });
            var reactClientSideBundle = new ReactBundle(null, new[] { @"~/Scripts/react/client/main.bundle.js", @"~/Scripts/react/client/runtime.bundle.js", @"~/Scripts/react/client/vendors~main.bundle.js", });

            //RazorReactConfiguration.AddReactManager(new RazorReactManager(reactServerSideBundle, reactClientSideBundle, new ChakraCoreJsEngineFactory(), new RazorReactOptions() { LiveReloadDevMode = true }));
            RazorReactConfiguration.AddReactManager(new RazorReactManager(reactServerSideBundle, reactClientSideBundle, new V8JsEngineFactory(), new RazorReactOptions() { LiveReloadDevMode = true }));

            RazorReactConfiguration.Initialize();
        }
    }
}
