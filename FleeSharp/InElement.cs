using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class InElement : ExpressionElement
    {
        private ExpressionElement MyOperand;

        private List<ExpressionElement> MyArguments;

        private ExpressionElement MyTargetCollectionElement;

        private Type MyTargetCollectionType;

        public override Type ResultType
        {
            get
            {
                return typeof(bool);
            }
        }

        public InElement(ExpressionElement operand, IList listElements)
        {
            this.MyOperand = operand;
            ExpressionElement[] arr = new ExpressionElement[listElements.Count - 1 + 1];
            listElements.CopyTo(arr, 0);
            this.MyArguments = new List<ExpressionElement>(arr);
            this.ResolveForListSearch();
        }

        public InElement(ExpressionElement operand, ExpressionElement targetCollection)
        {
            this.MyOperand = operand;
            this.MyTargetCollectionElement = targetCollection;
            this.ResolveForCollectionSearch();
        }

        private void ResolveForListSearch()
        {
            CompareElement ce = new CompareElement();
            try
            {
                List<ExpressionElement>.Enumerator enumerator = this.MyArguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ExpressionElement argumentElement = enumerator.Current;
                    ce.Initialize(this.MyOperand, argumentElement, LogicalCompareOperation.Equal);
                    ce.Validate();
                }
            }
            finally
            {
                List<ExpressionElement>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
        }

        private void ResolveForCollectionSearch()
        {
            this.MyTargetCollectionType = this.GetTargetCollectionType();
            bool flag = this.MyTargetCollectionType == null;
            if (flag)
            {
                this.ThrowCompileException("SearchArgIsNotKnownCollectionType", CompileExceptionReason.TypeMismatch, new object[]
                {
                    this.MyTargetCollectionElement.ResultType.Name
                });
            }
            MethodInfo mi = this.GetCollectionContainsMethod();
            ParameterInfo p = mi.GetParameters()[0];
            bool flag2 = !ImplicitConverter.EmitImplicitConvert(this.MyOperand.ResultType, p.ParameterType, null);
            if (flag2)
            {
                this.ThrowCompileException("OperandNotConvertibleToCollectionType", CompileExceptionReason.TypeMismatch, new object[]
                {
                    this.MyOperand.ResultType.Name,
                    p.ParameterType.Name
                });
            }
        }

        private Type GetTargetCollectionType()
        {
            Type collType = this.MyTargetCollectionElement.ResultType;
            Type[] interfaces = collType.GetInterfaces();
            Type[] array = interfaces;
            checked
            {
                Type GetTargetCollectionType;
                for (int i = 0; i < array.Length; i++)
                {
                    Type interfaceType = array[i];
                    bool flag = !interfaceType.IsGenericType;
                    if (!flag)
                    {
                        Type genericTypeDef = interfaceType.GetGenericTypeDefinition();
                        bool flag2 = genericTypeDef == typeof(ICollection<>) | genericTypeDef == typeof(IDictionary<, >);
                        if (flag2)
                        {
                            GetTargetCollectionType = interfaceType;
                            return GetTargetCollectionType;
                        }
                    }
                }
                bool flag3 = typeof(IList).IsAssignableFrom(collType);
                if (flag3)
                {
                    GetTargetCollectionType = typeof(IList);
                    return GetTargetCollectionType;
                }
                bool flag4 = typeof(IDictionary).IsAssignableFrom(collType);
                if (flag4)
                {
                    GetTargetCollectionType = typeof(IDictionary);
                    return GetTargetCollectionType;
                }
                GetTargetCollectionType = null;
                return GetTargetCollectionType;
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            bool flag = this.MyTargetCollectionType != null;
            if (flag)
            {
                this.EmitCollectionIn(ilg, services);
            }
            else
            {
                BranchManager bm = new BranchManager();
                bm.GetLabel("endLabel", ilg);
                bm.GetLabel("trueTerminal", ilg);
                FleeILGenerator ilgTemp = this.CreateTempFleeIlGenerator(ilg);
                Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);
                this.EmitListIn(ilgTemp, services, bm);
                bm.ComputeBranches();
                this.EmitListIn(ilg, services, bm);
            }
        }

        private void EmitCollectionIn(FleeILGenerator ilg, IServiceProvider services)
        {
            MethodInfo mi = this.GetCollectionContainsMethod();
            ParameterInfo p = mi.GetParameters()[0];
            this.MyTargetCollectionElement.Emit(ilg, services);
            this.MyOperand.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.MyOperand.ResultType, p.ParameterType, ilg);
            ilg.Emit(OpCodes.Callvirt, mi);
        }

        private MethodInfo GetCollectionContainsMethod()
        {
            string methodName = "Contains";
            bool flag = this.MyTargetCollectionType.IsGenericType && this.MyTargetCollectionType.GetGenericTypeDefinition() == typeof(IDictionary<, >);
            if (flag)
            {
                methodName = "ContainsKey";
            }
            return this.MyTargetCollectionType.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
        }

        private void EmitListIn(FleeILGenerator ilg, IServiceProvider services, BranchManager bm)
        {
            CompareElement ce = new CompareElement();
            Label endLabel = bm.FindLabel("endLabel");
            Label trueTerminal = bm.FindLabel("trueTerminal");
            LocalBuilder lb = ilg.DeclareLocal(this.MyOperand.ResultType);
            int targetIndex = lb.LocalIndex;
            this.MyOperand.Emit(ilg, services);
            Utility.EmitStoreLocal(ilg, targetIndex);
            LocalBasedElement targetShim = new LocalBasedElement(this.MyOperand, targetIndex);
            try
            {
                List<ExpressionElement>.Enumerator enumerator = this.MyArguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ExpressionElement argumentElement = enumerator.Current;
                    ce.Initialize(targetShim, argumentElement, LogicalCompareOperation.Equal);
                    ce.Emit(ilg, services);
                    EmitBranchToTrueTerminal(ilg, trueTerminal, bm);
                }
            }
            finally
            {
                List<ExpressionElement>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Br_S, endLabel);
            bm.MarkLabel(ilg, trueTerminal);
            ilg.MarkLabel(trueTerminal);
            ilg.Emit(OpCodes.Ldc_I4_1);
            bm.MarkLabel(ilg, endLabel);
            ilg.MarkLabel(endLabel);
        }

        private static void EmitBranchToTrueTerminal(FleeILGenerator ilg, Label trueTerminal, BranchManager bm)
        {
            bool isTemp = ilg.IsTemp;
            if (isTemp)
            {
                bm.AddBranch(ilg, trueTerminal);
                ilg.Emit(OpCodes.Brtrue_S, trueTerminal);
            }
            else
            {
                bool flag = !bm.IsLongBranch(ilg, trueTerminal);
                if (flag)
                {
                    ilg.Emit(OpCodes.Brtrue_S, trueTerminal);
                }
                else
                {
                    ilg.Emit(OpCodes.Brtrue, trueTerminal);
                }
            }
        }
    }
}