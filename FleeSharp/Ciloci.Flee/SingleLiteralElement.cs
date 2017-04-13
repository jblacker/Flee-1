using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class SingleLiteralElement : RealLiteralElement
	{
		private float MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(float);
			}
		}

		private SingleLiteralElement()
		{
		}

		public SingleLiteralElement(float value)
		{
			this.MyValue = value;
		}

		public static SingleLiteralElement Parse(string image, IServiceProvider services)
		{
			ExpressionParserOptions options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
			SingleLiteralElement element = new SingleLiteralElement();
			SingleLiteralElement Parse;
			try
			{
				float value = options.ParseSingle(image);
				Parse = new SingleLiteralElement(value);
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
			ilg.Emit(OpCodes.Ldc_R4, this.MyValue);
		}
	}
}
