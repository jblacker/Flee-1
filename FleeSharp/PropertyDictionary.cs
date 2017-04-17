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
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualBasic.CompilerServices;

    internal class PropertyDictionary
    {
        private readonly Dictionary<string, object> myProperties;

        public PropertyDictionary()
        {
            this.myProperties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public PropertyDictionary Clone()
        {
            var copy = new PropertyDictionary();
            foreach (var kvp in this.myProperties)
            {
                copy.SetValue(kvp.Key, RuntimeHelpers.GetObjectValue(kvp.Value));
            }

            //try
            //{
            //    var enumerator = this.myProperties.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var pair = enumerator.Current;
            //        copy.SetValue(pair.Key, RuntimeHelpers.GetObjectValue(pair.Value));
            //    }
            //}
            //finally
            //{
            //    Dictionary<string, object>.Enumerator enumerator;
            //    ((IDisposable)enumerator).Dispose();
            //}
            return copy;
        }

        public T GetValue<T>(string name)
        {
            var properties = this.myProperties;
            object value2;
            var propertyValue = properties.TryGetValue(name, out value2) ? 1 : 0;
            var value = Conversions.ToGenericParameter<T>(value2);
            var flag = propertyValue == 0;
            if (flag)
            {
                Debug.Fail($"Unknown property '{name}'");
            }
            return value;
        }

        public void SetToDefault<T>(string name)
        {
            this.SetValue(name, default(T));
        }

        public void SetValue(string name, object value)
        {
            this.myProperties[name] = RuntimeHelpers.GetObjectValue(value);
        }

        public bool Contains(string name)
        {
            return this.myProperties.ContainsKey(name);
        }
    }
}