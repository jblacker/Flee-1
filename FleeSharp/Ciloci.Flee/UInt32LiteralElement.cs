using System;
using System.Globalization;

namespace Ciloci.Flee
{
	internal class UInt32LiteralElement : IntegralLiteralElement
	{
		private uint MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(uint);
			}
		}

		public UInt32LiteralElement(uint value)
		{
			this.MyValue = value;
		}

		public static UInt32LiteralElement TryCreate(string image, NumberStyles ns)
		{
			uint value;
			bool flag = uint.TryParse(image, ns, null, out value);
			UInt32LiteralElement TryCreate;
			if (flag)
			{
				TryCreate = new UInt32LiteralElement(value);
			}
			else
			{
				TryCreate = null;
			}
			return TryCreate;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			LiteralElement.EmitLoad((int)this.MyValue, ilg);
		}
	}
}
