using System;
using System.Collections.Generic;

namespace Ciloci.Flee.CalcEngine
{
	public sealed class BatchLoader
	{
		private IDictionary<string, BatchLoadInfo> MyNameInfoMap;

		private DependencyManager<string> MyDependencies;

		internal BatchLoader()
		{
			this.MyNameInfoMap = new Dictionary<string, BatchLoadInfo>(StringComparer.OrdinalIgnoreCase);
			this.MyDependencies = new DependencyManager<string>(StringComparer.OrdinalIgnoreCase);
		}

		public void Add(string atomName, string expression, ExpressionContext context)
		{
			Utility.AssertNotNull(atomName, "atomName");
			Utility.AssertNotNull(expression, "expression");
			Utility.AssertNotNull(context, "context");
			BatchLoadInfo info = new BatchLoadInfo(atomName, expression, context);
			this.MyNameInfoMap.Add(atomName, info);
			this.MyDependencies.AddTail(atomName);
			ICollection<string> references = this.GetReferences(expression, context);
			try
			{
				IEnumerator<string> enumerator = references.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string reference = enumerator.Current;
					this.MyDependencies.AddTail(reference);
					this.MyDependencies.AddDepedency(reference, atomName);
				}
			}
			finally
			{
				IEnumerator<string> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}

		public bool Contains(string atomName)
		{
			return this.MyNameInfoMap.ContainsKey(atomName);
		}

		internal BatchLoadInfo[] GetBachInfos()
		{
			string[] tails = this.MyDependencies.GetTails();
			Queue<string> sources = this.MyDependencies.GetSources(tails);
			IList<string> result = this.MyDependencies.TopologicalSort(sources);
			BatchLoadInfo[] infos = new BatchLoadInfo[result.Count - 1 + 1];
			int num = result.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				infos[i] = this.MyNameInfoMap[result[i]];
			}
			return infos;
		}

		private ICollection<string> GetReferences(string expression, ExpressionContext context)
		{
			IdentifierAnalyzer analyzer = context.ParseIdentifiers(expression);
			return analyzer.GetIdentifiers(context);
		}
	}
}
