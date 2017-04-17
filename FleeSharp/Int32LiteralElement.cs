using System;
using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    internal class Int32LiteralElement : IntegralLiteralElement
    {
        private int myValue;

        private const string MinValue = "2147483648";

        private readonly bool myIsMinValue;

        public override Type ResultType => typeof(int);

        public int Value => this.myValue;

        public Int32LiteralElement(int value)
        {
            this.myValue = value;
        }

        private Int32LiteralElement()
        {
            this.myIsMinValue = true;
        }

        public static Int32LiteralElement TryCreate(string image, bool isHex, bool negated)
        {
            bool flag = negated & Operators.CompareString(image, "2147483648", false) == 0;
            Int32LiteralElement tryCreate;
            if (flag)
            {
                tryCreate = new Int32LiteralElement();
            }
            else if (isHex)
            {
                int value;
                bool flag2 = !int.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value);
                if (flag2)
                {
                    tryCreate = null;
                }
                else
                {
                    bool flag3 = value >= 0 & true;
                    if (flag3)
                    {
                        tryCreate = new Int32LiteralElement(value);
                    }
                    else
                    {
                        tryCreate = null;
                    }
                }
            }
            else
            {
                int value2;
                bool flag4 = int.TryParse(image, out value2);
                if (flag4)
                {
                    tryCreate = new Int32LiteralElement(value2);
                }
                else
                {
                    tryCreate = null;
                }
            }
            return tryCreate;
        }

        public void Negate()
        {
            bool myIsMinValue = this.myIsMinValue;
            if (myIsMinValue)
            {
                this.myValue = -2147483648;
            }
            else
            {
                this.myValue = -this.myValue;
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            EmitLoad(this.myValue, ilg);
        }
    }
}