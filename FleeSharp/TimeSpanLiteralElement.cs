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