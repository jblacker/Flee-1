using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class NotElement : UnaryElement
	{
		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			bool flag = this.MyChild.ResultType == typeof(bool);
			if (flag)
			{
				this.EmitLogical(ilg, services);
			}
			else
			{
				this.MyChild.Emit(ilg, services);
				ilg.Emit(OpCodes.Not);
			}
		}

		private void EmitLogical(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyChild.Emit(ilg, services);
			ilg.Emit(OpCodes.Ldc_I4_0);
			ilg.Emit(OpCodes.Ceq);
		}

		protected override Type GetResultType(Type childType)
		{
			bool flag = childType == typeof(bool);
			Type GetResultType;
			if (flag)
			{
				GetResultType = typeof(bool);
			}
			else
			{
				bool flag2 = Utility.IsIntegralType(childType);
				if (flag2)
				{
					GetResultType = childType;
				}
				else
				{
					GetResultType = null;
				}
			}
			return GetResultType;
		}
	}
}
