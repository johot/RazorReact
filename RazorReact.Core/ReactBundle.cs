using System;
using System.Collections.Generic;
using System.Text;

namespace RazorReact.Core
{
    public class ReactBundle
    {
        public IEnumerable<string> BundleFiles { get; }
        public string BundleId { get; }

        public ReactBundle(string bundleId, string bundleFile)
        {
            this.BundleId = bundleId;
            this.BundleFiles = new[] { bundleFile };
        }

        public ReactBundle(string bundleId, IEnumerable<string> bundleFiles)
        {
            this.BundleId = bundleId;
            this.BundleFiles = bundleFiles;
        }

        public ReactBundle(IEnumerable<string> bundleFiles)
        {
            this.BundleId = null;
            this.BundleFiles = bundleFiles;
        }
    }
}
