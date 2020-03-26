﻿using System.Collections.Generic;

namespace RazorReact.Core
{
    public interface IRazorReactManager
    {
        ReactBundle ReactBundle { get; }

        string GetClientSideRenderScripts(string componentName, object props, string bundleId = null, string containerId = null);
        string GetServerSideRenderedHtml(string componentName, object props, string bundleId = null, string containerId = null);

        void Initialize();
    }
}