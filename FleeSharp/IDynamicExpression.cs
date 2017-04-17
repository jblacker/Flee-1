namespace Flee
{
    public interface IDynamicExpression : IExpression
    {
        object Evaluate();
    }
}