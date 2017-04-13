using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class ArithmeticElement : BinaryExpressionElement
	{
		private static MethodInfo OurPowerMethodInfo;

		private static MethodInfo OurStringConcatMethodInfo;

		private static MethodInfo OurObjectConcatMethodInfo;

		private BinaryArithmeticOperation MyOperation;

		private bool IsOptimizablePower
		{
			get
			{
				bool flag = this.MyOperation != BinaryArithmeticOperation.Power;
				bool IsOptimizablePower;
				if (flag)
				{
					IsOptimizablePower = false;
				}
				else
				{
					Int32LiteralElement right = this.MyRightChild as Int32LiteralElement;
					bool flag2 = right == null;
					IsOptimizablePower = (!flag2 && right.Value >= 0);
				}
				return IsOptimizablePower;
			}
		}

		static ArithmeticElement()
		{
			ArithmeticElement.OurPowerMethodInfo = typeof(Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
			ArithmeticElement.OurStringConcatMethodInfo = typeof(string).GetMethod("Concat", BindingFlags.Static | BindingFlags.Public, null, new Type[]
			{
				typeof(string),
				typeof(string)
			}, null);
			ArithmeticElement.OurObjectConcatMethodInfo = typeof(string).GetMethod("Concat", BindingFlags.Static | BindingFlags.Public, null, new Type[]
			{
				typeof(object),
				typeof(object)
			}, null);
		}

		protected override void GetOperation(object operation)
		{
			this.MyOperation = (BinaryArithmeticOperation)operation;
		}

		protected override Type GetResultType(Type leftType, Type rightType)
		{
			Type binaryResultType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
			MethodInfo overloadedMethod = this.GetOverloadedArithmeticOperator();
			bool flag = overloadedMethod != null;
			Type GetResultType;
			if (flag)
			{
				GetResultType = overloadedMethod.ReturnType;
			}
			else
			{
				bool flag2 = binaryResultType != null;
				if (flag2)
				{
					bool flag3 = this.MyOperation == BinaryArithmeticOperation.Power;
					if (flag3)
					{
						GetResultType = this.GetPowerResultType(leftType, rightType, binaryResultType);
					}
					else
					{
						GetResultType = binaryResultType;
					}
				}
				else
				{
					bool flag4 = base.IsEitherChildOfType(typeof(string)) & this.MyOperation == BinaryArithmeticOperation.Add;
					if (flag4)
					{
						GetResultType = typeof(string);
					}
					else
					{
						GetResultType = null;
					}
				}
			}
			return GetResultType;
		}

		private Type GetPowerResultType(Type leftType, Type rightType, Type binaryResultType)
		{
			bool isOptimizablePower = this.IsOptimizablePower;
			Type GetPowerResultType;
			if (isOptimizablePower)
			{
				GetPowerResultType = leftType;
			}
			else
			{
				GetPowerResultType = typeof(double);
			}
			return GetPowerResultType;
		}

		private MethodInfo GetOverloadedArithmeticOperator()
		{
			string name = ArithmeticElement.GetOverloadedOperatorFunctionName(this.MyOperation);
			return base.GetOverloadedBinaryOperator(name, this.MyOperation);
		}

		private static string GetOverloadedOperatorFunctionName(BinaryArithmeticOperation op)
		{
			string GetOverloadedOperatorFunctionName;
			switch (op)
			{
			case BinaryArithmeticOperation.Add:
				GetOverloadedOperatorFunctionName = "Addition";
				break;
			case BinaryArithmeticOperation.Subtract:
				GetOverloadedOperatorFunctionName = "Subtraction";
				break;
			case BinaryArithmeticOperation.Multiply:
				GetOverloadedOperatorFunctionName = "Multiply";
				break;
			case BinaryArithmeticOperation.Divide:
				GetOverloadedOperatorFunctionName = "Division";
				break;
			case BinaryArithmeticOperation.Mod:
				GetOverloadedOperatorFunctionName = "Modulus";
				break;
			case BinaryArithmeticOperation.Power:
				GetOverloadedOperatorFunctionName = "Exponent";
				break;
			default:
				Debug.Assert(false, "unknown operator type");
				GetOverloadedOperatorFunctionName = null;
				break;
			}
			return GetOverloadedOperatorFunctionName;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			MethodInfo overloadedMethod = this.GetOverloadedArithmeticOperator();
			bool flag = overloadedMethod != null;
			if (flag)
			{
				base.EmitOverloadedOperatorCall(overloadedMethod, ilg, services);
			}
			else
			{
				bool flag2 = base.IsEitherChildOfType(typeof(string));
				if (flag2)
				{
					this.EmitStringConcat(ilg, services);
				}
				else
				{
					this.EmitArithmeticOperation(this.MyOperation, ilg, services);
				}
			}
		}

		private static bool IsUnsignedForArithmetic(Type t)
		{
			return t == typeof(uint) | t == typeof(ulong);
		}

		private void EmitArithmeticOperation(BinaryArithmeticOperation op, FleeILGenerator ilg, IServiceProvider services)
		{
			ExpressionOptions options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
			bool unsigned = ArithmeticElement.IsUnsignedForArithmetic(this.MyLeftChild.ResultType) & ArithmeticElement.IsUnsignedForArithmetic(this.MyRightChild.ResultType);
			bool integral = Utility.IsIntegralType(this.MyLeftChild.ResultType) & Utility.IsIntegralType(this.MyRightChild.ResultType);
			bool emitOverflow = integral & options.Checked;
			BinaryExpressionElement.EmitChildWithConvert(this.MyLeftChild, base.ResultType, ilg, services);
			bool flag = !this.IsOptimizablePower;
			if (flag)
			{
				BinaryExpressionElement.EmitChildWithConvert(this.MyRightChild, base.ResultType, ilg, services);
			}
			switch (op)
			{
			case BinaryArithmeticOperation.Add:
			{
				bool flag2 = emitOverflow;
				if (flag2)
				{
					bool flag3 = unsigned;
					if (flag3)
					{
						ilg.Emit(OpCodes.Add_Ovf_Un);
					}
					else
					{
						ilg.Emit(OpCodes.Add_Ovf);
					}
				}
				else
				{
					ilg.Emit(OpCodes.Add);
				}
				break;
			}
			case BinaryArithmeticOperation.Subtract:
			{
				bool flag4 = emitOverflow;
				if (flag4)
				{
					bool flag5 = unsigned;
					if (flag5)
					{
						ilg.Emit(OpCodes.Sub_Ovf_Un);
					}
					else
					{
						ilg.Emit(OpCodes.Sub_Ovf);
					}
				}
				else
				{
					ilg.Emit(OpCodes.Sub);
				}
				break;
			}
			case BinaryArithmeticOperation.Multiply:
				this.EmitMultiply(ilg, emitOverflow, unsigned);
				break;
			case BinaryArithmeticOperation.Divide:
			{
				bool flag6 = unsigned;
				if (flag6)
				{
					ilg.Emit(OpCodes.Div_Un);
				}
				else
				{
					ilg.Emit(OpCodes.Div);
				}
				break;
			}
			case BinaryArithmeticOperation.Mod:
			{
				bool flag7 = unsigned;
				if (flag7)
				{
					ilg.Emit(OpCodes.Rem_Un);
				}
				else
				{
					ilg.Emit(OpCodes.Rem);
				}
				break;
			}
			case BinaryArithmeticOperation.Power:
				this.EmitPower(ilg, emitOverflow, unsigned);
				break;
			default:
				Debug.Fail("Unknown op type");
				break;
			}
		}

		private void EmitPower(FleeILGenerator ilg, bool emitOverflow, bool unsigned)
		{
			bool isOptimizablePower = this.IsOptimizablePower;
			if (isOptimizablePower)
			{
				this.EmitOptimizedPower(ilg, emitOverflow, unsigned);
			}
			else
			{
				ilg.Emit(OpCodes.Call, ArithmeticElement.OurPowerMethodInfo);
			}
		}

		private void EmitOptimizedPower(FleeILGenerator ilg, bool emitOverflow, bool unsigned)
		{
			Int32LiteralElement right = (Int32LiteralElement)this.MyRightChild;
			bool flag = right.Value == 0;
			if (flag)
			{
				ilg.Emit(OpCodes.Pop);
				LiteralElement.EmitLoad(1, ilg);
				ImplicitConverter.EmitImplicitNumericConvert(typeof(int), this.MyLeftChild.ResultType, ilg);
			}
			else
			{
				bool flag2 = right.Value == 1;
				if (!flag2)
				{
					int num = right.Value - 1;
					for (int i = 1; i <= num; i++)
					{
						ilg.Emit(OpCodes.Dup);
					}
					int num2 = right.Value - 1;
					for (int j = 1; j <= num2; j++)
					{
						this.EmitMultiply(ilg, emitOverflow, unsigned);
					}
				}
			}
		}

		private void EmitMultiply(FleeILGenerator ilg, bool emitOverflow, bool unsigned)
		{
			if (emitOverflow)
			{
				if (unsigned)
				{
					ilg.Emit(OpCodes.Mul_Ovf_Un);
				}
				else
				{
					ilg.Emit(OpCodes.Mul_Ovf);
				}
			}
			else
			{
				ilg.Emit(OpCodes.Mul);
			}
		}

		private void EmitStringConcat(FleeILGenerator ilg, IServiceProvider services)
		{
			bool flag = base.AreBothChildrenOfType(typeof(string));
			MethodInfo concatMethodInfo;
			Type argType;
			if (flag)
			{
				concatMethodInfo = ArithmeticElement.OurStringConcatMethodInfo;
				argType = typeof(string);
			}
			else
			{
				Debug.Assert(base.IsEitherChildOfType(typeof(string)), "one child must be a string");
				concatMethodInfo = ArithmeticElement.OurObjectConcatMethodInfo;
				argType = typeof(object);
			}
			this.MyLeftChild.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyLeftChild.ResultType, argType, ilg);
			this.MyRightChild.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyRightChild.ResultType, argType, ilg);
			ilg.Emit(OpCodes.Call, concatMethodInfo);
		}
	}
}
