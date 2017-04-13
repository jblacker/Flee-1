using System;

namespace Ciloci.Flee.CalcEngine
{
	internal class GenericExpressionResultPair<T> : ExpressionResultPair
	{
		public T MyResult;

		public T Result
		{
			get
			{
				return this.MyResult;
			}
		}

		public override Type ResultType
		{
			get
			{
				return typeof(T);
			}
		}

		public override object ResultAsObject
		{
			get
			{
				return this.MyResult;
			}
		}

		public override void Recalculate()
		{
			this.MyResult = (T)((object)this.MyExpression.Evaluate());
		}
	}
}
