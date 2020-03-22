using System.Collections.Generic;

namespace RazorReact.Core
{
    public interface IRazorReactManager
    {
        IEnumerable<ReactBundle> ReactBundles { get; }

        string GetClientSideRenderScripts(string componentName, object props = null, RazorReactOptions razorReactOptions = null);
        string GetServerSideRenderedHtml(string componentName, object props = null, RazorReactOptions razorReactOptions = null);
    }
}