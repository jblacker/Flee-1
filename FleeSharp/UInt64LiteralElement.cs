using System;
using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class UInt64LiteralElement : IntegralLiteralElement
    {
        private ulong MyValue;

        public override Type ResultType
        {
            get
            {
                return typeof(ulong);
            }
        }

        public UInt64LiteralElement(string image, NumberStyles ns)
        {
            try
            {
                this.MyValue = ulong.Parse(image, ns);
            }
            catch (OverflowException expr_18)
            {
                ProjectData.SetProjectError(expr_18);
                this.OnParseOverflow(image);
                ProjectData.ClearProjectError();
            }
        }

        public UInt64LiteralElement(ulong value)
        {
            this.MyValue = value;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            EmitLoad((long)this.MyValue, ilg);
        }
    }
}