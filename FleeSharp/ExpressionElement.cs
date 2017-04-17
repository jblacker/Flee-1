using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Flee
{
    using Exceptions;

    internal abstract class ExpressionElement
    {
        public abstract Type ResultType
        {
            get;
        }

        protected string Name
        {
            get
            {
                var key = this.GetType().Name;
                var value = FleeResourceManager.Instance.GetElementNameString(key);
                Debug.Assert(value != null, $"Element name for '{key}' not in resource file");
                return value;
            }
        }


        public abstract void Emit(FleeIlGenerator ilg, IServiceProvider services);

        public override string ToString()
        {
            return this.Name;
        }

        protected void ThrowCompileException(string messageKey, CompileExceptionReason reason, params object[] arguments)
        {
            var messageTemplate = FleeResourceManager.Instance.GetCompileErrorString(messageKey);
            var message = string.Format(messageTemplate, arguments);
            message = this.Name + ": " + message;
            throw new ExpressionCompileException(message, reason);
        }

        protected void ThrowAmbiguousCallException(Type leftType, Type rightType, object operation)
        {
            this.ThrowCompileException("AmbiguousOverloadedOperator", CompileExceptionReason.AmbiguousMatch, new object[]
            {
                leftType.Name,
                rightType.Name,
                operation
            });
        }

        protected FleeIlGenerator CreateTempFleeIlGenerator(FleeIlGenerator ilgCurrent)
        {
            var dm = new DynamicMethod("temp", typeof(int), null, this.GetType());
            return new FleeIlGenerator(dm.GetILGenerator(), ilgCurrent.Length, true);
        }
    }
}