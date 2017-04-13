using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class CompareElement : BinaryExpressionElement
	{
		private LogicalCompareOperation MyOperation;

		public void Initialize(ExpressionElement leftChild, ExpressionElement rightChild, LogicalCompareOperation op)
		{
			this.MyLeftChild = leftChild;
			this.MyRightChild = rightChild;
			this.MyOperation = op;
		}

		public void Validate()
		{
			base.ValidateInternal(this.MyOperation);
		}

		protected override void GetOperation(object operation)
		{
			this.MyOperation = (LogicalCompareOperation)operation;
		}

		protected override Type GetResultType(Type leftType, Type rightType)
		{
			Type binaryResultType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
			MethodInfo overloadedOperator = this.GetOverloadedCompareOperator();
			bool isEqualityOp = CompareElement.IsOpTypeEqualOrNotEqual(this.MyOperation);
			bool flag = leftType == typeof(string) & rightType == typeof(string) & isEqualityOp;
			Type GetResultType;
			if (flag)
			{
				GetResultType = typeof(bool);
			}
			else
			{
				bool flag2 = overloadedOperator != null;
				if (flag2)
				{
					GetResultType = overloadedOperator.ReturnType;
				}
				else
				{
					bool flag3 = binaryResultType != null;
					if (flag3)
					{
						GetResultType = typeof(bool);
					}
					else
					{
						bool flag4 = leftType == typeof(bool) & rightType == typeof(bool) & isEqualityOp;
						if (flag4)
						{
							GetResultType = typeof(bool);
						}
						else
						{
							bool flag5 = this.AreBothChildrenReferenceTypes() & isEqualityOp;
							if (flag5)
							{
								GetResultType = typeof(bool);
							}
							else
							{
								bool flag6 = this.AreBothChildrenSameEnum();
								if (flag6)
								{
									GetResultType = typeof(bool);
								}
								else
								{
									GetResultType = null;
								}
							}
						}
					}
				}
			}
			return GetResultType;
		}

		private MethodInfo GetOverloadedCompareOperator()
		{
			string name = CompareElement.GetCompareOperatorName(this.MyOperation);
			return base.GetOverloadedBinaryOperator(name, this.MyOperation);
		}

		private static string GetCompareOperatorName(LogicalCompareOperation op)
		{
			string GetCompareOperatorName;
			switch (op)
			{
			case LogicalCompareOperation.LessThan:
				GetCompareOperatorName = "LessThan";
				break;
			case LogicalCompareOperation.GreaterThan:
				GetCompareOperatorName = "GreaterThan";
				break;
			case LogicalCompareOperation.Equal:
				GetCompareOperatorName = "Equality";
				break;
			case LogicalCompareOperation.NotEqual:
				GetCompareOperatorName = "Inequality";
				break;
			case LogicalCompareOperation.LessThanOrEqual:
				GetCompareOperatorName = "LessThanOrEqual";
				break;
			case LogicalCompareOperation.GreaterThanOrEqual:
				GetCompareOperatorName = "GreaterThanOrEqual";
				break;
			default:
				Debug.Assert(false, "unknown compare type");
				GetCompareOperatorName = null;
				break;
			}
			return GetCompareOperatorName;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			Type binaryResultType = ImplicitConverter.GetBinaryResultType(this.MyLeftChild.ResultType, this.MyRightChild.ResultType);
			MethodInfo overloadedOperator = this.GetOverloadedCompareOperator();
			bool flag = base.AreBothChildrenOfType(typeof(string));
			if (flag)
			{
				this.MyLeftChild.Emit(ilg, services);
				this.MyRightChild.Emit(ilg, services);
				CompareElement.EmitStringEquality(ilg, this.MyOperation, services);
			}
			else
			{
				bool flag2 = overloadedOperator != null;
				if (flag2)
				{
					base.EmitOverloadedOperatorCall(overloadedOperator, ilg, services);
				}
				else
				{
					bool flag3 = binaryResultType != null;
					if (flag3)
					{
						BinaryExpressionElement.EmitChildWithConvert(this.MyLeftChild, binaryResultType, ilg, services);
						BinaryExpressionElement.EmitChildWithConvert(this.MyRightChild, binaryResultType, ilg, services);
						this.EmitCompareOperation(ilg, this.MyOperation);
					}
					else
					{
						bool flag4 = base.AreBothChildrenOfType(typeof(bool));
						if (flag4)
						{
							this.EmitRegular(ilg, services);
						}
						else
						{
							bool flag5 = this.AreBothChildrenReferenceTypes();
							if (flag5)
							{
								this.EmitRegular(ilg, services);
							}
							else
							{
								bool flag6 = this.MyLeftChild.ResultType.IsEnum & this.MyRightChild.ResultType.IsEnum;
								if (flag6)
								{
									this.EmitRegular(ilg, services);
								}
								else
								{
									Debug.Fail("unknown operand types");
								}
							}
						}
					}
				}
			}
		}

		private void EmitRegular(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyLeftChild.Emit(ilg, services);
			this.MyRightChild.Emit(ilg, services);
			this.EmitCompareOperation(ilg, this.MyOperation);
		}

		private static void EmitStringEquality(FleeILGenerator ilg, LogicalCompareOperation op, IServiceProvider services)
		{
			ExpressionOptions options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
			Int32LiteralElement ic = new Int32LiteralElement((int)options.StringComparison);
			ic.Emit(ilg, services);
			MethodInfo mi = typeof(string).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public, null, new Type[]
			{
				typeof(string),
				typeof(string),
				typeof(StringComparison)
			}, null);
			ilg.Emit(OpCodes.Call, mi);
			bool flag = op == LogicalCompareOperation.NotEqual;
			if (flag)
			{
				ilg.Emit(OpCodes.Ldc_I4_0);
				ilg.Emit(OpCodes.Ceq);
			}
		}

		private static bool IsOpTypeEqualOrNotEqual(LogicalCompareOperation op)
		{
			return op == LogicalCompareOperation.Equal | op == LogicalCompareOperation.NotEqual;
		}

		private bool AreBothChildrenReferenceTypes()
		{
			return !this.MyLeftChild.ResultType.IsValueType & !this.MyRightChild.ResultType.IsValueType;
		}

		private bool AreBothChildrenSameEnum()
		{
			return this.MyLeftChild.ResultType.IsEnum && this.MyLeftChild.ResultType == this.MyRightChild.ResultType;
		}

		private void EmitCompareOperation(FleeILGenerator ilg, LogicalCompareOperation op)
		{
			OpCode ltOpcode = this.GetCompareGTLTOpcode(false);
			OpCode gtOpcode = this.GetCompareGTLTOpcode(true);
			switch (op)
			{
			case LogicalCompareOperation.LessThan:
				ilg.Emit(ltOpcode);
				break;
			case LogicalCompareOperation.GreaterThan:
				ilg.Emit(gtOpcode);
				break;
			case LogicalCompareOperation.Equal:
				ilg.Emit(OpCodes.Ceq);
				break;
			case LogicalCompareOperation.NotEqual:
				ilg.Emit(OpCodes.Ceq);
				ilg.Emit(OpCodes.Ldc_I4_0);
				ilg.Emit(OpCodes.Ceq);
				break;
			case LogicalCompareOperation.LessThanOrEqual:
				ilg.Emit(gtOpcode);
				ilg.Emit(OpCodes.Ldc_I4_0);
				ilg.Emit(OpCodes.Ceq);
				break;
			case LogicalCompareOperation.GreaterThanOrEqual:
				ilg.Emit(ltOpcode);
				ilg.Emit(OpCodes.Ldc_I4_0);
				ilg.Emit(OpCodes.Ceq);
				break;
			default:
				Debug.Fail("Unknown op type");
				break;
			}
		}

		private OpCode GetCompareGTLTOpcode(bool greaterThan)
		{
			Type leftType = this.MyLeftChild.ResultType;
			bool flag = leftType == this.MyRightChild.ResultType;
			OpCode GetCompareGTLTOpcode;
			if (flag)
			{
				bool flag2 = leftType == typeof(uint) | leftType == typeof(ulong);
				if (flag2)
				{
					if (greaterThan)
					{
						GetCompareGTLTOpcode = OpCodes.Cgt_Un;
					}
					else
					{
						GetCompareGTLTOpcode = OpCodes.Clt_Un;
					}
				}
				else
				{
					GetCompareGTLTOpcode = CompareElement.GetCompareOpcode(greaterThan);
				}
			}
			else
			{
				GetCompareGTLTOpcode = CompareElement.GetCompareOpcode(greaterThan);
			}
			return GetCompareGTLTOpcode;
		}

		private static OpCode GetCompareOpcode(bool greaterThan)
		{
			OpCode GetCompareOpcode;
			if (greaterThan)
			{
				GetCompareOpcode = OpCodes.Cgt;
			}
			else
			{
				GetCompareOpcode = OpCodes.Clt;
			}
			return GetCompareOpcode;
		}
	}
}
