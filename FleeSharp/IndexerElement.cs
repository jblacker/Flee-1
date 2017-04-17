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
    using System.Reflection;
    using System.Reflection.Emit;

    internal class IndexerElement : MemberElement
    {
        private readonly ArgumentList myIndexerElements;
        private ExpressionElement myIndexerElement;

        public IndexerElement(ArgumentList indexer)
        {
            this.myIndexerElements = indexer;
        }

        protected override void ResolveInternal()
        {
            var target = this.myPrevious.TargetType;
            var isArray = target.IsArray;
            if (isArray)
            {
                this.SetupArrayIndexer();
            }
            else
            {
                var flag = !this.FindIndexer(target);
                if (flag)
                {
                    this.ThrowCompileException("TypeNotArrayAndHasNoIndexerOfType", CompileExceptionReason.TypeMismatch, target.Name,
                        this.myIndexerElements);
                }
            }
        }

        private void SetupArrayIndexer()
        {
            this.myIndexerElement = this.myIndexerElements[0];
            var flag = this.myIndexerElements.Count > 1;
            if (flag)
            {
                this.ThrowCompileException("MultiArrayIndexNotSupported", CompileExceptionReason.TypeMismatch);
            }
            else
            {
                var flag2 = !ImplicitConverter.EmitImplicitConvert(this.myIndexerElement.ResultType, typeof(int), null);
                if (flag2)
                {
                    this.ThrowCompileException("ArrayIndexersMustBeOfType", CompileExceptionReason.TypeMismatch, typeof(int).Name);
                }
            }
        }

        private bool FindIndexer(Type targetType)
        {
            var members = targetType.GetDefaultMembers();
            var methods = new List<MethodInfo>();
            var array = members;
            checked
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var mi = array[i];
                    var pi = mi as PropertyInfo;
                    var flag = pi != null;
                    if (flag)
                    {
                        methods.Add(pi.GetGetMethod(true));
                    }
                }
                var func = new FunctionCallElement("Indexer", methods.ToArray(), this.myIndexerElements);
                func.Resolve(this.myServices);
                this.myIndexerElement = func;
                return true;
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            base.Emit(ilg, services);
            var isArray = this.IsArray;
            if (isArray)
            {
                this.EmitArrayLoad(ilg, services);
            }
            else
            {
                this.EmitIndexer(ilg, services);
            }
        }

        private void EmitArrayLoad(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myIndexerElement.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myIndexerElement.ResultType, typeof(int), ilg);
            var elementType = this.ResultType;
            var flag = !elementType.IsValueType;
            if (flag)
            {
                ilg.Emit(OpCodes.Ldelem_Ref);
            }
            else
            {
                this.EmitValueTypeArrayLoad(ilg, elementType);
            }
        }

        private void EmitValueTypeArrayLoad(FleeIlGenerator ilg, Type elementType)
        {
            var nextRequiresAddress = this.NextRequiresAddress;
            if (nextRequiresAddress)
            {
                ilg.Emit(OpCodes.Ldelema, elementType);
            }
            else
            {
                Utility.EmitArrayLoad(ilg, elementType);
            }
        }

        private void EmitIndexer(FleeIlGenerator ilg, IServiceProvider services)
        {
            var func = (FunctionCallElement) this.myIndexerElement;
            func.EmitFunctionCall(this.NextRequiresAddress, ilg, services);
        }

        private Type ArrayType
        {
            get
            {
                var isArray = this.IsArray;
                var arrayType = isArray ? this.myPrevious.TargetType : null;
                return arrayType;
            }
        }

        private bool IsArray => this.myPrevious.TargetType.IsArray;

        protected override bool IsPublic
        {
            get
            {
                var isArray = this.IsArray;
                return isArray || IsElementPublic((MemberElement) this.myIndexerElement);
            }
        }

        public override bool IsStatic => false;

        protected override bool RequiresAddress => !this.IsArray;

        public override Type ResultType
        {
            get
            {
                var isArray = this.IsArray;
                var resultType = isArray ? this.ArrayType.GetElementType() : this.myIndexerElement.ResultType;
                return resultType;
            }
        }
    }
}