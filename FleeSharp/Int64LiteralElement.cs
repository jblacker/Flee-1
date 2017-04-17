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
    using Microsoft.VisualBasic.CompilerServices;

    internal class Int64LiteralElement : IntegralLiteralElement
    {
        private const string MinValue = "9223372036854775808";

        private readonly bool myIsMinValue;
        private long myValue;

        public Int64LiteralElement(long value)
        {
            this.myValue = value;
        }

        private Int64LiteralElement()
        {
            this.myIsMinValue = true;
        }

        public static Int64LiteralElement TryCreate(string image, bool isHex, bool negated)
        {
            var flag = negated & (Operators.CompareString(image, "9223372036854775808", false) == 0);
            Int64LiteralElement tryCreate;
            if (flag)
            {
                tryCreate = new Int64LiteralElement();
            }
            else if (isHex)
            {
                long value;
                var flag2 = !long.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value);
                if (flag2)
                {
                    tryCreate = null;
                }
                else
                {
                    var flag3 = (value >= 0L) & (value <= 9223372036854775807L);
                    if (flag3)
                    {
                        tryCreate = new Int64LiteralElement(value);
                    }
                    else
                    {
                        tryCreate = null;
                    }
                }
            }
            else
            {
                long value2;
                var flag4 = long.TryParse(image, out value2);
                if (flag4)
                {
                    tryCreate = new Int64LiteralElement(value2);
                }
                else
                {
                    tryCreate = null;
                }
            }
            return tryCreate;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad(this.myValue, ilg);
        }

        public void Negate()
        {
            var myIsMinValue = this.myIsMinValue;
            if (myIsMinValue)
            {
                this.myValue = -9223372036854775808L;
            }
            else
            {
                this.myValue = -this.myValue;
            }
        }

        public override Type ResultType
        {
            get { return typeof(long); }
        }
    }
}