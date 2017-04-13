using Ciloci.Flee.PerCederberg.Grammatica.Runtime;
using System;

namespace Ciloci.Flee
{
	internal abstract class CustomTokenPattern : TokenPattern
	{
		public void Initialize(int id, string name, TokenPattern.PatternType type, string pattern, ExpressionContext context)
		{
			this.ComputeToken(id, name, type, pattern, context);
		}

		protected abstract void ComputeToken(int id, string name, TokenPattern.PatternType type, string pattern, ExpressionContext context);
	}
}
