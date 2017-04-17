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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class FleeIlGenerator
    {
        private readonly ILGenerator myIlGenerator;

        private readonly Dictionary<Type, LocalBuilder> myTempLocals;

        public FleeIlGenerator(ILGenerator ilg, int startLength = 0, bool isTemp = false)
        {
            this.myIlGenerator = ilg;
            this.myTempLocals = new Dictionary<Type, LocalBuilder>();
            this.IsTemp = isTemp;
            this.Length = startLength;
        }

        public int GetTempLocalIndex(Type localType)
        {
            LocalBuilder local = null;
            var flag = !this.myTempLocals.TryGetValue(localType, out local);
            if (flag)
            {
                local = this.myIlGenerator.DeclareLocal(localType);
                this.myTempLocals.Add(localType, local);
            }
            return local.LocalIndex;
        }

        public void Emit(OpCode op)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op);
        }

        public void Emit(OpCode op, Type arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, ConstructorInfo arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, MethodInfo arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, FieldInfo arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, byte arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, sbyte arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, short arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, int arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, long arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, float arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, double arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, string arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void Emit(OpCode op, Label arg)
        {
            this.RecordOpcode(op);
            this.myIlGenerator.Emit(op, arg);
        }

        public void MarkLabel(Label lbl)
        {
            this.myIlGenerator.MarkLabel(lbl);
        }

        public Label DefineLabel()
        {
            this.LabelCount++;
            return this.myIlGenerator.DefineLabel();
        }

        public LocalBuilder DeclareLocal(Type localType)
        {
            return this.myIlGenerator.DeclareLocal(localType);
        }

        private void RecordOpcode(OpCode op)
        {
            var operandLength = GetOpcodeOperandSize(op.OperandType);
            this.Length += op.Size + operandLength;
        }

        private static int GetOpcodeOperandSize(OperandType operand)
        {
            var getOpcodeOperandSize = 0;
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
                    getOpcodeOperandSize = 4;
                    return getOpcodeOperandSize;
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    getOpcodeOperandSize = 8;
                    return getOpcodeOperandSize;
                case OperandType.InlineNone:
                    getOpcodeOperandSize = 0;
                    return getOpcodeOperandSize;
                case OperandType.InlineVar:
                    getOpcodeOperandSize = 2;
                    return getOpcodeOperandSize;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    getOpcodeOperandSize = 1;
                    return getOpcodeOperandSize;
            }
            Debug.Fail("Unknown operand type");
            return getOpcodeOperandSize;
        }

        [Conditional("DEBUG")]
        public void ValidateLength()
        {
            Debug.Assert(this.Length == this.IlGeneratorLength, "ILGenerator length mismatch");
        }

        private int IlGeneratorLength => Utility.GetILGeneratorLength(this.myIlGenerator);

        public bool IsTemp { get; set; }

        public int LabelCount { get; private set; }

        public int Length { get; private set; }
    }
}