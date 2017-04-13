using System;

namespace Ciloci.Flee
{
	internal class BooleanLiteralElement : LiteralElement
	{
		private bool MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(bool);
			}
		}

		public BooleanLiteralElement(bool value)
		{
			this.MyValue = value;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			LiteralElement.EmitLoad(this.MyValue, ilg);
		}
	}
}
