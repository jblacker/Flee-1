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
    using System.Globalization;

    internal abstract class IntegralLiteralElement : LiteralElement
    {
        public static LiteralElement Create(string image, bool isHex, bool negated, IServiceProvider services)
        {
            var comparison = StringComparison.OrdinalIgnoreCase;
            var flag = !isHex;
            LiteralElement Create;
            if (flag)
            {
                var realElement = RealLiteralElement.CreateFromInteger(image, services);
                var flag2 = realElement != null;
                if (flag2)
                {
                    Create = realElement;
                    return Create;
                }
            }
            var hasUSuffix = image.EndsWith("u", comparison) & !image.EndsWith("lu", comparison);
            var hasLSuffix = image.EndsWith("l", comparison) & !image.EndsWith("ul", comparison);
            var hasULSuffix = image.EndsWith("ul", comparison) | image.EndsWith("lu", comparison);
            var hasSuffix = hasUSuffix | hasLSuffix | hasULSuffix;
            var numStyles = NumberStyles.Integer;
            if (isHex)
            {
                numStyles = NumberStyles.AllowHexSpecifier;
                image = image.Remove(0, 2);
            }
            var flag3 = !hasSuffix;
            if (flag3)
            {
                LiteralElement constant = Int32LiteralElement.TryCreate(image, isHex, negated);
                var flag4 = constant != null;
                if (flag4)
                {
                    Create = constant;
                }
                else
                {
                    constant = UInt32LiteralElement.TryCreate(image, numStyles);
                    var flag5 = constant != null;
                    if (flag5)
                    {
                        Create = constant;
                    }
                    else
                    {
                        constant = Int64LiteralElement.TryCreate(image, isHex, negated);
                        var flag6 = constant != null;
                        if (flag6)
                        {
                            Create = constant;
                        }
                        else
                        {
                            Create = new UInt64LiteralElement(image, numStyles);
                        }
                    }
                }
            }
            else
            {
                var flag7 = hasUSuffix;
                if (flag7)
                {
                    image = image.Remove(image.Length - 1);
                    LiteralElement constant = UInt32LiteralElement.TryCreate(image, numStyles);
                    var flag8 = constant != null;
                    if (flag8)
                    {
                        Create = constant;
                    }
                    else
                    {
                        Create = new UInt64LiteralElement(image, numStyles);
                    }
                }
                else
                {
                    var flag9 = hasLSuffix;
                    if (flag9)
                    {
                        image = image.Remove(image.Length - 1);
                        LiteralElement constant = Int64LiteralElement.TryCreate(image, isHex, negated);
                        var flag10 = constant != null;
                        if (flag10)
                        {
                            Create = constant;
                        }
                        else
                        {
                            Create = new UInt64LiteralElement(image, numStyles);
                        }
                    }
                    else
                    {
                        Debug.Assert(hasULSuffix, "expecting ul suffix");
                        image = image.Remove(image.Length - 2);
                        Create = new UInt64LiteralElement(image, numStyles);
                    }
                }
            }
            return Create;
        }
    }
}