using System;

namespace Flee
{
    internal class BooleanLiteralElement : LiteralElement
    {
        private readonly bool myValue;

        public override Type ResultType => typeof(bool);

        public BooleanLiteralElement(bool value)
        {
            this.myValue = value;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            EmitLoad(this.myValue, ilg);
        }
    }
}