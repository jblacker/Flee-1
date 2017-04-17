namespace Flee
{
    public interface IGenericExpression<T> : IExpression
    {
        T Evaluate();
    }
}