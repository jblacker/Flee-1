// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp
{
    using System;
    using System.Collections.Generic;
    using System.Resources;

    internal class FleeResourceManager
    {
        private readonly Dictionary<string, ResourceManager> myResourceManagers;

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

        public static FleeResourceManager Instance { get; } = new FleeResourceManager();
    }
}