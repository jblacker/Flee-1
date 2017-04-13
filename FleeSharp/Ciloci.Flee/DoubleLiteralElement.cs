using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class DoubleLiteralElement : RealLiteralElement
	{
		private double MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(double);
			}
		}

		private DoubleLiteralElement()
		{
		}

		public DoubleLiteralElement(double value)
		{
			this.MyValue = value;
		}

		public static DoubleLiteralElement Parse(string image, IServiceProvider services)
		{
			ExpressionParserOptions options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
			DoubleLiteralElement element = new DoubleLiteralElement();
			DoubleLiteralElement Parse;
			try
			{
				double value = options.ParseDouble(image);
				Parse = new DoubleLiteralElement(value);
			}
			catch (OverflowException expr_2F)
			{
				ProjectData.SetProjectError(expr_2F);
				element.OnParseOverflow(image);
				Parse = null;
				ProjectData.ClearProjectError();
			}
			return Parse;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			ilg.Emit(OpCodes.Ldc_R8, this.MyValue);
		}
	}
}
