using Ciloci.Flee.PerCederberg.Grammatica.Runtime;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;

namespace Ciloci.Flee
{
	internal class RealPattern : CustomTokenPattern
	{
		protected override void ComputeToken(int id, string name, TokenPattern.PatternType type, string pattern, ExpressionContext context)
		{
			ExpressionParserOptions options = context.ParserOptions;
			char digitsBeforePattern = Conversions.ToChar(Interaction.IIf(options.RequireDigitsBeforeDecimalPoint, '+', '*'));
			pattern = string.Format(pattern, digitsBeforePattern, options.DecimalSeparator);
			base.SetData(id, name, type, pattern);
		}
	}
}
