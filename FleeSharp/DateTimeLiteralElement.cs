namespace Flee
{
    using System;
    using System.Globalization;
    using System.Reflection.Emit;

    internal class DateTimeLiteralElement : LiteralElement
    {
        private DateTime myValue;

        public DateTimeLiteralElement(string image, ExpressionContext context)
        {
            var options = context.ParserOptions;
            var flag =
                !DateTime.TryParseExact(image, options.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out this.myValue);
            if (flag)
            {
                this.ThrowCompileException("CannotParseType", CompileExceptionReason.InvalidFormat, typeof(DateTime).Name);
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            var index = ilg.GetTempLocalIndex(typeof(DateTime));
            Utility.EmitLoadLocalAddress(ilg, index);
            EmitLoad(this.myValue.Ticks, ilg);
            var ci = typeof(DateTime).GetConstructor(new[]
            {
                typeof(long)
            });
            ilg.Emit(OpCodes.Call, ci);
            Utility.EmitLoadLocal(ilg, index);
        }

        public override Type ResultType => typeof(DateTime);
    }
}