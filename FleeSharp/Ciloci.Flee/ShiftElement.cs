using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class ShiftElement : BinaryExpressionElement
	{
		private ShiftOperation MyOperation;

		protected override Type GetResultType(Type leftType, Type rightType)
		{
			bool flag = !ImplicitConverter.EmitImplicitNumericConvert(rightType, typeof(int), null);
			Type GetResultType;
			if (flag)
			{
				GetResultType = null;
			}
			else
			{
				bool flag2 = !Utility.IsIntegralType(leftType);
				if (flag2)
				{
					GetResultType = null;
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
						GetResultType = typeof(int);
						break;
					case TypeCode.UInt32:
						GetResultType = typeof(uint);
						break;
					case TypeCode.Int64:
						GetResultType = typeof(long);
						break;
					case TypeCode.UInt64:
						GetResultType = typeof(ulong);
						break;
					default:
						Debug.Assert(false, "unknown left shift operand");
						GetResultType = null;
						break;
					}
				}
			}
			return GetResultType;
		}

		protected override void GetOperation(object operation)
		{
			this.MyOperation = (ShiftOperation)operation;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyLeftChild.Emit(ilg, services);
			this.EmitShiftCount(ilg, services);
			this.EmitShift(ilg);
		}

		private void EmitShiftCount(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyRightChild.Emit(ilg, services);
			TypeCode tc = Type.GetTypeCode(this.MyLeftChild.ResultType);
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

		private void EmitShift(FleeILGenerator ilg)
		{
			OpCode op;
			switch (Type.GetTypeCode(this.MyLeftChild.ResultType))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			{
				bool flag = this.MyOperation == ShiftOperation.LeftShift;
				if (flag)
				{
					op = OpCodes.Shl;
				}
				else
				{
					op = OpCodes.Shr;
				}
				break;
			}
			case TypeCode.UInt32:
			case TypeCode.UInt64:
			{
				bool flag2 = this.MyOperation == ShiftOperation.LeftShift;
				if (flag2)
				{
					op = OpCodes.Shl;
				}
				else
				{
					op = OpCodes.Shr_Un;
				}
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
