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

    internal class NegateElement : UnaryElement
    {
        protected override Type GetResultType(Type childType)
        {
            var tc = Type.GetTypeCode(childType);
            var mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", childType, childType);
            var flag = mi != null;
            Type getResultType;
            if (flag)
            {
                getResultType = mi.ReturnType;
            }
            else
            {
                switch (tc)
                {
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        getResultType = childType;
                        return getResultType;
                    case TypeCode.UInt32:
                        getResultType = typeof(long);
                        return getResultType;
                }
                getResultType = null;
            }
            return getResultType;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var resultType = this.ResultType;
            this.myChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myChild.ResultType, resultType, ilg);
            var mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", resultType, resultType);
            var flag = mi == null;
            if (flag)
            {
                ilg.Emit(OpCodes.Neg);
            }
            else
            {
                ilg.Emit(OpCodes.Call, mi);
            }
        }
    }
}