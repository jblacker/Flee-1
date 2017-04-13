using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class RootExpressionElement : ExpressionElement
	{
		private ExpressionElement MyChild;

		private Type MyResultType;

		public override Type ResultType
		{
			get
			{
				return typeof(object);
			}
		}

		public RootExpressionElement(ExpressionElement child, Type resultType)
		{
			this.MyChild = child;
			this.MyResultType = resultType;
			this.Validate();
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyChild.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyChild.ResultType, this.MyResultType, ilg);
			ExpressionOptions options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
			bool flag = !options.IsGeneric;
			if (flag)
			{
				ImplicitConverter.EmitImplicitConvert(this.MyResultType, typeof(object), ilg);
			}
			ilg.Emit(OpCodes.Ret);
		}

		private void Validate()
		{
			bool flag = !ImplicitConverter.EmitImplicitConvert(this.MyChild.ResultType, this.MyResultType, null);
			if (flag)
			{
				base.ThrowCompileException("CannotConvertTypeToExpressionResult", CompileExceptionReason.TypeMismatch, new object[]
				{
					this.MyChild.ResultType.Name,
					this.MyResultType.Name
				});
			}
		}
	}
}
