using System;
using System.Collections.Generic;
using System.Web;
using JavaScriptEngineSwitcher.Core;
using RazorReact.Core;

namespace RazorReact.AspNet
{
    public class RazorReactManager : RazorReactManagerBase
    {
        public RazorReactManager(IEnumerable<ReactBundle> reactBundles, IJsEngine jsEngine) : base(reactBundles, new ServerPathMapper(), jsEngine)
        {
        }

        public static IRazorReactManager Current { get; private set; }

        public static void Initialize(ReactBundle reactBundle, IJsEngine jsEngine)
        {
            Initialize(new[] { reactBundle }, jsEngine);
        }
        public static void Initialize(IEnumerable<ReactBundle> reactBundles, IJsEngine jsEngine)
        {
            if (Current == null)
            {
                Current = new RazorReactManager(reactBundles, jsEngine);
            }
            else
            {
                throw new Exception("Razor React manager has already been initialized");
            }
        }
    }
}
