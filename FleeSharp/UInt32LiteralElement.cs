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
    using System.Globalization;

    internal class UInt32LiteralElement : IntegralLiteralElement
    {
        private readonly uint myValue;

        public UInt32LiteralElement(uint value)
        {
            this.myValue = value;
        }

        public static UInt32LiteralElement TryCreate(string image, NumberStyles ns)
        {
            uint value;
            var flag = uint.TryParse(image, ns, null, out value);
            var tryCreate = flag ? new UInt32LiteralElement(value) : null;
            return tryCreate;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad((int) this.myValue, ilg);
        }

        public override Type ResultType => typeof(uint);
    }
}