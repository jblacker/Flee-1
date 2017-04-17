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

    internal class XorElement : BinaryExpressionElement
    {
        protected override Type GetResultType(Type leftType, Type rightType)
        {
            var bitwiseType = Utility.GetBitwiseOpType(leftType, rightType);
            var flag = bitwiseType != null;
            Type getResultType;
            if (flag)
            {
                getResultType = bitwiseType;
            }
            else
            {
                var flag2 = this.AreBothChildrenOfType(typeof(bool));
                getResultType = flag2 ? typeof(bool) : null;
            }
            return getResultType;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var resultType = this.ResultType;
            this.myLeftChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myLeftChild.ResultType, resultType, ilg);
            this.myRightChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myRightChild.ResultType, resultType, ilg);
            ilg.Emit(OpCodes.Xor);
        }

        protected override void GetOperation(object operation)
        {
        }
    }
}