using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RazorReact.Core
{
    public static class RazorReactConfiguration
    {
        public static IEnumerable<IRazorReactManager> ReactManagers { get; private set; } = new List<IRazorReactManager>();

        public static void AddReactManager(IRazorReactManager reactManager)
        {
            (ReactManagers as IList<IRazorReactManager>).Add(reactManager);
        }

        public static IRazorReactManager GetReactBundleManager(string bundleId)
        {
            var matchingReactManager = ReactManagers.FirstOrDefault(rm => rm.ReactBundle.BundleId == bundleId);

            if (matchingReactManager == null)
            {
                throw new Exception("No RazorReactManager found for bundle id: " + bundleId);
            }

            return matchingReactManager;
        }

        public static void Initialize()
        {
            foreach (var reactManager in ReactManagers)
            {
                reactManager.Initialize();
            }
        }
    }
}
