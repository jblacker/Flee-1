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


#pragma warning disable 612

namespace Flee
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    using Extensions;

    internal class FunctionCallElement : MemberElement
    {
        private readonly ArgumentList myArguments;

        private readonly ICollection<MethodInfo> myMethods;

        private Type myOnDemandFunctionReturnType;

        private CustomMethodInfo myTargetMethodInfo;

        public FunctionCallElement(string name, ArgumentList arguments)
        {
            this.myName = name;
            this.myArguments = arguments;
        }

        internal FunctionCallElement(string name, ICollection<MethodInfo> methods, ArgumentList arguments)
        {
            this.myName = name;
            this.myArguments = arguments;
            this.myMethods = methods;
        }

        protected override void ResolveInternal()
        {
            var argTypes = this.myArguments.GetArgumentTypes();
            var methods = this.myMethods;
            var flag = methods == null;
            if (flag)
            {
                var arr = this.GetMembers(MemberTypes.Method);
                var arr2 = new MethodInfo[arr.Length - 1 + 1];
                Array.Copy(arr, arr2, arr.Length);
                methods = arr2;
            }
            var flag2 = methods.Count > 0;
            if (flag2)
            {
                this.BindToMethod(methods, this.myPrevious, argTypes);
            }
            else
            {
                this.myOnDemandFunctionReturnType = this.myContext.Variables.ResolveOnDemandFunction(this.myName, argTypes);
                var flag3 = this.myOnDemandFunctionReturnType == null;
                if (flag3)
                {
                    this.ThrowFunctionNotFoundException(this.myPrevious);
                }
            }
        }

        private void ThrowFunctionNotFoundException(MemberElement previous)
        {
            var flag = previous == null;
            if (flag)
            {
                this.ThrowCompileException("UndefinedFunction", CompileExceptionReason.UndefinedName, this.myName, this.myArguments);
            }
            else
            {
                this.ThrowCompileException("UndefinedFunctionOnType", CompileExceptionReason.UndefinedName, this.myName,
                    this.myArguments, previous.TargetType.Name);
            }
        }

        private void ThrowNoAccessibleMethodsException(MemberElement previous)
        {
            var flag = previous == null;
            if (flag)
            {
                this.ThrowCompileException("NoAccessibleMatches", CompileExceptionReason.AccessDenied, this.myName, this.myArguments);
            }
            else
            {
                this.ThrowCompileException("NoAccessibleMatchesOnType", CompileExceptionReason.AccessDenied, this.myName,
                    this.myArguments, previous.TargetType.Name);
            }
        }

        private void ThrowAmbiguousMethodCallException()
        {
            this.ThrowCompileException("AmbiguousCallOfFunction", CompileExceptionReason.AmbiguousMatch, this.myName, this.myArguments);
        }

        private void BindToMethod(ICollection<MethodInfo> methods, MemberElement previous, Type[] argTypes)
        {
            var customInfos = new List<CustomMethodInfo>();
            methods.Each(m => customInfos.Add(new CustomMethodInfo(m)));
            //try
            //{
            //    var enumerator = methods.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var mi = enumerator.Current;
            //        var cmi = new CustomMethodInfo(mi);
            //        customInfos.Add(cmi);
            //    }
            //}
            //finally
            //{
            //    IEnumerator<MethodInfo> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}

            var arr = customInfos.ToArray();
            customInfos.Clear();
            var array = arr;
            checked
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var cmi2 = array[i];
                    var flag = cmi2.IsMatch(argTypes);
                    if (flag)
                    {
                        customInfos.Add(cmi2);
                    }
                }
                var flag2 = customInfos.Count == 0;
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
            var array = infos;
            checked
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var cmi = array[i];
                    cmi.ComputeScore(argTypes);
                }
                Array.Sort(infos);
                infos = this.GetAccessibleInfos(infos);
                var flag = infos.Length == 0;
                if (flag)
                {
                    this.ThrowNoAccessibleMethodsException(previous);
                }
                this.DetectAmbiguousMatches(infos);
                this.myTargetMethodInfo = infos[0];
            }
        }

        private CustomMethodInfo[] GetAccessibleInfos(CustomMethodInfo[] infos)
        {
            var accessible = new List<CustomMethodInfo>();
            checked
            {
                for (var i = 0; i < infos.Length; i++)
                {
                    var cmi = infos[i];
                    var flag = cmi.IsAccessible(this);
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
            var sameScores = new List<CustomMethodInfo>();
            var first = infos[0];
            checked
            {
                for (var i = 0; i < infos.Length; i++)
                {
                    var cmi = infos[i];
                    var flag = ((IEquatable<CustomMethodInfo>) cmi).Equals(first);
                    if (flag)
                    {
                        sameScores.Add(cmi);
                    }
                }
                var flag2 = sameScores.Count > 1;
                if (flag2)
                {
                    this.ThrowAmbiguousMethodCallException();
                }
            }
        }

        protected override void Validate()
        {
            base.Validate();
            var flag = this.myOnDemandFunctionReturnType != null;
            if (!flag)
            {
                var flag2 = this.Method.ReturnType == typeof(void);
                if (flag2)
                {
                    this.ThrowCompileException("FunctionHasNoReturnValue", CompileExceptionReason.FunctionHasNoReturnValue, this.myName);
                }
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            base.Emit(ilg, services);
            var elements = this.myArguments.ToArray();
            var flag = this.myOnDemandFunctionReturnType != null;
            if (flag)
            {
                this.EmitOnDemandFunction(elements, ilg, services);
            }
            else
            {
                var isOwnerMember = this.myOptions.IsOwnerType(this.Method.ReflectedType);
                var flag2 = this.myPrevious == null && isOwnerMember && !this.IsStatic;
                if (flag2)
                {
                    this.EmitLoadOwner(ilg);
                }
                this.EmitFunctionCall(this.NextRequiresAddress, ilg, services);
            }
        }

        private void EmitOnDemandFunction(ExpressionElement[] elements, FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoadVariables(ilg);
            ilg.Emit(OpCodes.Ldstr, this.myName);
            EmitElementArrayLoad(elements, typeof(object), ilg, services);
            var mi = VariableCollection.GetFunctionInvokeMethod(this.myOnDemandFunctionReturnType);
            this.EmitMethodCall(mi, ilg);
        }

        private void EmitParamArrayArguments(ParameterInfo[] parameters, ExpressionElement[] elements, FleeIlGenerator ilg,
            IServiceProvider services)
        {
            var fixedParameters = new ParameterInfo[this.myTargetMethodInfo.myFixedArgTypes.Length - 1 + 1];
            Array.Copy(parameters, fixedParameters, fixedParameters.Length);
            var fixedElements = new ExpressionElement[this.myTargetMethodInfo.myFixedArgTypes.Length - 1 + 1];
            Array.Copy(elements, fixedElements, fixedElements.Length);
            this.EmitRegularFunctionInternal(fixedParameters, fixedElements, ilg, services);
            var paramArrayElements = new ExpressionElement[elements.Length - fixedElements.Length - 1 + 1];
            Array.Copy(elements, fixedElements.Length, paramArrayElements, 0, paramArrayElements.Length);
            EmitElementArrayLoad(paramArrayElements, this.myTargetMethodInfo.paramArrayElementType, ilg, services);
        }

        private static void EmitElementArrayLoad(ExpressionElement[] elements, Type arrayElementType, FleeIlGenerator ilg,
            IServiceProvider services)
        {
            LiteralElement.EmitLoad(elements.Length, ilg);
            ilg.Emit(OpCodes.Newarr, arrayElementType);
            var local = ilg.DeclareLocal(arrayElementType.MakeArrayType());
            var arrayLocalIndex = local.LocalIndex;
            Utility.EmitStoreLocal(ilg, arrayLocalIndex);
            var num = elements.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                Utility.EmitLoadLocal(ilg, arrayLocalIndex);
                LiteralElement.EmitLoad(i, ilg);
                var element = elements[i];
                element.Emit(ilg, services);
                ImplicitConverter.EmitImplicitConvert(element.ResultType, arrayElementType, ilg);
                Utility.EmitArrayStore(ilg, arrayElementType);
            }
            Utility.EmitLoadLocal(ilg, arrayLocalIndex);
        }

        public void EmitFunctionCall(bool nextRequiresAddress, FleeIlGenerator ilg, IServiceProvider services)
        {
            var parameters = this.Method.GetParameters();
            var elements = this.myArguments.ToArray();
            var flag = !this.myTargetMethodInfo.isParamArray;
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

        private void EmitRegularFunctionInternal(ParameterInfo[] parameters, ExpressionElement[] elements, FleeIlGenerator ilg,
            IServiceProvider services)
        {
            Debug.Assert(parameters.Length == elements.Length, "argument count mismatch");
            var num = parameters.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                var element = elements[i];
                var pi = parameters[i];
                element.Emit(ilg, services);
                var success = ImplicitConverter.EmitImplicitConvert(element.ResultType, pi.ParameterType, ilg);
                Debug.Assert(success, "conversion failed");
            }
        }

        protected override bool IsPublic => this.Method.IsPublic;

        public override bool IsStatic => this.Method.IsStatic;

        private MethodInfo Method => this.myTargetMethodInfo.Target;

        protected override bool RequiresAddress => !IsGetTypeMethod(this.Method);

        public override Type ResultType
        {
            get
            {
                var flag = this.myOnDemandFunctionReturnType != null;
                var resultType = flag ? this.myOnDemandFunctionReturnType : this.Method.ReturnType;
                return resultType;
            }
        }
    }
}