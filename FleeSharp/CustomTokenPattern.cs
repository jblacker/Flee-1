namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal abstract class CustomTokenPattern : TokenPattern
    {
        public void Initialize(int id, string name, PatternType type, string pattern, ExpressionContext context)
        {
            this.ComputeToken(id, name, type, pattern, context);
        }

        protected abstract void ComputeToken(int id, string name, PatternType type, string pattern, ExpressionContext context);
    }
}