using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class FunctionCallElement : MemberElement
    {
        private ArgumentList MyArguments;

        private ICollection<MethodInfo> MyMethods;

        private CustomMethodInfo MyTargetMethodInfo;

        private Type MyOnDemandFunctionReturnType;

        private MethodInfo Method
        {
            get
            {
                return this.MyTargetMethodInfo.Target;
            }
        }

        public override Type ResultType
        {
            get
            {
                bool flag = this.MyOnDemandFunctionReturnType != null;
                Type ResultType;
                if (flag)
                {
                    ResultType = this.MyOnDemandFunctionReturnType;
                }
                else
                {
                    ResultType = this.Method.ReturnType;
                }
                return ResultType;
            }
        }

        protected override bool RequiresAddress
        {
            get
            {
                return !IsGetTypeMethod(this.Method);
            }
        }

        protected override bool IsPublic
        {
            get
            {
                return this.Method.IsPublic;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return this.Method.IsStatic;
            }
        }

        public FunctionCallElement(string name, ArgumentList arguments)
        {
            this.MyName = name;
            this.MyArguments = arguments;
        }

        internal FunctionCallElement(string name, ICollection<MethodInfo> methods, ArgumentList arguments)
        {
            this.MyName = name;
            this.MyArguments = arguments;
            this.MyMethods = methods;
        }

        protected override void ResolveInternal()
        {
            Type[] argTypes = this.MyArguments.GetArgumentTypes();
            ICollection<MethodInfo> methods = this.MyMethods;
            bool flag = methods == null;
            if (flag)
            {
                MemberInfo[] arr = this.GetMembers(MemberTypes.Method);
                MethodInfo[] arr2 = new MethodInfo[arr.Length - 1 + 1];
                Array.Copy(arr, arr2, arr.Length);
                methods = arr2;
            }
            bool flag2 = methods.Count > 0;
            if (flag2)
            {
                this.BindToMethod(methods, this.MyPrevious, argTypes);
            }
            else
            {
                this.MyOnDemandFunctionReturnType = this.MyContext.Variables.ResolveOnDemandFunction(this.MyName, argTypes);
                bool flag3 = this.MyOnDemandFunctionReturnType == null;
                if (flag3)
                {
                    this.ThrowFunctionNotFoundException(this.MyPrevious);
                }
            }
        }

        private void ThrowFunctionNotFoundException(MemberElement previous)
        {
            bool flag = previous == null;
            if (flag)
            {
                this.ThrowCompileException("UndefinedFunction", CompileExceptionReason.UndefinedName, new object[]
                {
                    this.MyName,
                    this.MyArguments
                });
            }
            else
            {
                this.ThrowCompileException("UndefinedFunctionOnType", CompileExceptionReason.UndefinedName, new object[]
                {
                    this.MyName,
                    this.MyArguments,
                    previous.TargetType.Name
                });
            }
        }

        private void ThrowNoAccessibleMethodsException(MemberElement previous)
        {
            bool flag = previous == null;
            if (flag)
            {
                this.ThrowCompileException("NoAccessibleMatches", CompileExceptionReason.AccessDenied, new object[]
                {
                    this.MyName,
                    this.MyArguments
                });
            }
            else
            {
                this.ThrowCompileException("NoAccessibleMatchesOnType", CompileExceptionReason.AccessDenied, new object[]
                {
                    this.MyName,
                    this.MyArguments,
                    previous.TargetType.Name
                });
            }
        }

        private void ThrowAmbiguousMethodCallException()
        {
            this.ThrowCompileException("AmbiguousCallOfFunction", CompileExceptionReason.AmbiguousMatch, new object[]
            {
                this.MyName,
                this.MyArguments
            });
        }

        private void BindToMethod(ICollection<MethodInfo> methods, MemberElement previous, Type[] argTypes)
        {
            List<CustomMethodInfo> customInfos = new List<CustomMethodInfo>();
            try
            {
                IEnumerator<MethodInfo> enumerator = methods.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MethodInfo mi = enumerator.Current;
                    CustomMethodInfo cmi = new CustomMethodInfo(mi);
                    customInfos.Add(cmi);
                }
            }
            finally
            {
                IEnumerator<MethodInfo> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            CustomMethodInfo[] arr = customInfos.ToArray();
            customInfos.Clear();
            CustomMethodInfo[] array = arr;
            checked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    CustomMethodInfo cmi2 = array[i];
                    bool flag = cmi2.IsMatch(argTypes);
                    if (flag)
                    {
                        customInfos.Add(cmi2);
                    }
                }
                bool flag2 = customInfos.Count == 0;
                if (flag2)
                {
                    this.ThrowFunctionNotFoundException(previous);
                }
                else
                {
                    this.ResolveOverloads(customInfos.ToArray(), previous, argTypes);
                }
            }
        }

        private void ResolveOverloads(CustomMethodInfo[] infos, MemberElement previous, Type[] argTypes)
        {
            CustomMethodInfo[] array = infos;
            checked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    CustomMethodInfo cmi = array[i];
                    cmi.ComputeScore(argTypes);
                }
                Array.Sort<CustomMethodInfo>(infos);
                infos = this.GetAccessibleInfos(infos);
                bool flag = infos.Length == 0;
                if (flag)
                {
                    this.ThrowNoAccessibleMethodsException(previous);
                }
                this.DetectAmbiguousMatches(infos);
                this.MyTargetMethodInfo = infos[0];
            }
        }

        private CustomMethodInfo[] GetAccessibleInfos(CustomMethodInfo[] infos)
        {
            List<CustomMethodInfo> accessible = new List<CustomMethodInfo>();
            checked
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    CustomMethodInfo cmi = infos[i];
                    bool flag = cmi.IsAccessible(this);
                    if (flag)
                    {
                        accessible.Add(cmi);
                    }
                }
                return accessible.ToArray();
            }
        }

        private void DetectAmbiguousMatches(CustomMethodInfo[] infos)
        {
            List<CustomMethodInfo> sameScores = new List<CustomMethodInfo>();
            CustomMethodInfo first = infos[0];
            checked
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    CustomMethodInfo cmi = infos[i];
                    bool flag = ((IEquatable<CustomMethodInfo>)cmi).Equals(first);
                    if (flag)
                    {
                        sameScores.Add(cmi);
                    }
                }
                bool flag2 = sameScores.Count > 1;
                if (flag2)
                {
                    this.ThrowAmbiguousMethodCallException();
                }
            }
        }

        protected override void Validate()
        {
            base.Validate();
            bool flag = this.MyOnDemandFunctionReturnType != null;
            if (!flag)
            {
                bool flag2 = this.Method.ReturnType == typeof(void);
                if (flag2)
                {
                    this.ThrowCompileException("FunctionHasNoReturnValue", CompileExceptionReason.FunctionHasNoReturnValue, new object[]
                    {
                        this.MyName
                    });
                }
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            base.Emit(ilg, services);
            ExpressionElement[] elements = this.MyArguments.ToArray();
            bool flag = this.MyOnDemandFunctionReturnType != null;
            if (flag)
            {
                this.EmitOnDemandFunction(elements, ilg, services);
            }
            else
            {
                bool isOwnerMember = this.MyOptions.IsOwnerType(this.Method.ReflectedType);
                bool flag2 = this.MyPrevious == null && isOwnerMember && !this.IsStatic;
                if (flag2)
                {
                    this.EmitLoadOwner(ilg);
                }
                this.EmitFunctionCall(this.NextRequiresAddress, ilg, services);
            }
        }

        private void EmitOnDemandFunction(ExpressionElement[] elements, FleeILGenerator ilg, IServiceProvider services)
        {
            EmitLoadVariables(ilg);
            ilg.Emit(OpCodes.Ldstr, this.MyName);
            EmitElementArrayLoad(elements, typeof(object), ilg, services);
            MethodInfo mi = VariableCollection.GetFunctionInvokeMethod(this.MyOnDemandFunctionReturnType);
            this.EmitMethodCall(mi, ilg);
        }

        private void EmitParamArrayArguments(ParameterInfo[] parameters, ExpressionElement[] elements, FleeILGenerator ilg, IServiceProvider services)
        {
            ParameterInfo[] fixedParameters = new ParameterInfo[this.MyTargetMethodInfo.myFixedArgTypes.Length - 1 + 1];
            Array.Copy(parameters, fixedParameters, fixedParameters.Length);
            ExpressionElement[] fixedElements = new ExpressionElement[this.MyTargetMethodInfo.myFixedArgTypes.Length - 1 + 1];
            Array.Copy(elements, fixedElements, fixedElements.Length);
            this.EmitRegularFunctionInternal(fixedParameters, fixedElements, ilg, services);
            ExpressionElement[] paramArrayElements = new ExpressionElement[elements.Length - fixedElements.Length - 1 + 1];
            Array.Copy(elements, fixedElements.Length, paramArrayElements, 0, paramArrayElements.Length);
            EmitElementArrayLoad(paramArrayElements, this.MyTargetMethodInfo.paramArrayElementType, ilg, services);
        }

        private static void EmitElementArrayLoad(ExpressionElement[] elements, Type arrayElementType, FleeILGenerator ilg, IServiceProvider services)
        {
            LiteralElement.EmitLoad(elements.Length, ilg);
            ilg.Emit(OpCodes.Newarr, arrayElementType);
            LocalBuilder local = ilg.DeclareLocal(arrayElementType.MakeArrayType());
            int arrayLocalIndex = local.LocalIndex;
            Utility.EmitStoreLocal(ilg, arrayLocalIndex);
            int num = elements.Length - 1;
            for (int i = 0; i <= num; i++)
            {
                Utility.EmitLoadLocal(ilg, arrayLocalIndex);
                LiteralElement.EmitLoad(i, ilg);
                ExpressionElement element = elements[i];
                element.Emit(ilg, services);
                ImplicitConverter.EmitImplicitConvert(element.ResultType, arrayElementType, ilg);
                Utility.EmitArrayStore(ilg, arrayElementType);
            }
            Utility.EmitLoadLocal(ilg, arrayLocalIndex);
        }

        public void EmitFunctionCall(bool nextRequiresAddress, FleeILGenerator ilg, IServiceProvider services)
        {
            ParameterInfo[] parameters = this.Method.GetParameters();
            ExpressionElement[] elements = this.MyArguments.ToArray();
            bool flag = !this.MyTargetMethodInfo.isParamArray;
            if (flag)
            {
                this.EmitRegularFunctionInternal(parameters, elements, ilg, services);
            }
            else
            {
                this.EmitParamArrayArguments(parameters, elements, ilg, services);
            }
            EmitMethodCall(this.ResultType, nextRequiresAddress, this.Method, ilg);
        }

        private void EmitRegularFunctionInternal(ParameterInfo[] parameters, ExpressionElement[] elements, FleeILGenerator ilg, IServiceProvider services)
        {
            Debug.Assert(parameters.Length == elements.Length, "argument count mismatch");
            int num = parameters.Length - 1;
            for (int i = 0; i <= num; i++)
            {
                ExpressionElement element = elements[i];
                ParameterInfo pi = parameters[i];
                element.Emit(ilg, services);
                bool success = ImplicitConverter.EmitImplicitConvert(element.ResultType, pi.ParameterType, ilg);
                Debug.Assert(success, "conversion failed");
            }
        }
    }
}