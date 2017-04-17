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

namespace Flee
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    internal class AndOrElement : BinaryExpressionElement
    {
        private static readonly object ourTrueTerminalKey = RuntimeHelpers.GetObjectValue(new object());
        private static readonly object ourFalseTerminalKey = RuntimeHelpers.GetObjectValue(new object());

        private static readonly object ourEndLabelKey = RuntimeHelpers.GetObjectValue(new object());
        private AndOrOperation myOperation;

        protected override void GetOperation(object operation)
        {
            this.myOperation = (AndOrOperation) operation;
        }

        protected override Type GetResultType(Type leftType, Type rightType)
        {
            var bitwiseOpType = Utility.GetBitwiseOpType(leftType, rightType);
            var flag = bitwiseOpType != null;
            Type getResultType;
            if (flag)
            {
                getResultType = bitwiseOpType;
            }
            else
            {
                var flag2 = this.AreBothChildrenOfType(typeof(bool));
                getResultType = flag2 ? typeof(bool) : null;
            }
            return getResultType;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var resultType = this.ResultType;
            var flag = resultType == typeof(bool);
            if (flag)
            {
                this.DoEmitLogical(ilg, services);
            }
            else
            {
                this.myLeftChild.Emit(ilg, services);
                ImplicitConverter.EmitImplicitConvert(this.myLeftChild.ResultType, resultType, ilg);
                this.myRightChild.Emit(ilg, services);
                ImplicitConverter.EmitImplicitConvert(this.myRightChild.ResultType, resultType, ilg);
                EmitBitwiseOperation(ilg, this.myOperation);
            }
        }

        private static void EmitBitwiseOperation(FleeIlGenerator ilg, AndOrOperation op)
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

        private void DoEmitLogical(FleeIlGenerator ilg, IServiceProvider services)
        {
            var info = new ShortCircuitInfo();
            var ilgTemp = this.CreateTempFleeIlGenerator(ilg);
            Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);
            this.EmitLogical(ilgTemp, info, services);
            info.ClearTempState();
            info.Branches.ComputeBranches();
            Utility.SyncFleeILGeneratorLabels(ilgTemp, ilg);
            this.EmitLogical(ilg, info, services);
        }

        private void EmitLogical(FleeIlGenerator ilg, ShortCircuitInfo info, IServiceProvider services)
        {
            info.Branches.GetLabel(RuntimeHelpers.GetObjectValue(ourEndLabelKey), ilg);
            this.PopulateData(info);
            EmitLogicalShortCircuit(ilg, info, services);
            var terminalOperand = (ExpressionElement) info.Operands.Pop();
            EmitOperand(terminalOperand, info, ilg, services);
            var endLabel = info.Branches.FindLabel(RuntimeHelpers.GetObjectValue(ourEndLabelKey));
            ilg.Emit(OpCodes.Br_S, endLabel);
            EmitTerminals(info, ilg, endLabel);
            ilg.MarkLabel(endLabel);
        }

        private static void EmitLogicalShortCircuit(FleeIlGenerator ilg, ShortCircuitInfo info, IServiceProvider services)
        {
            while (info.Operators.Count != 0)
            {
                var op = (AndOrElement) info.Operators.Pop();
                var leftOperand = (ExpressionElement) info.Operands.Pop();
                EmitOperand(leftOperand, info, ilg, services);
                var i = GetShortCircuitLabel(op, info, ilg);
                EmitBranch(op, ilg, i, info);
            }
        }

        private static void EmitBranch(AndOrElement op, FleeIlGenerator ilg, Label target, ShortCircuitInfo info)
        {
            var isTemp = ilg.IsTemp;
            if (isTemp)
            {
                info.Branches.AddBranch(ilg, target);
                var shortBranch = GetBranchOpcode(op, false);
                ilg.Emit(shortBranch, target);
            }
            else
            {
                var longBranch = info.Branches.IsLongBranch(ilg, target);
                var brOpcode = GetBranchOpcode(op, longBranch);
                ilg.Emit(brOpcode, target);
            }
        }

        private static OpCode GetBranchOpcode(AndOrElement op, bool longBranch)
        {
            var flag = op.myOperation == AndOrOperation.And;
            OpCode getBranchOpcode;
            if (flag)
            {
                getBranchOpcode = longBranch ? OpCodes.Brfalse : OpCodes.Brfalse_S;
            }
            else if (longBranch)
            {
                getBranchOpcode = OpCodes.Brtrue;
            }
            else
            {
                getBranchOpcode = OpCodes.Brtrue_S;
            }
            return getBranchOpcode;
        }

        private static Label GetShortCircuitLabel(AndOrElement current, ShortCircuitInfo info, FleeIlGenerator ilg)
        {
            var cloneOperands = (Stack) info.Operands.Clone();
            var cloneOperators = (Stack) info.Operators.Clone();
            current.PopRightChild(cloneOperands, cloneOperators);
            Label getShortCircuitLabel;
            while (cloneOperators.Count > 0)
            {
                var top = (AndOrElement) cloneOperators.Pop();
                var flag = top.myOperation != current.myOperation;
                if (flag)
                {
                    var nextOperand = RuntimeHelpers.GetObjectValue(cloneOperands.Pop());
                    getShortCircuitLabel = GetLabel(RuntimeHelpers.GetObjectValue(nextOperand), ilg, info);
                    return getShortCircuitLabel;
                }
                top.PopRightChild(cloneOperands, cloneOperators);
            }
            var flag2 = current.myOperation == AndOrOperation.And;
            if (flag2)
            {
                getShortCircuitLabel = GetLabel(RuntimeHelpers.GetObjectValue(ourFalseTerminalKey), ilg, info);
                return getShortCircuitLabel;
            }
            getShortCircuitLabel = GetLabel(RuntimeHelpers.GetObjectValue(ourTrueTerminalKey), ilg, info);
            return getShortCircuitLabel;
        }

        private void PopRightChild(Stack operands, Stack operators)
        {
            var andOrChild = this.myRightChild as AndOrElement;
            var flag = andOrChild != null;
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
            var andOrChild = this.myLeftChild as AndOrElement;
            var flag = andOrChild == null;
            if (flag)
            {
                operands.Pop();
            }
            else
            {
                andOrChild.Pop(operands, operators);
            }
            andOrChild = this.myRightChild as AndOrElement;
            var flag2 = andOrChild == null;
            if (flag2)
            {
                operands.Pop();
            }
            else
            {
                andOrChild.Pop(operands, operators);
            }
        }

        private static void EmitOperand(ExpressionElement operand, ShortCircuitInfo info, FleeIlGenerator ilg,
            IServiceProvider services)
        {
            var flag = info.Branches.HasLabel(operand);
            if (flag)
            {
                var leftLabel = info.Branches.FindLabel(operand);
                ilg.MarkLabel(leftLabel);
                MarkBranchTarget(info, leftLabel, ilg);
            }
            operand.Emit(ilg, services);
        }

        private static void EmitTerminals(ShortCircuitInfo info, FleeIlGenerator ilg, Label endLabel)
        {
            var flag = info.Branches.HasLabel(RuntimeHelpers.GetObjectValue(ourFalseTerminalKey));
            if (flag)
            {
                var falseLabel = info.Branches.FindLabel(RuntimeHelpers.GetObjectValue(ourFalseTerminalKey));
                ilg.MarkLabel(falseLabel);
                MarkBranchTarget(info, falseLabel, ilg);
                ilg.Emit(OpCodes.Ldc_I4_0);
                var flag2 = info.Branches.HasLabel(RuntimeHelpers.GetObjectValue(ourTrueTerminalKey));
                if (flag2)
                {
                    ilg.Emit(OpCodes.Br_S, endLabel);
                }
            }
            var flag3 = info.Branches.HasLabel(RuntimeHelpers.GetObjectValue(ourTrueTerminalKey));
            if (flag3)
            {
                var trueLabel = info.Branches.FindLabel(RuntimeHelpers.GetObjectValue(ourTrueTerminalKey));
                ilg.MarkLabel(trueLabel);
                MarkBranchTarget(info, trueLabel, ilg);
                ilg.Emit(OpCodes.Ldc_I4_1);
            }
        }

        private static void MarkBranchTarget(ShortCircuitInfo info, Label target, FleeIlGenerator ilg)
        {
            var isTemp = ilg.IsTemp;
            if (isTemp)
            {
                info.Branches.MarkLabel(ilg, target);
            }
        }

        private static Label GetLabel(object key, FleeIlGenerator ilg, ShortCircuitInfo info)
        {
            return info.Branches.GetLabel(RuntimeHelpers.GetObjectValue(key), ilg);
        }

        private void PopulateData(ShortCircuitInfo info)
        {
            var andOrChild = this.myRightChild as AndOrElement;
            var flag = andOrChild == null;
            if (flag)
            {
                info.Operands.Push(this.myRightChild);
            }
            else
            {
                andOrChild.PopulateData(info);
            }
            info.Operators.Push(this);
            andOrChild = this.myLeftChild as AndOrElement;
            var flag2 = andOrChild == null;
            if (flag2)
            {
                info.Operands.Push(this.myLeftChild);
            }
            else
            {
                andOrChild.PopulateData(info);
            }
        }
    }
}