using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class ArgumentSeparatorPattern : CustomTokenPattern
    {
        protected override void ComputeToken(int id, string name, PatternType type, string pattern, ExpressionContext context)
        {
            var options = context.ParserOptions;
            this.SetData(id, name, type, Conversions.ToString(options.FunctionArgumentSeparator));
        }
    }
}