using RazorReact.AspNet;
using RazorReact.Core;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class RazorReactHelpers
    {
        public static IHtmlString RazorReact(this HtmlHelper htmlHelper, string componentName, object props, string bundleId = null, string containerId = null, RazorReactOptions options = null)
        {
            var razorReactManager = RazorReactConfiguration.GetReactBundleManager(bundleId);
            var htmlStringBuilder = new StringBuilder();
            htmlStringBuilder.Append(razorReactManager.GetServerSideRenderedHtml(componentName, props, bundleId, containerId, options));
            htmlStringBuilder.Append(razorReactManager.GetClientSideRenderScripts(componentName, props, bundleId, containerId, options));
            return new HtmlString(htmlStringBuilder.ToString());
        }
    }
}