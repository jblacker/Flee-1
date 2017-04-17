using System;
using System.Collections.Generic;
using System.Resources;

namespace Flee
{
    internal class FleeResourceManager
    {
        private Dictionary<string, ResourceManager> MyResourceManagers;

        private static FleeResourceManager OurInstance = new FleeResourceManager();

        public static FleeResourceManager Instance
        {
            get
            {
                return OurInstance;
            }
        }

        private FleeResourceManager()
        {
            this.MyResourceManagers = new Dictionary<string, ResourceManager>(StringComparer.OrdinalIgnoreCase);
        }

        private ResourceManager GetResourceManager(string resourceFile)
        {
            ResourceManager GetResourceManager;
            lock (this)
            {
                ResourceManager rm = null;
                bool flag = !this.MyResourceManagers.TryGetValue(resourceFile, out rm);
                if (flag)
                {
                    Type t = typeof(FleeResourceManager);
                    rm = new ResourceManager(string.Format("{0}.{1}", t.Namespace, resourceFile), t.Assembly);
                    this.MyResourceManagers.Add(resourceFile, rm);
                }
                GetResourceManager = rm;
            }
            return GetResourceManager;
        }

        private string GetResourceString(string resourceFile, string key)
        {
            ResourceManager rm = this.GetResourceManager(resourceFile);
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