using System;

namespace Ciloci.Flee.CalcEngine
{
	internal abstract class ExpressionResultPair
	{
		private string MyName;

		protected IDynamicExpression MyExpression;

		public string Name
		{
			get
			{
				return this.MyName;
			}
		}

		public abstract Type ResultType
		{
			get;
		}

		public abstract object ResultAsObject
		{
			get;
		}

		public IDynamicExpression Expression
		{
			get
			{
				return this.MyExpression;
			}
		}

		public abstract void Recalculate();

		public void SetExpression(IDynamicExpression e)
		{
			this.MyExpression = e;
		}

		public void SetName(string name)
		{
			this.MyName = name;
		}

		public override string ToString()
		{
			return this.MyName;
		}
	}
}
