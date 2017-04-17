using System;
using System.Globalization;

namespace Flee
{
    internal class UInt32LiteralElement : IntegralLiteralElement
    {
        private readonly uint myValue;

        public override Type ResultType => typeof(uint);

        public UInt32LiteralElement(uint value)
        {
            this.myValue = value;
        }

        public static UInt32LiteralElement TryCreate(string image, NumberStyles ns)
        {
            uint value;
            bool flag = uint.TryParse(image, ns, null, out value);
            var tryCreate = flag ? new UInt32LiteralElement(value) : null;
            return tryCreate;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad((int)this.myValue, ilg);
        }
    }
}