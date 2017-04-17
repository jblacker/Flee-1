namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class StringPattern : CustomTokenPattern
    {
        protected override void ComputeToken(int id, string name, PatternType type, string pattern, ExpressionContext context)
        {
            ExpressionParserOptions options = context.ParserOptions;
            pattern = pattern.Replace('"', options.StringQuote);
            this.SetData(id, name, type, pattern);
        }
    }
}