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

    internal class TimeSpanLiteralElement : LiteralElement
    {
        private TimeSpan myValue;

        public TimeSpanLiteralElement(string image)
        {
            var flag = !TimeSpan.TryParse(image, out this.myValue);
            if (flag)
            {
                this.ThrowCompileException("CannotParseType", CompileExceptionReason.InvalidFormat, typeof(TimeSpan).Name);
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var index = ilg.GetTempLocalIndex(typeof(TimeSpan));
            Utility.EmitLoadLocalAddress(ilg, index);
            EmitLoad(this.myValue.Ticks, ilg);
            var ci = typeof(TimeSpan).GetConstructor(new[]
            {
                typeof(long)
            });
            ilg.Emit(OpCodes.Call, ci);
            Utility.EmitLoadLocal(ilg, index);
        }

        public override Type ResultType => typeof(TimeSpan);
    }
}