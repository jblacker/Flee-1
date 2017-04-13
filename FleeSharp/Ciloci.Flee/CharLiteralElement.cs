using System;

namespace Ciloci.Flee
{
	internal class CharLiteralElement : LiteralElement
	{
		private char MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(char);
			}
		}

		public CharLiteralElement(char value)
		{
			this.MyValue = value;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			int intValue = Convert.ToInt32(this.MyValue);
			LiteralElement.EmitLoad(intValue, ilg);
		}
	}
}
