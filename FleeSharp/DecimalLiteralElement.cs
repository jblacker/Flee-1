using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class DecimalLiteralElement : RealLiteralElement
    {
        private static readonly ConstructorInfo ourConstructorInfo = GetConstructor();

        private readonly decimal myValue;

        public override Type ResultType => typeof(decimal);

        private DecimalLiteralElement()
        {
        }

        public DecimalLiteralElement(decimal value)
        {
            this.myValue = value;
        }

        private static ConstructorInfo GetConstructor()
        {
            var types = new Type[]
            {
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(bool),
                typeof(byte)
            };
            return typeof(decimal).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, types, null);
        }

        public static DecimalLiteralElement Parse(string image, IServiceProvider services)
        {
            var options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
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
    }
}