using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	internal class AndOrElement : BinaryExpressionElement
	{
		private AndOrOperation MyOperation;

		private static object OurTrueTerminalKey = RuntimeHelpers.GetObjectValue(new object());

		private static object OurFalseTerminalKey = RuntimeHelpers.GetObjectValue(new object());

		private static object OurEndLabelKey = RuntimeHelpers.GetObjectValue(new object());

		protected override void GetOperation(object operation)
		{
			this.MyOperation = (AndOrOperation)operation;
		}

		protected override Type GetResultType(Type leftType, Type rightType)
		{
			Type bitwiseOpType = Utility.GetBitwiseOpType(leftType, rightType);
			bool flag = bitwiseOpType != null;
			Type GetResultType;
			if (flag)
			{
				GetResultType = bitwiseOpType;
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
			bool flag = resultType == typeof(bool);
			if (flag)
			{
				this.DoEmitLogical(ilg, services);
			}
			else
			{
				this.MyLeftChild.Emit(ilg, services);
				ImplicitConverter.EmitImplicitConvert(this.MyLeftChild.ResultType, resultType, ilg);
				this.MyRightChild.Emit(ilg, services);
				ImplicitConverter.EmitImplicitConvert(this.MyRightChild.ResultType, resultType, ilg);
				AndOrElement.EmitBitwiseOperation(ilg, this.MyOperation);
			}
		}

		private static void EmitBitwiseOperation(FleeILGenerator ilg, AndOrOperation op)
		{
			if (op != AndOrOperation.And)
			{
				if (op != AndOrOperation.Or)
				{
					Debug.Fail("Unknown op type");
				}
				else
				{
					ilg.Emit(OpCodes.Or);
				}
			}
			else
			{
				ilg.Emit(OpCodes.And);
			}
		}

		private void DoEmitLogical(FleeILGenerator ilg, IServiceProvider services)
		{
			ShortCircuitInfo info = new ShortCircuitInfo();
			FleeILGenerator ilgTemp = base.CreateTempFleeILGenerator(ilg);
			Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);
			this.EmitLogical(ilgTemp, info, services);
			info.ClearTempState();
			info.Branches.ComputeBranches();
			Utility.SyncFleeILGeneratorLabels(ilgTemp, ilg);
			this.EmitLogical(ilg, info, services);
		}

		private void EmitLogical(FleeILGenerator ilg, ShortCircuitInfo info, IServiceProvider services)
		{
			info.Branches.GetLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurEndLabelKey), ilg);
			this.PopulateData(info);
			AndOrElement.EmitLogicalShortCircuit(ilg, info, services);
			ExpressionElement terminalOperand = (ExpressionElement)info.Operands.Pop();
			AndOrElement.EmitOperand(terminalOperand, info, ilg, services);
			Label endLabel = info.Branches.FindLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurEndLabelKey));
			ilg.Emit(OpCodes.Br_S, endLabel);
			AndOrElement.EmitTerminals(info, ilg, endLabel);
			ilg.MarkLabel(endLabel);
		}

		private static void EmitLogicalShortCircuit(FleeILGenerator ilg, ShortCircuitInfo info, IServiceProvider services)
		{
			while (info.Operators.Count != 0)
			{
				AndOrElement op = (AndOrElement)info.Operators.Pop();
				ExpressionElement leftOperand = (ExpressionElement)info.Operands.Pop();
				AndOrElement.EmitOperand(leftOperand, info, ilg, services);
				Label i = AndOrElement.GetShortCircuitLabel(op, info, ilg);
				AndOrElement.EmitBranch(op, ilg, i, info);
			}
		}

		private static void EmitBranch(AndOrElement op, FleeILGenerator ilg, Label target, ShortCircuitInfo info)
		{
			bool isTemp = ilg.IsTemp;
			if (isTemp)
			{
				info.Branches.AddBranch(ilg, target);
				OpCode shortBranch = AndOrElement.GetBranchOpcode(op, false);
				ilg.Emit(shortBranch, target);
			}
			else
			{
				bool longBranch = info.Branches.IsLongBranch(ilg, target);
				OpCode brOpcode = AndOrElement.GetBranchOpcode(op, longBranch);
				ilg.Emit(brOpcode, target);
			}
		}

		private static OpCode GetBranchOpcode(AndOrElement op, bool longBranch)
		{
			bool flag = op.MyOperation == AndOrOperation.And;
			OpCode GetBranchOpcode;
			if (flag)
			{
				if (longBranch)
				{
					GetBranchOpcode = OpCodes.Brfalse;
				}
				else
				{
					GetBranchOpcode = OpCodes.Brfalse_S;
				}
			}
			else if (longBranch)
			{
				GetBranchOpcode = OpCodes.Brtrue;
			}
			else
			{
				GetBranchOpcode = OpCodes.Brtrue_S;
			}
			return GetBranchOpcode;
		}

		private static Label GetShortCircuitLabel(AndOrElement current, ShortCircuitInfo info, FleeILGenerator ilg)
		{
			Stack cloneOperands = (Stack)info.Operands.Clone();
			Stack cloneOperators = (Stack)info.Operators.Clone();
			current.PopRightChild(cloneOperands, cloneOperators);
			Label GetShortCircuitLabel;
			while (cloneOperators.Count > 0)
			{
				AndOrElement top = (AndOrElement)cloneOperators.Pop();
				bool flag = top.MyOperation != current.MyOperation;
				if (flag)
				{
					object nextOperand = RuntimeHelpers.GetObjectValue(cloneOperands.Pop());
					GetShortCircuitLabel = AndOrElement.GetLabel(RuntimeHelpers.GetObjectValue(nextOperand), ilg, info);
					return GetShortCircuitLabel;
				}
				top.PopRightChild(cloneOperands, cloneOperators);
			}
			bool flag2 = current.MyOperation == AndOrOperation.And;
			if (flag2)
			{
				GetShortCircuitLabel = AndOrElement.GetLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurFalseTerminalKey), ilg, info);
				return GetShortCircuitLabel;
			}
			GetShortCircuitLabel = AndOrElement.GetLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurTrueTerminalKey), ilg, info);
			return GetShortCircuitLabel;
		}

		private void PopRightChild(Stack operands, Stack operators)
		{
			AndOrElement andOrChild = this.MyRightChild as AndOrElement;
			bool flag = andOrChild != null;
			if (flag)
			{
				andOrChild.Pop(operands, operators);
			}
			else
			{
				operands.Pop();
			}
		}

		private void Pop(Stack operands, Stack operators)
		{
			operators.Pop();
			AndOrElement andOrChild = this.MyLeftChild as AndOrElement;
			bool flag = andOrChild == null;
			if (flag)
			{
				operands.Pop();
			}
			else
			{
				andOrChild.Pop(operands, operators);
			}
			andOrChild = (this.MyRightChild as AndOrElement);
			bool flag2 = andOrChild == null;
			if (flag2)
			{
				operands.Pop();
			}
			else
			{
				andOrChild.Pop(operands, operators);
			}
		}

		private static void EmitOperand(ExpressionElement operand, ShortCircuitInfo info, FleeILGenerator ilg, IServiceProvider services)
		{
			bool flag = info.Branches.HasLabel(operand);
			if (flag)
			{
				Label leftLabel = info.Branches.FindLabel(operand);
				ilg.MarkLabel(leftLabel);
				AndOrElement.MarkBranchTarget(info, leftLabel, ilg);
			}
			operand.Emit(ilg, services);
		}

		private static void EmitTerminals(ShortCircuitInfo info, FleeILGenerator ilg, Label endLabel)
		{
			bool flag = info.Branches.HasLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurFalseTerminalKey));
			if (flag)
			{
				Label falseLabel = info.Branches.FindLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurFalseTerminalKey));
				ilg.MarkLabel(falseLabel);
				AndOrElement.MarkBranchTarget(info, falseLabel, ilg);
				ilg.Emit(OpCodes.Ldc_I4_0);
				bool flag2 = info.Branches.HasLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurTrueTerminalKey));
				if (flag2)
				{
					ilg.Emit(OpCodes.Br_S, endLabel);
				}
			}
			bool flag3 = info.Branches.HasLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurTrueTerminalKey));
			if (flag3)
			{
				Label trueLabel = info.Branches.FindLabel(RuntimeHelpers.GetObjectValue(AndOrElement.OurTrueTerminalKey));
				ilg.MarkLabel(trueLabel);
				AndOrElement.MarkBranchTarget(info, trueLabel, ilg);
				ilg.Emit(OpCodes.Ldc_I4_1);
			}
		}

		private static void MarkBranchTarget(ShortCircuitInfo info, Label target, FleeILGenerator ilg)
		{
			bool isTemp = ilg.IsTemp;
			if (isTemp)
			{
				info.Branches.MarkLabel(ilg, target);
			}
		}

		private static Label GetLabel(object key, FleeILGenerator ilg, ShortCircuitInfo info)
		{
			return info.Branches.GetLabel(RuntimeHelpers.GetObjectValue(key), ilg);
		}

		private void PopulateData(ShortCircuitInfo info)
		{
			AndOrElement andOrChild = this.MyRightChild as AndOrElement;
			bool flag = andOrChild == null;
			if (flag)
			{
				info.Operands.Push(this.MyRightChild);
			}
			else
			{
				andOrChild.PopulateData(info);
			}
			info.Operators.Push(this);
			andOrChild = (this.MyLeftChild as AndOrElement);
			bool flag2 = andOrChild == null;
			if (flag2)
			{
				info.Operands.Push(this.MyLeftChild);
			}
			else
			{
				andOrChild.PopulateData(info);
			}
		}
	}
}
