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

    internal class NotElement : UnaryElement
    {
        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var flag = this.myChild.ResultType == typeof(bool);
            if (flag)
            {
                this.EmitLogical(ilg, services);
            }
            else
            {
                this.myChild.Emit(ilg, services);
                ilg.Emit(OpCodes.Not);
            }
        }

        private void EmitLogical(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myChild.Emit(ilg, services);
            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Ceq);
        }

        protected override Type GetResultType(Type childType)
        {
            var flag = childType == typeof(bool);
            Type getResultType;
            if (flag)
            {
                getResultType = typeof(bool);
            }
            else
            {
                var flag2 = Utility.IsIntegralType(childType);
                getResultType = flag2 ? childType : null;
            }
            return getResultType;
        }
    }
}