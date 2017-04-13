using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class TimeSpanLiteralElement : LiteralElement
	{
		private TimeSpan MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(TimeSpan);
			}
		}

		public TimeSpanLiteralElement(string image)
		{
			bool flag = !TimeSpan.TryParse(image, out this.MyValue);
			if (flag)
			{
				base.ThrowCompileException("CannotParseType", CompileExceptionReason.InvalidFormat, new object[]
				{
					typeof(TimeSpan).Name
				});
			}
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			int index = ilg.GetTempLocalIndex(typeof(TimeSpan));
			Utility.EmitLoadLocalAddress(ilg, index);
			LiteralElement.EmitLoad(this.MyValue.Ticks, ilg);
			ConstructorInfo ci = typeof(TimeSpan).GetConstructor(new Type[]
			{
				typeof(long)
			});
			ilg.Emit(OpCodes.Call, ci);
			Utility.EmitLoadLocal(ilg, index);
		}
	}
}
