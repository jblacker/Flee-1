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
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class InElement : ExpressionElement
    {
        private readonly List<ExpressionElement> myArguments;
        private readonly ExpressionElement myOperand;

        private readonly ExpressionElement myTargetCollectionElement;

        private Type myTargetCollectionType;

        public InElement(ExpressionElement operand, IList listElements)
        {
            this.myOperand = operand;
            var arr = new ExpressionElement[listElements.Count - 1 + 1];
            listElements.CopyTo(arr, 0);
            this.myArguments = new List<ExpressionElement>(arr);
            this.ResolveForListSearch();
        }

        public InElement(ExpressionElement operand, ExpressionElement targetCollection)
        {
            this.myOperand = operand;
            this.myTargetCollectionElement = targetCollection;
            this.ResolveForCollectionSearch();
        }

        private void ResolveForListSearch()
        {
            var ce = new CompareElement();
            this.myArguments.ForEach(a =>
            {
                ce.Initialize(this.myOperand, a, LogicalCompareOperation.Equal);
                ce.Validate();
            });
            //try
            //{
            //    var enumerator = this.myArguments.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var argumentElement = enumerator.Current;
            //        ce.Initialize(this.myOperand, argumentElement, LogicalCompareOperation.Equal);
            //        ce.Validate();
            //    }
            //}
            //finally
            //{
            //    List<ExpressionElement>.Enumerator enumerator;
            //    ((IDisposable)enumerator).Dispose();
            //}
        }

        private void ResolveForCollectionSearch()
        {
            this.myTargetCollectionType = this.GetTargetCollectionType();
            var flag = this.myTargetCollectionType == null;
            if (flag)
            {
                this.ThrowCompileException("SearchArgIsNotKnownCollectionType", CompileExceptionReason.TypeMismatch,
                    this.myTargetCollectionElement.ResultType.Name);
            }
            var mi = this.GetCollectionContainsMethod();
            var p = mi.GetParameters()[0];
            var flag2 = !ImplicitConverter.EmitImplicitConvert(this.myOperand.ResultType, p.ParameterType, null);
            if (flag2)
            {
                this.ThrowCompileException("OperandNotConvertibleToCollectionType", CompileExceptionReason.TypeMismatch,
                    this.myOperand.ResultType.Name, p.ParameterType.Name);
            }
        }

        private Type GetTargetCollectionType()
        {
            var collType = this.myTargetCollectionElement.ResultType;
            var interfaces = collType.GetInterfaces();
            var array = interfaces;
            checked
            {
                Type getTargetCollectionType;
                for (var i = 0; i < array.Length; i++)
                {
                    var interfaceType = array[i];
                    var flag = !interfaceType.IsGenericType;
                    if (!flag)
                    {
                        var genericTypeDef = interfaceType.GetGenericTypeDefinition();
                        var flag2 = (genericTypeDef == typeof(ICollection<>)) | (genericTypeDef == typeof(IDictionary<,>));
                        if (flag2)
                        {
                            getTargetCollectionType = interfaceType;
                            return getTargetCollectionType;
                        }
                    }
                }
                var flag3 = typeof(IList).IsAssignableFrom(collType);
                if (flag3)
                {
                    getTargetCollectionType = typeof(IList);
                    return getTargetCollectionType;
                }
                var flag4 = typeof(IDictionary).IsAssignableFrom(collType);
                if (flag4)
                {
                    getTargetCollectionType = typeof(IDictionary);
                    return getTargetCollectionType;
                }
                return null;
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var flag = this.myTargetCollectionType != null;
            if (flag)
            {
                this.EmitCollectionIn(ilg, services);
            }
            else
            {
                var bm = new BranchManager();
                bm.GetLabel("endLabel", ilg);
                bm.GetLabel("trueTerminal", ilg);
                var ilgTemp = this.CreateTempFleeIlGenerator(ilg);
                Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);
                this.EmitListIn(ilgTemp, services, bm);
                bm.ComputeBranches();
                this.EmitListIn(ilg, services, bm);
            }
        }

        private void EmitCollectionIn(FleeIlGenerator ilg, IServiceProvider services)
        {
            var mi = this.GetCollectionContainsMethod();
            var p = mi.GetParameters()[0];
            this.myTargetCollectionElement.Emit(ilg, services);
            this.myOperand.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myOperand.ResultType, p.ParameterType, ilg);
            ilg.Emit(OpCodes.Callvirt, mi);
        }

        private MethodInfo GetCollectionContainsMethod()
        {
            var methodName = "Contains";
            var flag = this.myTargetCollectionType.IsGenericType &&
                this.myTargetCollectionType.GetGenericTypeDefinition() == typeof(IDictionary<,>);
            if (flag)
            {
                methodName = "ContainsKey";
            }
            return this.myTargetCollectionType.GetMethod(methodName,
                BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
        }

        private void EmitListIn(FleeIlGenerator ilg, IServiceProvider services, BranchManager bm)
        {
            var ce = new CompareElement();
            var endLabel = bm.FindLabel("endLabel");
            var trueTerminal = bm.FindLabel("trueTerminal");
            var lb = ilg.DeclareLocal(this.myOperand.ResultType);
            var targetIndex = lb.LocalIndex;
            this.myOperand.Emit(ilg, services);
            Utility.EmitStoreLocal(ilg, targetIndex);
            var targetShim = new LocalBasedElement(this.myOperand, targetIndex);
            this.myArguments.ForEach(argumentElement =>
            {
                ce.Initialize(targetShim, argumentElement, LogicalCompareOperation.Equal);
                ce.Emit(ilg, services);
                EmitBranchToTrueTerminal(ilg, trueTerminal, bm);
            });
            //try
            //{
            //    var enumerator = this.myArguments.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var argumentElement = enumerator.Current;
            //        ce.Initialize(targetShim, argumentElement, LogicalCompareOperation.Equal);
            //        ce.Emit(ilg, services);
            //        EmitBranchToTrueTerminal(ilg, trueTerminal, bm);
            //    }
            //}
            //finally
            //{
            //    List<ExpressionElement>.Enumerator enumerator;
            //    ((IDisposable)enumerator).Dispose();
            //}

            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Br_S, endLabel);
            bm.MarkLabel(ilg, trueTerminal);
            ilg.MarkLabel(trueTerminal);
            ilg.Emit(OpCodes.Ldc_I4_1);
            bm.MarkLabel(ilg, endLabel);
            ilg.MarkLabel(endLabel);
        }

        private static void EmitBranchToTrueTerminal(FleeIlGenerator ilg, Label trueTerminal, BranchManager bm)
        {
            var isTemp = ilg.IsTemp;
            if (isTemp)
            {
                bm.AddBranch(ilg, trueTerminal);
                ilg.Emit(OpCodes.Brtrue_S, trueTerminal);
            }
            else
            {
                var flag = !bm.IsLongBranch(ilg, trueTerminal);
                ilg.Emit(flag ? OpCodes.Brtrue_S : OpCodes.Brtrue, trueTerminal);
            }
        }

        public override Type ResultType => typeof(bool);
    }
}