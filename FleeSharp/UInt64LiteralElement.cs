using System;
using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class UInt64LiteralElement : IntegralLiteralElement
    {
        private readonly ulong myValue;

        public override Type ResultType => typeof(ulong);

        public UInt64LiteralElement(string image, NumberStyles ns)
        {
            try
            {
                this.myValue = ulong.Parse(image, ns);
            }
            catch (OverflowException overflowException)
            {
                ProjectData.SetProjectError(overflowException);
                this.OnParseOverflow(image);
                ProjectData.ClearProjectError();
            }
        }

        public UInt64LiteralElement(ulong value)
        {
            this.myValue = value;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad((long)this.myValue, ilg);
        }
    }
}