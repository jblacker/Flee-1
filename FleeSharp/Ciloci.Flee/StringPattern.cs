using Ciloci.Flee.PerCederberg.Grammatica.Runtime;
using System;

namespace Ciloci.Flee
{
	internal class StringPattern : CustomTokenPattern
	{
		protected override void ComputeToken(int id, string name, TokenPattern.PatternType type, string pattern, ExpressionContext context)
		{
			ExpressionParserOptions options = context.ParserOptions;
			pattern = pattern.Replace('"', options.StringQuote);
			base.SetData(id, name, type, pattern);
		}
	}
}
