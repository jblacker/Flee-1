using System;

namespace Ciloci.Flee
{
	internal class LocalBasedElement : ExpressionElement
	{
		private int MyIndex;

		private ExpressionElement MyTarget;

		public override Type ResultType
		{
			get
			{
				return this.MyTarget.ResultType;
			}
		}

		public LocalBasedElement(ExpressionElement target, int index)
		{
			this.MyTarget = target;
			this.MyIndex = index;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			Utility.EmitLoadLocal(ilg, this.MyIndex);
		}
	}
}
