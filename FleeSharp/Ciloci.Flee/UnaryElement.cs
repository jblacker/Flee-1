using System;

namespace Ciloci.Flee
{
	internal abstract class UnaryElement : ExpressionElement
	{
		protected ExpressionElement MyChild;

		private Type MyResultType;

		public override Type ResultType
		{
			get
			{
				return this.MyResultType;
			}
		}

		public void SetChild(ExpressionElement child)
		{
			this.MyChild = child;
			this.MyResultType = this.GetResultType(child.ResultType);
			bool flag = this.MyResultType == null;
			if (flag)
			{
				base.ThrowCompileException("OperationNotDefinedForType", CompileExceptionReason.TypeMismatch, new object[]
				{
					this.MyChild.ResultType.Name
				});
			}
		}

		protected abstract Type GetResultType(Type childType);
	}
}
