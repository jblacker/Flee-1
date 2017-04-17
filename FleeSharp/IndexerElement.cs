using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class IndexerElement : MemberElement
    {
        private ExpressionElement myIndexerElement;

        private readonly ArgumentList myIndexerElements;

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

        protected override bool IsPublic
        {
            get
            {
                var isArray = this.IsArray;
                return isArray || IsElementPublic((MemberElement)this.myIndexerElement);
            }
        }

        public override bool IsStatic => false;

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
                    this.ThrowCompileException("TypeNotArrayAndHasNoIndexerOfType", CompileExceptionReason.TypeMismatch, new object[]
                    {
                        target.Name,
                        this.myIndexerElements
                    });
                }
            }
        }

        private void SetupArrayIndexer()
        {
            this.myIndexerElement = this.myIndexerElements[0];
            var flag = this.myIndexerElements.Count > 1;
            if (flag)
            {
                this.ThrowCompileException("MultiArrayIndexNotSupported", CompileExceptionReason.TypeMismatch, new object[0]);
            }
            else
            {
                var flag2 = !ImplicitConverter.EmitImplicitConvert(this.myIndexerElement.ResultType, typeof(int), null);
                if (flag2)
                {
                    this.ThrowCompileException("ArrayIndexersMustBeOfType", CompileExceptionReason.TypeMismatch, new object[]
                    {
                        typeof(int).Name
                    });
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
            var func = (FunctionCallElement)this.myIndexerElement;
            func.EmitFunctionCall(this.NextRequiresAddress, ilg, services);
        }
    }
}