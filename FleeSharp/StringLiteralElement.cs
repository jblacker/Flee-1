using System;
using System.Reflection.Emit;

namespace Flee
{
    internal class StringLiteralElement : LiteralElement
    {
        private readonly string myValue;

        public override Type ResultType => typeof(string);

        public StringLiteralElement(string value)
        {
            this.myValue = value;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            ilg.Emit(OpCodes.Ldstr, this.myValue);
        }
    }
}