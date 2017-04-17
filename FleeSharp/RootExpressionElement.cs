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
    using System.Reflection.Emit;

    internal class RootExpressionElement : ExpressionElement
    {
        private readonly ExpressionElement myChild;

        private readonly Type myResultType;

        public RootExpressionElement(ExpressionElement child, Type resultType)
        {
            this.myChild = child;
            this.myResultType = resultType;
            this.Validate();
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myChild.ResultType, this.myResultType, ilg);
            var options = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));
            var flag = !options.IsGeneric;
            if (flag)
            {
                ImplicitConverter.EmitImplicitConvert(this.myResultType, typeof(object), ilg);
            }
            ilg.Emit(OpCodes.Ret);
        }

        private void Validate()
        {
            var flag = !ImplicitConverter.EmitImplicitConvert(this.myChild.ResultType, this.myResultType, null);
            if (flag)
            {
                this.ThrowCompileException("CannotConvertTypeToExpressionResult", CompileExceptionReason.TypeMismatch,
                    this.myChild.ResultType.Name, this.myResultType.Name);
            }
        }

        public override Type ResultType => typeof(object);
    }
}