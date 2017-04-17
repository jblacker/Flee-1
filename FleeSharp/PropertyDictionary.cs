using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class PropertyDictionary
    {
        private Dictionary<string, object> MyProperties;

        public PropertyDictionary()
        {
            this.MyProperties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public PropertyDictionary Clone()
        {
            PropertyDictionary copy = new PropertyDictionary();
            try
            {
                Dictionary<string, object>.Enumerator enumerator = this.MyProperties.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, object> pair = enumerator.Current;
                    copy.SetValue(pair.Key, RuntimeHelpers.GetObjectValue(pair.Value));
                }
            }
            finally
            {
                Dictionary<string, object>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            return copy;
        }

        public T GetValue<T>(string name)
        {
            T value = default(T);
            Dictionary<string, object> arg_19_0 = this.MyProperties;
            object value2 = value;
            int arg_26_0 = arg_19_0.TryGetValue(name, out value2) ? 1 : 0;
            value = Conversions.ToGenericParameter<T>(value2);
            bool flag = arg_26_0 == 0;
            if (flag)
            {
                Debug.Fail(string.Format("Unknown property '{0}'", name));
            }
            return value;
        }

        public void SetToDefault<T>(string name)
        {
            this.SetValue(name, default(T));
        }

        public void SetValue(string name, object value)
        {
            this.MyProperties[name] = RuntimeHelpers.GetObjectValue(value);
        }

        public bool Contains(string name)
        {
            return this.MyProperties.ContainsKey(name);
        }
    }
}