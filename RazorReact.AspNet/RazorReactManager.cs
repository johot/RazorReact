using System;
using System.Collections.Generic;
using System.Web;
using JavaScriptEngineSwitcher.Core;
using RazorReact.Core;

namespace RazorReact.AspNet
{
    public class RazorReactManager : RazorReactManagerBase
    {
        public RazorReactManager(ReactBundle reactBundle, IJsEngineFactory jsEngineFactory, RazorReactOptions options = null) : base(reactBundle, new ServerPathMapper(), jsEngineFactory, options)
        {
        }
    }
}

// RazorReactConfiguration.Add