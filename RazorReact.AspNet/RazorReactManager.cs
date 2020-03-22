using System;
using System.Collections.Generic;
using System.Web;
using RazorReact.Core;

namespace RazorReact.AspNet
{
    public class RazorReactManager : RazorReactManagerBase
    {
        public RazorReactManager(IEnumerable<ReactBundle> reactBundles) : base(reactBundles, new ServerPathMapper())
        {
        }

        public static IRazorReactManager Current { get; private set; }

        public static void Initialize(ReactBundle reactBundle)
        {
            Initialize(new[] { reactBundle });
        }
        public static void Initialize(IEnumerable<ReactBundle> reactBundles)
        {
            if (Current == null)
            {
                Current = new RazorReactManager(reactBundles);
            }
            else
            {
                throw new Exception("Razor React manager has already been initialized");
            }
        }
    }
}
