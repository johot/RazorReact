using System;
using System.Collections.Generic;
using System.Text;

namespace RazorReact.Core
{
    public class RazorReactOptions
    {
        public string ContainerId { get; set; }

        public bool ClientSide { get; set; } = true;
        public bool ServerSide { get; set; } = true;

        public bool CacheRendering { get; set; } = true;
    }
}
