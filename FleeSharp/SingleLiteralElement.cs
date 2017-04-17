using System;
using System.Reflection.Emit;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class SingleLiteralElement : RealLiteralElement
    {
        private readonly float myValue;

        public override Type ResultType => typeof(float);

        private SingleLiteralElement()
        {
        }

        public SingleLiteralElement(float value)
        {
            this.myValue = value;
        }

        public static SingleLiteralElement Parse(string image, IServiceProvider services)
        {
            var options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
            var element = new SingleLiteralElement();
            SingleLiteralElement parse;
            try
            {
                var value = options.ParseSingle(image);
                parse = new SingleLiteralElement(value);
            }
            catch (OverflowException expr_2F)
            {
                ProjectData.SetProjectError(expr_2F);
                element.OnParseOverflow(image);
                parse = null;
                ProjectData.ClearProjectError();
            }
            return parse;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            ilg.Emit(OpCodes.Ldc_R4, this.myValue);
        }
    }
}