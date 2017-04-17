namespace Flee.CalculationEngine
{
    using System;

    internal class GenericExpressionResultPair<T> : ExpressionResultPair
	{
		public T expressionResultPair;

		public T ResultPair => this.expressionResultPair;

	    public override Type ResultType => typeof(T);

	    public override object ResultAsObject => this.expressionResultPair;

	    public override void Recalculate()
		{
			this.expressionResultPair = (T)((object)this.dynamicExpression.Evaluate());
		}
	}
}
