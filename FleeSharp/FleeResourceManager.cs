using System;
using System.Collections.Generic;
using System.Resources;

namespace Flee
{
    internal class FleeResourceManager
    {
        private readonly Dictionary<string, ResourceManager> myResourceManagers;

        public static FleeResourceManager Instance { get; } = new FleeResourceManager();

        private FleeResourceManager()
        {
            this.myResourceManagers = new Dictionary<string, ResourceManager>(StringComparer.OrdinalIgnoreCase);
        }

        private ResourceManager GetResourceManager(string resourceFile)
        {
            ResourceManager getResourceManager;
            lock (this)
            {
                ResourceManager rm;
                var flag = !this.myResourceManagers.TryGetValue(resourceFile, out rm);
                if (flag)
                {
                    var t = typeof(FleeResourceManager);
                    rm = new ResourceManager($"{t.Namespace}.{resourceFile}", t.Assembly);
                    this.myResourceManagers.Add(resourceFile, rm);
                }
                getResourceManager = rm;
            }
            return getResourceManager;
        }

        private string GetResourceString(string resourceFile, string key)
        {
            var rm = this.GetResourceManager(resourceFile);
            return rm.GetString(key);
        }

        public string GetCompileErrorString(string key)
        {
            return this.GetResourceString("CompileErrors", key);
        }

        public string GetElementNameString(string key)
        {
            return this.GetResourceString("ElementNames", key);
        }

        public string GetGeneralErrorString(string key)
        {
            return this.GetResourceString("GeneralErrors", key);
        }
    }
}