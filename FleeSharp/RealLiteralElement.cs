// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace Flee
{
    using System;
    using System.Diagnostics;

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
                    var options = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));
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
            var options = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));
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