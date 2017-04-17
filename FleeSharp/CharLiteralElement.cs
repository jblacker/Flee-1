using System;

namespace Flee
{
    internal class CharLiteralElement : LiteralElement
    {
        private readonly char myValue;

        public override Type ResultType => typeof(char);

        public CharLiteralElement(char value)
        {
            this.myValue = value;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            int intValue = Convert.ToInt32(this.myValue);
            EmitLoad(intValue, ilg);
        }
    }
}