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
    using System.Diagnostics;
    using System.Reflection.Emit;
    using Exceptions;

    internal abstract class ExpressionElement
    {
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
            this.ThrowCompileException("AmbiguousOverloadedOperator", CompileExceptionReason.AmbiguousMatch, leftType.Name,
                rightType.Name, operation);
        }

        protected FleeIlGenerator CreateTempFleeIlGenerator(FleeIlGenerator ilgCurrent)
        {
            var dm = new DynamicMethod("temp", typeof(int), null, this.GetType());
            return new FleeIlGenerator(dm.GetILGenerator(), ilgCurrent.Length, true);
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

        public abstract Type ResultType { get; }
    }
}