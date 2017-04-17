using System;
using System.Diagnostics;

namespace Flee
{
    internal abstract class RealLiteralElement : LiteralElement
    {
        public static LiteralElement CreateFromInteger(string image, IServiceProvider services)
        {
            LiteralElement element = CreateSingle(image, services);
            var flag = element != null;
            LiteralElement createFromInteger;
            if (flag)
            {
                createFromInteger = element;
            }
            else
            {
                element = CreateDecimal(image, services);
                var flag2 = element != null;
                if (flag2)
                {
                    createFromInteger = element;
                }
                else
                {
                    var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
                    var integersAsDoubles = options.IntegersAsDoubles;
                    createFromInteger = integersAsDoubles ? DoubleLiteralElement.Parse(image, services) : null;
                }
            }
            return createFromInteger;
        }

        public static LiteralElement Create(string image, IServiceProvider services)
        {
            LiteralElement element = CreateSingle(image, services);
            var flag = element != null;
            LiteralElement create;
            if (flag)
            {
                create = element;
            }
            else
            {
                element = CreateDecimal(image, services);
                var flag2 = element != null;
                if (flag2)
                {
                    create = element;
                }
                else
                {
                    element = CreateDouble(image, services);
                    var flag3 = element != null;
                    if (flag3)
                    {
                        create = element;
                    }
                    else
                    {
                        element = CreateImplicitReal(image, services);
                        create = element;
                    }
                }
            }
            return create;
        }

        private static LiteralElement CreateImplicitReal(string image, IServiceProvider services)
        {
            var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
            LiteralElement createImplicitReal = null;
            switch (options.RealLiteralDataType)
            {
                case RealLiteralDataType.Single:
                    createImplicitReal = SingleLiteralElement.Parse(image, services);
                    break;
                case RealLiteralDataType.Double:
                    createImplicitReal = DoubleLiteralElement.Parse(image, services);
                    break;
                case RealLiteralDataType.Decimal:
                    createImplicitReal = DecimalLiteralElement.Parse(image, services);
                    break;
                default:
                    Debug.Fail("Unknown value");
                    break;
            }
            return createImplicitReal;
        }

        private static DoubleLiteralElement CreateDouble(string image, IServiceProvider services)
        {
            var flag = image.EndsWith("d", StringComparison.OrdinalIgnoreCase);
            DoubleLiteralElement createDouble;
            if (flag)
            {
                image = image.Remove(image.Length - 1);
                createDouble = DoubleLiteralElement.Parse(image, services);
            }
            else
            {
                createDouble = null;
            }
            return createDouble;
        }

        private static SingleLiteralElement CreateSingle(string image, IServiceProvider services)
        {
            var flag = image.EndsWith("f", StringComparison.OrdinalIgnoreCase);
            SingleLiteralElement createSingle;
            if (flag)
            {
                image = image.Remove(image.Length - 1);
                createSingle = SingleLiteralElement.Parse(image, services);
            }
            else
            {
                createSingle = null;
            }
            return createSingle;
        }

        private static DecimalLiteralElement CreateDecimal(string image, IServiceProvider services)
        {
            var flag = image.EndsWith("m", StringComparison.OrdinalIgnoreCase);
            DecimalLiteralElement createDecimal;
            if (flag)
            {
                image = image.Remove(image.Length - 1);
                createDecimal = DecimalLiteralElement.Parse(image, services);
            }
            else
            {
                createDecimal = null;
            }
            return createDecimal;
        }
    }
}