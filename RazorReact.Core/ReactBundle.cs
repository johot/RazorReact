using System;
using System.Collections.Generic;
using System.Text;

namespace RazorReact.Core
{
    public class ReactBundle
    {
        public IEnumerable<string> BundleFiles { get; }

        public ReactBundle(string bundleFile)
        {
            this.BundleFiles = new[] { bundleFile };
        }

        public ReactBundle(IEnumerable<string> bundleFiles)
        {
            this.BundleFiles = bundleFiles;
        }
    }
}
