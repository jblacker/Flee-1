using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal abstract class LiteralElement : ExpressionElement
	{
		protected void OnParseOverflow(string image)
		{
			base.ThrowCompileException("ValueNotRepresentableInType", CompileExceptionReason.ConstantOverflow, new object[]
			{
				image,
				this.ResultType.Name
			});
		}

		public static void EmitLoad(int value, FleeILGenerator ilg)
		{
			bool flag = value >= -1 & value <= 8;
			if (flag)
			{
				LiteralElement.EmitSuperShort(value, ilg);
			}
			else
			{
				bool flag2 = value >= -128 & value <= 127;
				if (flag2)
				{
					ilg.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
				}
				else
				{
					ilg.Emit(OpCodes.Ldc_I4, value);
				}
			}
		}

		protected static void EmitLoad(long value, FleeILGenerator ilg)
		{
			bool flag = value >= -2147483648L & value <= 2147483647L;
			if (flag)
			{
				LiteralElement.EmitLoad((int)value, ilg);
				ilg.Emit(OpCodes.Conv_I8);
			}
			else
			{
				bool flag2 = value >= 0L & value <= (long)((ulong)-1);
				if (flag2)
				{
					LiteralElement.EmitLoad((int)value, ilg);
					ilg.Emit(OpCodes.Conv_U8);
				}
				else
				{
					ilg.Emit(OpCodes.Ldc_I8, value);
				}
			}
		}

		protected static void EmitLoad(bool value, FleeILGenerator ilg)
		{
			if (value)
			{
				ilg.Emit(OpCodes.Ldc_I4_1);
			}
			else
			{
				ilg.Emit(OpCodes.Ldc_I4_0);
			}
		}

		private static void EmitSuperShort(int value, FleeILGenerator ilg)
		{
			OpCode ldcOpcode;
			switch (value)
			{
			case -1:
				ldcOpcode = OpCodes.Ldc_I4_M1;
				break;
			case 0:
				ldcOpcode = OpCodes.Ldc_I4_0;
				break;
			case 1:
				ldcOpcode = OpCodes.Ldc_I4_1;
				break;
			case 2:
				ldcOpcode = OpCodes.Ldc_I4_2;
				break;
			case 3:
				ldcOpcode = OpCodes.Ldc_I4_3;
				break;
			case 4:
				ldcOpcode = OpCodes.Ldc_I4_4;
				break;
			case 5:
				ldcOpcode = OpCodes.Ldc_I4_5;
				break;
			case 6:
				ldcOpcode = OpCodes.Ldc_I4_6;
				break;
			case 7:
				ldcOpcode = OpCodes.Ldc_I4_7;
				break;
			case 8:
				ldcOpcode = OpCodes.Ldc_I4_8;
				break;
			default:
				Debug.Assert(false, "value out of range");
				break;
			}
			ilg.Emit(ldcOpcode);
		}
	}
}
