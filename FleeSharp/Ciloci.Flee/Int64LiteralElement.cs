using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Globalization;

namespace Ciloci.Flee
{
	internal class Int64LiteralElement : IntegralLiteralElement
	{
		private long MyValue;

		private const string MinValue = "9223372036854775808";

		private bool MyIsMinValue;

		public override Type ResultType
		{
			get
			{
				return typeof(long);
			}
		}

		public Int64LiteralElement(long value)
		{
			this.MyValue = value;
		}

		private Int64LiteralElement()
		{
			this.MyIsMinValue = true;
		}

		public static Int64LiteralElement TryCreate(string image, bool isHex, bool negated)
		{
			bool flag = negated & Operators.CompareString(image, "9223372036854775808", false) == 0;
			Int64LiteralElement TryCreate;
			if (flag)
			{
				TryCreate = new Int64LiteralElement();
			}
			else if (isHex)
			{
				long value;
				bool flag2 = !long.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value);
				if (flag2)
				{
					TryCreate = null;
				}
				else
				{
					bool flag3 = value >= 0L & value <= 9223372036854775807L;
					if (flag3)
					{
						TryCreate = new Int64LiteralElement(value);
					}
					else
					{
						TryCreate = null;
					}
				}
			}
			else
			{
				long value2;
				bool flag4 = long.TryParse(image, out value2);
				if (flag4)
				{
					TryCreate = new Int64LiteralElement(value2);
				}
				else
				{
					TryCreate = null;
				}
			}
			return TryCreate;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			LiteralElement.EmitLoad(this.MyValue, ilg);
		}

		public void Negate()
		{
			bool myIsMinValue = this.MyIsMinValue;
			if (myIsMinValue)
			{
				this.MyValue = -9223372036854775808L;
			}
			else
			{
				this.MyValue = -this.MyValue;
			}
		}
	}
}
