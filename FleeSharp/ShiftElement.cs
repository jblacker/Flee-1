// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp
{
    using System;
    using System.Diagnostics;
    using System.Reflection.Emit;

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
            this.myOperation = (ShiftOperation) operation;
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