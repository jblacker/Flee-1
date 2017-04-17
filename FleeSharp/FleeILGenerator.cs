using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class FleeILGenerator
    {
        private ILGenerator MyILGenerator;

        private int MyLength;

        private int MyLabelCount;

        private Dictionary<Type, LocalBuilder> MyTempLocals;

        private bool MyIsTemp;

        public int Length
        {
            get
            {
                return this.MyLength;
            }
        }

        public int LabelCount
        {
            get
            {
                return this.MyLabelCount;
            }
        }

        private int ILGeneratorLength
        {
            get
            {
                return Utility.GetILGeneratorLength(this.MyILGenerator);
            }
        }

        public bool IsTemp
        {
            get
            {
                return this.MyIsTemp;
            }
        }

        public FleeILGenerator(ILGenerator ilg, int startLength = 0, bool isTemp = false)
        {
            this.MyILGenerator = ilg;
            this.MyTempLocals = new Dictionary<Type, LocalBuilder>();
            this.MyIsTemp = isTemp;
            this.MyLength = startLength;
        }

        public int GetTempLocalIndex(Type localType)
        {
            LocalBuilder local = null;
            bool flag = !this.MyTempLocals.TryGetValue(localType, out local);
            if (flag)
            {
                local = this.MyILGenerator.DeclareLocal(localType);
                this.MyTempLocals.Add(localType, local);
            }
            return local.LocalIndex;
        }

        public void Emit(OpCode op)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op);
        }

        public void Emit(OpCode op, Type arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, ConstructorInfo arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, MethodInfo arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, FieldInfo arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, byte arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, sbyte arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, short arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, int arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, long arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, float arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, double arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, string arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, Label arg)
        {
            this.RecordOpcode(op);
            this.MyILGenerator.Emit(op, arg);
        }

        public void MarkLabel(Label lbl)
        {
            this.MyILGenerator.MarkLabel(lbl);
        }

        public Label DefineLabel()
        {
            this.MyLabelCount++;
            return this.MyILGenerator.DefineLabel();
        }

        public LocalBuilder DeclareLocal(Type localType)
        {
            return this.MyILGenerator.DeclareLocal(localType);
        }

        private void RecordOpcode(OpCode op)
        {
            int operandLength = GetOpcodeOperandSize(op.OperandType);
            this.MyLength += op.Size + operandLength;
        }

        private static int GetOpcodeOperandSize(OperandType operand)
        {
            int GetOpcodeOperandSize;
            switch (operand)
            {
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    GetOpcodeOperandSize = 4;
                    return GetOpcodeOperandSize;
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    GetOpcodeOperandSize = 8;
                    return GetOpcodeOperandSize;
                case OperandType.InlineNone:
                    GetOpcodeOperandSize = 0;
                    return GetOpcodeOperandSize;
                case OperandType.InlineVar:
                    GetOpcodeOperandSize = 2;
                    return GetOpcodeOperandSize;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    GetOpcodeOperandSize = 1;
                    return GetOpcodeOperandSize;
            }
            Debug.Fail("Unknown operand type");
            return GetOpcodeOperandSize;
        }

        [Conditional("DEBUG")]
        public void ValidateLength()
        {
            Debug.Assert(this.Length == this.ILGeneratorLength, "ILGenerator length mismatch");
        }
    }
}