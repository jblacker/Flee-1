using System;
using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class Int32LiteralElement : IntegralLiteralElement
    {
        private int MyValue;

        private const string MinValue = "2147483648";

        private bool MyIsMinValue;

        public override Type ResultType
        {
            get
            {
                return typeof(int);
            }
        }

        public int Value
        {
            get
            {
                return this.MyValue;
            }
        }

        public Int32LiteralElement(int value)
        {
            this.MyValue = value;
        }

        private Int32LiteralElement()
        {
            this.MyIsMinValue = true;
        }

        public static Int32LiteralElement TryCreate(string image, bool isHex, bool negated)
        {
            bool flag = negated & Operators.CompareString(image, "2147483648", false) == 0;
            Int32LiteralElement TryCreate;
            if (flag)
            {
                TryCreate = new Int32LiteralElement();
            }
            else if (isHex)
            {
                int value;
                bool flag2 = !int.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value);
                if (flag2)
                {
                    TryCreate = null;
                }
                else
                {
                    bool flag3 = value >= 0 & value <= 2147483647;
                    if (flag3)
                    {
                        TryCreate = new Int32LiteralElement(value);
                    }
                    else
                    {
                        TryCreate = null;
                    }
                }
            }
            else
            {
                int value2;
                bool flag4 = int.TryParse(image, out value2);
                if (flag4)
                {
                    TryCreate = new Int32LiteralElement(value2);
                }
                else
                {
                    TryCreate = null;
                }
            }
            return TryCreate;
        }

        public void Negate()
        {
            bool myIsMinValue = this.MyIsMinValue;
            if (myIsMinValue)
            {
                this.MyValue = -2147483648;
            }
            else
            {
                this.MyValue = -this.MyValue;
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            EmitLoad(this.MyValue, ilg);
        }
    }
}