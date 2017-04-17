namespace Flee
{
    public interface IExpression
    {
        string Text
        {
            get;
        }

        ExpressionInfo Info
        {
            get;
        }

        ExpressionContext Context
        {
            get;
        }

        object Owner
        {
            get;
            set;
        }

        IExpression Clone();
    }
}