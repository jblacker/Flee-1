using System;

namespace Ciloci.Flee
{
	internal class DynamicExpressionVariable<T> : IVariable, IGenericVariable<T>
	{
		private IDynamicExpression MyExpression;

		public object ValueAsObject
		{
			get
			{
				return this.MyExpression;
			}
			set
			{
				this.MyExpression = (IDynamicExpression)value;
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
			return new DynamicExpressionVariable<T>
			{
				MyExpression = this.MyExpression
			};
		}

		public T GetValue()
		{
			return (T)((object)this.MyExpression.Evaluate());
		}
	}
}
