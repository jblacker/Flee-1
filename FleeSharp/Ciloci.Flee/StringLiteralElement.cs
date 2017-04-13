using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class StringLiteralElement : LiteralElement
	{
		private string MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(string);
			}
		}

		public StringLiteralElement(string value)
		{
			this.MyValue = value;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			ilg.Emit(OpCodes.Ldstr, this.MyValue);
		}
	}
}
