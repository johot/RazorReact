using RazorReact.AspNet;
using RazorReact.Core;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class RazorReactHelpers
    {
        public static IHtmlString RazorReact(this HtmlHelper htmlHelper, string componentName, object props = null, RazorReactOptions razorReactOptions = null) 
        {
            var htmlStringBuilder = new StringBuilder();
            htmlStringBuilder.Append(RazorReactManager.Current.GetServerSideRenderedHtml(componentName, props, razorReactOptions));
            htmlStringBuilder.Append(RazorReactManager.Current.GetClientSideRenderScripts(componentName, props, razorReactOptions));
            return new HtmlString(htmlStringBuilder.ToString());
        }
    }
}