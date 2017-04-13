using System;
using System.Collections.Generic;

namespace Ciloci.Flee
{
	public sealed class ExpressionInfo
	{
		private IDictionary<string, object> MyData;

		internal ExpressionInfo()
		{
			this.MyData = new Dictionary<string, object>();
			this.MyData.Add("ReferencedVariables", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
		}

		internal void AddReferencedVariable(string name)
		{
			IDictionary<string, string> dict = (IDictionary<string, string>)this.MyData["ReferencedVariables"];
			dict[name] = name;
		}

		public string[] GetReferencedVariables()
		{
			IDictionary<string, string> dict = (IDictionary<string, string>)this.MyData["ReferencedVariables"];
			string[] arr = new string[dict.Count - 1 + 1];
			dict.Keys.CopyTo(arr, 0);
			return arr;
		}
	}
}
