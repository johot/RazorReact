using System;
using System.Collections.Generic;
using System.Text;

namespace RazorReact.Core
{
    public class RazorReactOptions
    {
        public bool ClientSide { get; set; } = true;
        public bool ServerSide { get; set; } = true;

        public bool CacheRendering { get; set; } = true;

        public bool LiveReloadDevMode { get; set; } = false; // Will disable caching and always reinitialize scripts during render, will make everything slower
    }
}
