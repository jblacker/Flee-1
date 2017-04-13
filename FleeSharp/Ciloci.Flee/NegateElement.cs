using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class NegateElement : UnaryElement
	{
		protected override Type GetResultType(Type childType)
		{
			TypeCode tc = Type.GetTypeCode(childType);
			MethodInfo mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", childType, childType);
			bool flag = mi != null;
			Type GetResultType;
			if (flag)
			{
				GetResultType = mi.ReturnType;
			}
			else
			{
				switch (tc)
				{
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					GetResultType = childType;
					return GetResultType;
				case TypeCode.UInt32:
					GetResultType = typeof(long);
					return GetResultType;
				}
				GetResultType = null;
			}
			return GetResultType;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			Type resultType = this.ResultType;
			this.MyChild.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyChild.ResultType, resultType, ilg);
			MethodInfo mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", resultType, resultType);
			bool flag = mi == null;
			if (flag)
			{
				ilg.Emit(OpCodes.Neg);
			}
			else
			{
				ilg.Emit(OpCodes.Call, mi);
			}
		}
	}
}
