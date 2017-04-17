namespace Flee
{
    using System;
    using System.Collections.Generic;

    public sealed class ExpressionInfo
    {
        private readonly IDictionary<string, object> myData;

        internal ExpressionInfo()
        {
            this.myData = new Dictionary<string, object>
                {{"ReferencedVariables", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)}};
        }

        internal void AddReferencedVariable(string name)
        {
            var dict = (IDictionary<string, string>) this.myData["ReferencedVariables"];
            dict[name] = name;
        }

        public string[] GetReferencedVariables()
        {
            var dict = (IDictionary<string, string>) this.myData["ReferencedVariables"];
            var arr = new string[dict.Count - 1 + 1];
            dict.Keys.CopyTo(arr, 0);
            return arr;
        }
    }
}