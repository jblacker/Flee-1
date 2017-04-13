using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class DecimalLiteralElement : RealLiteralElement
	{
		private static ConstructorInfo OurConstructorInfo = DecimalLiteralElement.GetConstructor();

		private decimal MyValue;

		public override Type ResultType
		{
			get
			{
				return typeof(decimal);
			}
		}

		private DecimalLiteralElement()
		{
		}

		public DecimalLiteralElement(decimal value)
		{
			this.MyValue = value;
		}

		private static ConstructorInfo GetConstructor()
		{
			Type[] types = new Type[]
			{
				typeof(int),
				typeof(int),
				typeof(int),
				typeof(bool),
				typeof(byte)
			};
			return typeof(decimal).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		public static DecimalLiteralElement Parse(string image, IServiceProvider services)
		{
			ExpressionParserOptions options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
			DecimalLiteralElement element = new DecimalLiteralElement();
			DecimalLiteralElement Parse;
			try
			{
				double value = Convert.ToDouble(options.ParseDecimal(image));
				Parse = new DecimalLiteralElement(new decimal(value));
			}
			catch (OverflowException expr_39)
			{
				ProjectData.SetProjectError(expr_39);
				element.OnParseOverflow(image);
				Parse = null;
				ProjectData.ClearProjectError();
			}
			return Parse;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			int index = ilg.GetTempLocalIndex(typeof(decimal));
			Utility.EmitLoadLocalAddress(ilg, index);
			int[] bits = decimal.GetBits(this.MyValue);
			LiteralElement.EmitLoad(bits[0], ilg);
			LiteralElement.EmitLoad(bits[1], ilg);
			LiteralElement.EmitLoad(bits[2], ilg);
			int flags = bits[3];
			LiteralElement.EmitLoad(flags >> 31 == -1, ilg);
			LiteralElement.EmitLoad(flags >> 16, ilg);
			ilg.Emit(OpCodes.Call, DecimalLiteralElement.OurConstructorInfo);
			Utility.EmitLoadLocal(ilg, index);
		}
	}
}
