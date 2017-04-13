using Ciloci.Flee.PerCederberg.Grammatica.Runtime;
using Microsoft.VisualBasic.CompilerServices;
using System;

namespace Ciloci.Flee
{
	internal class ArgumentSeparatorPattern : CustomTokenPattern
	{
		protected override void ComputeToken(int id, string name, TokenPattern.PatternType type, string pattern, ExpressionContext context)
		{
			ExpressionParserOptions options = context.ParserOptions;
			base.SetData(id, name, type, Conversions.ToString(options.FunctionArgumentSeparator));
		}
	}
}
