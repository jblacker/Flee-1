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
    using System.Reflection;
    using System.Reflection.Emit;
    using Microsoft.VisualBasic.CompilerServices;

    internal class DecimalLiteralElement : RealLiteralElement
    {
        private static readonly ConstructorInfo ourConstructorInfo = GetConstructor();

        private readonly decimal myValue;

        private DecimalLiteralElement()
        {
        }

        public DecimalLiteralElement(decimal value)
        {
            this.myValue = value;
        }

        private static ConstructorInfo GetConstructor()
        {
            var types = new[]
            {
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(bool),
                typeof(byte)
            };
            return typeof(decimal).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, types,
                null);
        }

        public static DecimalLiteralElement Parse(string image, IServiceProvider services)
        {
            var options = (ExpressionParserOptions) services.GetService(typeof(ExpressionParserOptions));
            var element = new DecimalLiteralElement();
            DecimalLiteralElement parse;
            try
            {
                var value = Convert.ToDouble(options.ParseDecimal(image));
                parse = new DecimalLiteralElement(new decimal(value));
            }
            catch (OverflowException expr39)
            {
                ProjectData.SetProjectError(expr39);
                element.OnParseOverflow(image);
                parse = null;
                ProjectData.ClearProjectError();
            }
            return parse;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var index = ilg.GetTempLocalIndex(typeof(decimal));
            Utility.EmitLoadLocalAddress(ilg, index);
            var bits = decimal.GetBits(this.myValue);
            EmitLoad(bits[0], ilg);
            EmitLoad(bits[1], ilg);
            EmitLoad(bits[2], ilg);
            var flags = bits[3];
            EmitLoad(flags >> 31 == -1, ilg);
            EmitLoad(flags >> 16, ilg);
            ilg.Emit(OpCodes.Call, ourConstructorInfo);
            Utility.EmitLoadLocal(ilg, index);
        }

        public override Type ResultType => typeof(decimal);
    }
}