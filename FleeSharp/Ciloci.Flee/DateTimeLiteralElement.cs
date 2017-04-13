using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class DateTimeLiteralElement : LiteralElement
	{
		private DateTime MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(DateTime);
			}
		}

		public DateTimeLiteralElement(string image, ExpressionContext context)
		{
			ExpressionParserOptions options = context.ParserOptions;
			bool flag = !DateTime.TryParseExact(image, options.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out this.MyValue);
			if (flag)
			{
				base.ThrowCompileException("CannotParseType", CompileExceptionReason.InvalidFormat, new object[]
				{
					typeof(DateTime).Name
				});
			}
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			int index = ilg.GetTempLocalIndex(typeof(DateTime));
			Utility.EmitLoadLocalAddress(ilg, index);
			LiteralElement.EmitLoad(this.MyValue.Ticks, ilg);
			ConstructorInfo ci = typeof(DateTime).GetConstructor(new Type[]
			{
				typeof(long)
			});
			ilg.Emit(OpCodes.Call, ci);
			Utility.EmitLoadLocal(ilg, index);
		}
	}
}
