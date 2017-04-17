using System;
using System.Reflection.Emit;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class DoubleLiteralElement : RealLiteralElement
    {
        private readonly double myValue;

        public override Type ResultType => typeof(double);

        private DoubleLiteralElement()
        {
        }

        public DoubleLiteralElement(double value)
        {
            this.myValue = value;
        }

        public static DoubleLiteralElement Parse(string image, IServiceProvider services)
        {
            var options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
            var element = new DoubleLiteralElement();
            DoubleLiteralElement parse;
            try
            {
                var value = options.ParseDouble(image);
                parse = new DoubleLiteralElement(value);
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
            ilg.Emit(OpCodes.Ldc_R8, this.myValue);
        }
    }
}