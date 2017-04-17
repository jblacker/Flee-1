namespace Flee
{
    using System;
    using System.Globalization;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Int64LiteralElement : IntegralLiteralElement
    {
        private const string MinValue = "9223372036854775808";

        private readonly bool myIsMinValue;
        private long myValue;

        public Int64LiteralElement(long value)
        {
            this.myValue = value;
        }

        private Int64LiteralElement()
        {
            this.myIsMinValue = true;
        }

        public static Int64LiteralElement TryCreate(string image, bool isHex, bool negated)
        {
            var flag = negated & (Operators.CompareString(image, "9223372036854775808", false) == 0);
            Int64LiteralElement tryCreate;
            if (flag)
            {
                tryCreate = new Int64LiteralElement();
            }
            else if (isHex)
            {
                long value;
                var flag2 = !long.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value);
                if (flag2)
                {
                    tryCreate = null;
                }
                else
                {
                    var flag3 = (value >= 0L) & (value <= 9223372036854775807L);
                    if (flag3)
                    {
                        tryCreate = new Int64LiteralElement(value);
                    }
                    else
                    {
                        tryCreate = null;
                    }
                }
            }
            else
            {
                long value2;
                var flag4 = long.TryParse(image, out value2);
                if (flag4)
                {
                    tryCreate = new Int64LiteralElement(value2);
                }
                else
                {
                    tryCreate = null;
                }
            }
            return tryCreate;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad(this.myValue, ilg);
        }

        public void Negate()
        {
            var myIsMinValue = this.myIsMinValue;
            if (myIsMinValue)
            {
                this.myValue = -9223372036854775808L;
            }
            else
            {
                this.myValue = -this.myValue;
            }
        }

        public override Type ResultType
        {
            get { return typeof(long); }
        }
    }
}