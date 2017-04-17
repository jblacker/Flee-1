using System;
using System.Collections.Generic;

namespace Flee
{
    public sealed class ExpressionInfo
    {
        private readonly IDictionary<string, object> MyData;

        internal ExpressionInfo()
        {
            this.MyData = new Dictionary<string, object>();
            this.MyData.Add("ReferencedVariables", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        internal void AddReferencedVariable(string name)
        {
            var dict = (IDictionary<string, string>)this.MyData["ReferencedVariables"];
            dict[name] = name;
        }

        public string[] GetReferencedVariables()
        {
            var dict = (IDictionary<string, string>)this.MyData["ReferencedVariables"];
            var arr = new string[dict.Count - 1 + 1];
            dict.Keys.CopyTo(arr, 0);
            return arr;
        }
    }
}