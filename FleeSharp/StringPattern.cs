namespace Flee
{
    internal class StringPattern : CustomTokenPattern
    {
        protected override void ComputeToken(int id, string name, PatternType type, string pattern, ExpressionContext context)
        {
            var options = context.ParserOptions;
            pattern = pattern.Replace('"', options.StringQuote);
            this.SetData(id, name, type, pattern);
        }
    }
}