using System;
using System.Diagnostics;
using System.Globalization;

namespace Flee
{
    internal abstract class IntegralLiteralElement : LiteralElement
    {
        public static LiteralElement Create(string image, bool isHex, bool negated, IServiceProvider services)
        {
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            bool flag = !isHex;
            LiteralElement Create;
            if (flag)
            {
                LiteralElement realElement = RealLiteralElement.CreateFromInteger(image, services);
                bool flag2 = realElement != null;
                if (flag2)
                {
                    Create = realElement;
                    return Create;
                }
            }
            bool hasUSuffix = image.EndsWith("u", comparison) & !image.EndsWith("lu", comparison);
            bool hasLSuffix = image.EndsWith("l", comparison) & !image.EndsWith("ul", comparison);
            bool hasULSuffix = image.EndsWith("ul", comparison) | image.EndsWith("lu", comparison);
            bool hasSuffix = hasUSuffix | hasLSuffix | hasULSuffix;
            NumberStyles numStyles = NumberStyles.Integer;
            if (isHex)
            {
                numStyles = NumberStyles.AllowHexSpecifier;
                image = image.Remove(0, 2);
            }
            bool flag3 = !hasSuffix;
            if (flag3)
            {
                LiteralElement constant = Int32LiteralElement.TryCreate(image, isHex, negated);
                bool flag4 = constant != null;
                if (flag4)
                {
                    Create = constant;
                }
                else
                {
                    constant = UInt32LiteralElement.TryCreate(image, numStyles);
                    bool flag5 = constant != null;
                    if (flag5)
                    {
                        Create = constant;
                    }
                    else
                    {
                        constant = Int64LiteralElement.TryCreate(image, isHex, negated);
                        bool flag6 = constant != null;
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
                bool flag7 = hasUSuffix;
                if (flag7)
                {
                    image = image.Remove(image.Length - 1);
                    LiteralElement constant = UInt32LiteralElement.TryCreate(image, numStyles);
                    bool flag8 = constant != null;
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
                    bool flag9 = hasLSuffix;
                    if (flag9)
                    {
                        image = image.Remove(image.Length - 1);
                        LiteralElement constant = Int64LiteralElement.TryCreate(image, isHex, negated);
                        bool flag10 = constant != null;
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