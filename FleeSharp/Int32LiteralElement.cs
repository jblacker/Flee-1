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
    using System.Globalization;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Int32LiteralElement : IntegralLiteralElement
    {
        private const string MinValue = "2147483648";

        private readonly bool myIsMinValue;

        public Int32LiteralElement(int value)
        {
            this.Value = value;
        }

        private Int32LiteralElement()
        {
            this.myIsMinValue = true;
        }

        public static Int32LiteralElement TryCreate(string image, bool isHex, bool negated)
        {
            var flag = negated & (Operators.CompareString(image, "2147483648", false) == 0);
            Int32LiteralElement tryCreate;
            if (flag)
            {
                tryCreate = new Int32LiteralElement();
            }
            else if (isHex)
            {
                int value;
                var flag2 = !int.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value);
                if (flag2)
                {
                    tryCreate = null;
                }
                else
                {
                    var flag3 = (value >= 0) & true;
                    if (flag3)
                    {
                        tryCreate = new Int32LiteralElement(value);
                    }
                    else
                    {
                        tryCreate = null;
                    }
                }
            }
            else
            {
                int value2;
                var flag4 = int.TryParse(image, out value2);
                if (flag4)
                {
                    tryCreate = new Int32LiteralElement(value2);
                }
                else
                {
                    tryCreate = null;
                }
            }
            return tryCreate;
        }

        public void Negate()
        {
            var myIsMinValue = this.myIsMinValue;
            if (myIsMinValue)
            {
                this.Value = -2147483648;
            }
            else
            {
                this.Value = -this.Value;
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad(this.Value, ilg);
        }

        public override Type ResultType => typeof(int);

        public int Value { get; private set; }
    }
}