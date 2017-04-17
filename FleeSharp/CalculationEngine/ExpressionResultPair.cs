namespace Flee.CalculationEngine
{
    using System;

    internal abstract class ExpressionResultPair
	{
		private string myName;

		protected IDynamicExpression dynamicExpression;

		public string Name => this.myName;

	    public abstract Type ResultType
		{
			get;
		}

		public abstract object ResultAsObject
		{
			get;
		}

		public IDynamicExpression Expression => this.dynamicExpression;

	    public abstract void Recalculate();

		public void SetExpression(IDynamicExpression e)
		{
			this.dynamicExpression = e;
		}

		public void SetName(string name)
		{
			this.myName = name;
		}

		public override string ToString()
		{
			return this.myName;
		}
	}
}
