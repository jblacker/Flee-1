using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Flee
{
    internal class ShiftElement : BinaryExpressionElement
    {
        private ShiftOperation myOperation;

        protected override Type GetResultType(Type leftType, Type rightType)
        {
            var flag = !ImplicitConverter.EmitImplicitNumericConvert(rightType, typeof(int), null);
            Type getResultType = null;
            if (!flag)
            {
                var flag2 = !Utility.IsIntegralType(leftType);
                if (flag2)
                {
                    getResultType = null;
                }
                else
                {
                    switch (Type.GetTypeCode(leftType))
                    {
                        case TypeCode.SByte:
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                            getResultType = typeof(int);
                            break;
                        case TypeCode.UInt32:
                            getResultType = typeof(uint);
                            break;
                        case TypeCode.Int64:
                            getResultType = typeof(long);
                            break;
                        case TypeCode.UInt64:
                            getResultType = typeof(ulong);
                            break;
                        default:
                            Debug.Assert(false, "unknown left shift operand");
                            break;
                    }
                }
            }
            return getResultType;
        }

        protected override void GetOperation(object operation)
        {
            this.myOperation = (ShiftOperation)operation;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myLeftChild.Emit(ilg, services);
            this.EmitShiftCount(ilg, services);
            this.EmitShift(ilg);
        }

        private void EmitShiftCount(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myRightChild.Emit(ilg, services);
            var tc = Type.GetTypeCode(this.myLeftChild.ResultType);
            if (tc - TypeCode.SByte > 5)
            {
                if (tc - TypeCode.Int64 > 1)
                {
                    Debug.Assert(false, "unknown left shift operand");
                }
                else
                {
                    ilg.Emit(OpCodes.Ldc_I4_S, 63);
                }
            }
            else
            {
                ilg.Emit(OpCodes.Ldc_I4_S, 31);
            }
            ilg.Emit(OpCodes.And);
        }

        private void EmitShift(FleeIlGenerator ilg)
        {
            var op = new OpCode();
            switch (Type.GetTypeCode(this.myLeftChild.ResultType))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                {
                    var flag = this.myOperation == ShiftOperation.LeftShift;
                    op = flag ? OpCodes.Shl : OpCodes.Shr;
                    break;
                }
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                {
                    var flag2 = this.myOperation == ShiftOperation.LeftShift;
                    op = flag2 ? OpCodes.Shl : OpCodes.Shr_Un;
                    break;
                }
                default:
                    Debug.Assert(false, "unknown left shift operand");
                    break;
            }
            ilg.Emit(op);
        }
    }
}