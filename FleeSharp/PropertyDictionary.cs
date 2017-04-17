using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
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