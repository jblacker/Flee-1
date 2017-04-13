using System;

namespace Ciloci.Flee
{
	internal class GenericExpressionVariable<T> : IVariable, IGenericVariable<T>
	{
		private IGenericExpression<T> MyExpression;

		public object ValueAsObject
		{
			get
			{
				return this.MyExpression;
			}
			set
			{
				this.MyExpression = (IGenericExpression<T>)value;
			}
		}

		public Type VariableType
		{
			get
			{
				return this.MyExpression.Context.Options.ResultType;
			}
		}

		public IVariable Clone()
		{
			return new GenericExpressionVariable<T>
			{
				MyExpression = this.MyExpression
			};
		}

		public T GetValue()
		{
			return this.MyExpression.Evaluate();
		}
	}
}
