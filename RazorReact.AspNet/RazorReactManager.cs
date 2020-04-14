using System;
using System.Collections.Generic;
using System.Web;
using JavaScriptEngineSwitcher.Core;
using RazorReact.Core;

namespace RazorReact.AspNet
{
    public class RazorReactManager : RazorReactManagerBase
    {
        public RazorReactManager(ReactBundle reactServerSideBundle,ReactBundle reactClientSideBundle, IJsEngineFactory jsEngineFactory, RazorReactOptions options = null) : base(reactServerSideBundle, reactClientSideBundle, new ServerPathMapper(), jsEngineFactory, options)
        {
        }
    }
}

// RazorReactConfiguration.Add