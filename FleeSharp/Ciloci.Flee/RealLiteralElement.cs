using System;
using System.Diagnostics;

namespace Ciloci.Flee
{
	internal abstract class RealLiteralElement : LiteralElement
	{
		public static LiteralElement CreateFromInteger(string image, IServiceProvider services)
		{
			LiteralElement element = RealLiteralElement.CreateSingle(image, services);
			bool flag = element != null;
			LiteralElement CreateFromInteger;
			if (flag)
			{
				CreateFromInteger = element;
			}
			else
			{
				element = RealLiteralElement.CreateDecimal(image, services);
				bool flag2 = element != null;
				if (flag2)
				{
					CreateFromInteger = element;
				}
				else
				{
					ExpressionOptions options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
					bool integersAsDoubles = options.IntegersAsDoubles;
					if (integersAsDoubles)
					{
						CreateFromInteger = DoubleLiteralElement.Parse(image, services);
					}
					else
					{
						CreateFromInteger = null;
					}
				}
			}
			return CreateFromInteger;
		}

		public static LiteralElement Create(string image, IServiceProvider services)
		{
			LiteralElement element = RealLiteralElement.CreateSingle(image, services);
			bool flag = element != null;
			LiteralElement Create;
			if (flag)
			{
				Create = element;
			}
			else
			{
				element = RealLiteralElement.CreateDecimal(image, services);
				bool flag2 = element != null;
				if (flag2)
				{
					Create = element;
				}
				else
				{
					element = RealLiteralElement.CreateDouble(image, services);
					bool flag3 = element != null;
					if (flag3)
					{
						Create = element;
					}
					else
					{
						element = RealLiteralElement.CreateImplicitReal(image, services);
						Create = element;
					}
				}
			}
			return Create;
		}

		private static LiteralElement CreateImplicitReal(string image, IServiceProvider services)
		{
			ExpressionOptions options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
			LiteralElement CreateImplicitReal;
			switch (options.RealLiteralDataType)
			{
			case RealLiteralDataType.Single:
				CreateImplicitReal = SingleLiteralElement.Parse(image, services);
				break;
			case RealLiteralDataType.Double:
				CreateImplicitReal = DoubleLiteralElement.Parse(image, services);
				break;
			case RealLiteralDataType.Decimal:
				CreateImplicitReal = DecimalLiteralElement.Parse(image, services);
				break;
			default:
				Debug.Fail("Unknown value");
				CreateImplicitReal = null;
				break;
			}
			return CreateImplicitReal;
		}

		private static DoubleLiteralElement CreateDouble(string image, IServiceProvider services)
		{
			bool flag = image.EndsWith("d", StringComparison.OrdinalIgnoreCase);
			DoubleLiteralElement CreateDouble;
			if (flag)
			{
				image = image.Remove(image.Length - 1);
				CreateDouble = DoubleLiteralElement.Parse(image, services);
			}
			else
			{
				CreateDouble = null;
			}
			return CreateDouble;
		}

		private static SingleLiteralElement CreateSingle(string image, IServiceProvider services)
		{
			bool flag = image.EndsWith("f", StringComparison.OrdinalIgnoreCase);
			SingleLiteralElement CreateSingle;
			if (flag)
			{
				image = image.Remove(image.Length - 1);
				CreateSingle = SingleLiteralElement.Parse(image, services);
			}
			else
			{
				CreateSingle = null;
			}
			return CreateSingle;
		}

		private static DecimalLiteralElement CreateDecimal(string image, IServiceProvider services)
		{
			bool flag = image.EndsWith("m", StringComparison.OrdinalIgnoreCase);
			DecimalLiteralElement CreateDecimal;
			if (flag)
			{
				image = image.Remove(image.Length - 1);
				CreateDecimal = DecimalLiteralElement.Parse(image, services);
			}
			else
			{
				CreateDecimal = null;
			}
			return CreateDecimal;
		}
	}
}
