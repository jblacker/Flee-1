using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class XorElement : BinaryExpressionElement
	{
		protected override Type GetResultType(Type leftType, Type rightType)
		{
			Type bitwiseType = Utility.GetBitwiseOpType(leftType, rightType);
			bool flag = bitwiseType != null;
			Type GetResultType;
			if (flag)
			{
				GetResultType = bitwiseType;
			}
			else
			{
				bool flag2 = base.AreBothChildrenOfType(typeof(bool));
				if (flag2)
				{
					GetResultType = typeof(bool);
				}
				else
				{
					GetResultType = null;
				}
			}
			return GetResultType;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			Type resultType = base.ResultType;
			this.MyLeftChild.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyLeftChild.ResultType, resultType, ilg);
			this.MyRightChild.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyRightChild.ResultType, resultType, ilg);
			ilg.Emit(OpCodes.Xor);
		}

		protected override void GetOperation(object operation)
		{
		}
	}
}
