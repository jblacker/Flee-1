using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class RealPattern : CustomTokenPattern
    {
        protected override void ComputeToken(int id, string name, PatternType type, string pattern, ExpressionContext context)
        {
            var options = context.ParserOptions;
            var digitsBeforePattern = Conversions.ToChar(Interaction.IIf(options.RequireDigitsBeforeDecimalPoint, '+', '*'));
            pattern = string.Format(pattern, digitsBeforePattern, options.DecimalSeparator);
            this.SetData(id, name, type, pattern);
        }
    }
}