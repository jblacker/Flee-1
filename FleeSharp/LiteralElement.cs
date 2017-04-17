using System.Diagnostics;
using System.Reflection.Emit;

namespace Flee
{
    using System;

    internal abstract class LiteralElement : ExpressionElement
    {
        protected void OnParseOverflow(string image)
        {
            this.ThrowCompileException("ValueNotRepresentableInType", CompileExceptionReason.ConstantOverflow, new object[]
            {
                image,
                this.ResultType.Name
            });
        }

        public static void EmitLoad(int value, FleeIlGenerator ilg)
        {
            if (value >= -1 & value <= 8)
            {
                EmitSuperShort(value, ilg);
            }
            else if (value >= sbyte.MinValue & value <= sbyte.MaxValue)
            {
                ilg.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(value));
            }
            else
            {
                ilg.Emit(OpCodes.Ldc_I4, value);
            }
        }

        protected static void EmitLoad(long value, FleeIlGenerator ilg)
        {
            if (value >= int.MinValue & value <= int.MaxValue)
            {
                EmitLoad(Convert.ToInt32(value), ilg);
                ilg.Emit(OpCodes.Conv_I8);
            }
            else if (value >= 0 & value <= uint.MaxValue)
            {
                EmitLoad(Convert.ToInt32(value), ilg);
                ilg.Emit(OpCodes.Conv_U8);
            }
            else
            {
                ilg.Emit(OpCodes.Ldc_I8, value);
            }
        }

        protected static void EmitLoad(bool value, FleeIlGenerator ilg)
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

        private static void EmitSuperShort(int value, FleeIlGenerator ilg)
        {
            OpCode ldcOpcode = default(OpCode);
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