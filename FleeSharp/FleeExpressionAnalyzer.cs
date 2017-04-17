using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    using System.Linq;
    using Extensions;
    using PerCederberg.Grammatica.Runtime;

    internal class FleeExpressionAnalyzer : ExpressionAnalyzer
    {
        private IServiceProvider myServices;

        private Regex myUnicodeEscapeRegex;

        private Regex myRegularEscapeRegex;

        private bool myInUnaryNegate;

        private ExpressionContext Context => (ExpressionContext)this.myServices.GetService(typeof(ExpressionContext));

        internal FleeExpressionAnalyzer()
        {
        }

        public void SetServices(IServiceProvider services)
        {
            this.myServices = services;
            this.myUnicodeEscapeRegex = new Regex("\\\\u[0-9a-f]{4}", RegexOptions.IgnoreCase);
            this.myRegularEscapeRegex = new Regex(string.Format("\\\\[\\\\{0}'trn]", this.Context.ParserOptions.StringQuote), RegexOptions.IgnoreCase);
        }

        public void Reset()
        {
            this.myServices = null;
        }

        public override Node ExitExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        public override Node ExitExpressionGroup(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        public override Node ExitXorExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(XorElement));
            return node;
        }

        public override Node ExitOrExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(AndOrElement));
            return node;
        }

        public override Node ExitAndExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(AndOrElement));
            return node;
        }

        public override Node ExitNotExpression(Production node)
        {
            this.AddUnaryOp(node, typeof(NotElement));
            return node;
        }

        public override Node ExitCompareExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(CompareElement));
            return node;
        }

        public override Node ExitShiftExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(ShiftElement));
            return node;
        }

        public override Node ExitAdditiveExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(ArithmeticElement));
            return node;
        }

        public override Node ExitMultiplicativeExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(ArithmeticElement));
            return node;
        }

        public override Node ExitPowerExpression(Production node)
        {
            this.AddBinaryOp(node, typeof(ArithmeticElement));
            return node;
        }

        public override Node ExitNegateExpression(Production node)
        {
            IList childValues = this.GetChildValues(node);
            var childElement = (ExpressionElement)childValues[childValues.Count - 1];
            var flag = childElement.GetType() == typeof(Int32LiteralElement) & childValues.Count == 2;
            if (flag)
            {
                ((Int32LiteralElement)childElement).Negate();
                node.AddValue(childElement);
            }
            else
            {
                var flag2 = childElement.GetType() == typeof(Int64LiteralElement) & childValues.Count == 2;
                if (flag2)
                {
                    ((Int64LiteralElement)childElement).Negate();
                    node.AddValue(childElement);
                }
                else
                {
                    this.AddUnaryOp(node, typeof(NegateElement));
                }
            }
            return node;
        }

        public override Node ExitMemberExpression(Production node)
        {
            var childValues = this.GetChildValues(node);
            var first = RuntimeHelpers.GetObjectValue(childValues[0]);
            var flag = childValues.Count == 1 && !(first is MemberElement);

            if (flag)
            {
                node.AddValue(RuntimeHelpers.GetObjectValue(first));
            }
            else
            {
                var list = new InvocationListElement(childValues, this.myServices);
                node.AddValue(list);
            }
            return node;
        }

        public override Node ExitIndexExpression(Production node)
        {
            IList childValues = this.GetChildValues(node);
            var args = new ArgumentList(childValues);
            var e = new IndexerElement(args);
            node.AddValue(e);
            return node;
        }

        public override Node ExitMemberAccessExpression(Production node)
        {
            node.AddValue(RuntimeHelpers.GetObjectValue(node.GetChildAt(1).GetValue(0)));
            return node;
        }

        public override Node ExitSpecialFunctionExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        public override Node ExitIfExpression(Production node)
        {
            IList childValues = this.GetChildValues(node);
            var op = new ConditionalElement((ExpressionElement)childValues[0], (ExpressionElement)childValues[1], (ExpressionElement)childValues[2]);
            node.AddValue(op);
            return node;
        }

        public override Node ExitInExpression(Production node)
        {
            var childValues = this.GetChildValues(node);
            var flag = childValues.Count == 1;
            Node exitInExpression;
            if (flag)
            {
                this.AddFirstChildValue(node);
                exitInExpression = node;
            }
            else
            {
                var operand = (ExpressionElement)childValues[0];
                childValues.RemoveAt(0);
                var second = RuntimeHelpers.GetObjectValue(childValues[0]);
                var flag2 = second is IList;
                InElement op;
                if (flag2)
                {
                    op = new InElement(operand, (IList)second);
                }
                else
                {
                    var il = new InvocationListElement(childValues, this.myServices);
                    op = new InElement(operand, il);
                }
                node.AddValue(op);
                exitInExpression = node;
            }
            return exitInExpression;
        }

        public override Node ExitInTargetExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        public override Node ExitInListTargetExpression(Production node)
        {
            IList childValues = this.GetChildValues(node);
            node.AddValue(childValues);
            return node;
        }

        public override Node ExitCastExpression(Production node)
        {
            IList childValues = this.GetChildValues(node);
            var destTypeParts = (string[])childValues[1];
            var isArray = (bool)childValues[2];
            var op = new CastElement((ExpressionElement)childValues[0], destTypeParts, isArray, this.myServices);
            node.AddValue(op);
            return node;
        }

        public override Node ExitCastTypeExpression(Production node)
        {
            var childValues = this.GetChildValues(node);
            var parts = childValues.Map(Conversions.ToString).ToList();
            //try
            //{
            //    IEnumerator enumerator = childValues.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        string part = Conversions.ToString(enumerator.Current);
            //        parts.Add(part);
            //    }
            //}
            //finally
            //{
            //    IEnumerator enumerator;
            //    if (enumerator is IDisposable)
            //    {
            //        (enumerator as IDisposable).Dispose();
            //    }
            //}
            var isArray = false;
            var flag = Operators.CompareString(parts[parts.Count - 1], "[]", false) == 0;
            if (flag)
            {
                isArray = true;
                parts.RemoveAt(parts.Count - 1);
            }
            node.AddValue(parts.ToArray());
            node.AddValue(isArray);
            return node;
        }

        public override Node ExitMemberFunctionExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        public override Node ExitFieldPropertyExpression(Production node)
        {
            var name = (string)node.GetChildAt(0).GetValue(0);
            var elem = new IdentifierElement(name);
            node.AddValue(elem);
            return node;
        }

        public override Node ExitFunctionCallExpression(Production node)
        {
            IList childValues = this.GetChildValues(node);
            var name = (string)childValues[0];
            childValues.RemoveAt(0);
            var args = new ArgumentList(childValues);
            var funcCall = new FunctionCallElement(name, args);
            node.AddValue(funcCall);
            return node;
        }

        public override Node ExitArgumentList(Production node)
        {
            var childValues = this.GetChildValues(node);
            node.AddValues(childValues);
            return node;
        }

        public override Node ExitBasicExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        public override Node ExitLiteralExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        private void AddFirstChildValue(Production node)
        {
            node.AddValue(RuntimeHelpers.GetObjectValue(this.GetChildAt(node, 0).ValuesArrayList[0]));
        }

        private void AddUnaryOp(Production node, Type elementType)
        {
            IList childValues = this.GetChildValues(node);
            var flag = childValues.Count == 2;
            if (flag)
            {
                var element = (UnaryElement)Activator.CreateInstance(elementType);
                element.SetChild((ExpressionElement)childValues[1]);
                node.AddValue(element);
            }
            else
            {
                node.AddValue(RuntimeHelpers.GetObjectValue(childValues[0]));
            }
        }

        private void AddBinaryOp(Production node, Type elementType)
        {
            IList childValues = this.GetChildValues(node);
            var flag = childValues.Count > 1;
            if (flag)
            {
                var e = BinaryExpressionElement.CreateElement(childValues, elementType);
                node.AddValue(e);
            }
            else
            {
                var flag2 = childValues.Count == 1;
                if (flag2)
                {
                    node.AddValue(RuntimeHelpers.GetObjectValue(childValues[0]));
                }
                else
                {
                    Debug.Assert(false, "wrong number of chilren");
                }
            }
        }

        public override Node ExitReal(Token node)
        {
            var image = node.Image;
            var element = RealLiteralElement.Create(image, this.myServices);
            node.AddValue(element);
            return node;
        }

        public override Node ExitInteger(Token node)
        {
            var element = IntegralLiteralElement.Create(node.Image, false, this.myInUnaryNegate, this.myServices);
            node.AddValue(element);
            return node;
        }

        public override Node ExitHexLiteral(Token node)
        {
            var element = IntegralLiteralElement.Create(node.Image, true, this.myInUnaryNegate, this.myServices);
            node.AddValue(element);
            return node;
        }

        public override Node ExitBooleanLiteralExpression(Production node)
        {
            this.AddFirstChildValue(node);
            return node;
        }

        public override Node ExitTrue(Token node)
        {
            node.AddValue(new BooleanLiteralElement(true));
            return node;
        }

        public override Node ExitFalse(Token node)
        {
            node.AddValue(new BooleanLiteralElement(false));
            return node;
        }

        public override Node ExitStringLiteral(Token node)
        {
            var s = this.DoEscapes(node.Image);
            var element = new StringLiteralElement(s);
            node.AddValue(element);
            return node;
        }

        public override Node ExitCharLiteral(Token node)
        {
            var s = this.DoEscapes(node.Image);
            node.AddValue(new CharLiteralElement(s[0]));
            return node;
        }

        public override Node ExitDatetime(Token node)
        {
            var context = (ExpressionContext)this.myServices.GetService(typeof(ExpressionContext));
            var image = node.Image.Substring(1, node.Image.Length - 2);
            var element = new DateTimeLiteralElement(image, context);
            node.AddValue(element);
            return node;
        }

        public override Node ExitTimespan(Token node)
        {
            var image = node.Image.Substring(2, node.Image.Length - 3);
            var element = new TimeSpanLiteralElement(image);
            node.AddValue(element);
            return node;
        }

        private string DoEscapes(string image)
        {
            image = image.Substring(1, image.Length - 2);
            image = this.myUnicodeEscapeRegex.Replace(image, new MatchEvaluator(this.UnicodeEscapeMatcher));
            image = this.myRegularEscapeRegex.Replace(image, new MatchEvaluator(this.RegularEscapeMatcher));
            return image;
        }

        private string RegularEscapeMatcher(Match m)
        {
            var s = m.Value;
            s = s.Remove(0, 1);
            var left = s;
            var flag = Operators.CompareString(left, "\\", false) == 0 || Operators.CompareString(left, Conversions.ToString(this.Context.ParserOptions.StringQuote), false) == 0 || Operators.CompareString(left, "'", false) == 0;
            string regularEscapeMatcher;
            if (flag)
            {
                regularEscapeMatcher = s;
            }
            else
            {
                flag = (Operators.CompareString(left, "t", false) == 0 || Operators.CompareString(left, "T", false) == 0);
                if (flag)
                {
                    regularEscapeMatcher = Convert.ToChar(9).ToString();
                }
                else
                {
                    flag = (Operators.CompareString(left, "n", false) == 0 || Operators.CompareString(left, "N", false) == 0);
                    if (flag)
                    {
                        regularEscapeMatcher = Convert.ToChar(10).ToString();
                    }
                    else
                    {
                        flag = (Operators.CompareString(left, "r", false) == 0 || Operators.CompareString(left, "R", false) == 0);
                        if (flag)
                        {
                            regularEscapeMatcher = Convert.ToChar(13).ToString();
                        }
                        else
                        {
                            Debug.Assert(false, "Unrecognized escape sequence");
                            regularEscapeMatcher = null;
                        }
                    }
                }
            }
            return regularEscapeMatcher;
        }

        private string UnicodeEscapeMatcher(Match m)
        {
            var s = m.Value;
            s = s.Remove(0, 2);
            var code = int.Parse(s, NumberStyles.AllowHexSpecifier);
            return Convert.ToChar(code).ToString();
        }

        public override Node ExitIdentifier(Token node)
        {
            node.AddValue(node.Image);
            return node;
        }

        public override Node ExitNullLiteral(Token node)
        {
            node.AddValue(new NullLiteralElement());
            return node;
        }

        public override Node ExitArrayBraces(Token node)
        {
            node.AddValue("[]");
            return node;
        }

        public override Node ExitAdd(Token node)
        {
            node.AddValue(BinaryArithmeticOperation.Add);
            return node;
        }

        public override Node ExitSub(Token node)
        {
            node.AddValue(BinaryArithmeticOperation.Subtract);
            return node;
        }

        public override Node ExitMul(Token node)
        {
            node.AddValue(BinaryArithmeticOperation.Multiply);
            return node;
        }

        public override Node ExitDiv(Token node)
        {
            node.AddValue(BinaryArithmeticOperation.Divide);
            return node;
        }

        public override Node ExitMod(Token node)
        {
            node.AddValue(BinaryArithmeticOperation.Mod);
            return node;
        }

        public override Node ExitPower(Token node)
        {
            node.AddValue(BinaryArithmeticOperation.Power);
            return node;
        }

        public override Node ExitEq(Token node)
        {
            node.AddValue(LogicalCompareOperation.Equal);
            return node;
        }

        public override Node ExitNe(Token node)
        {
            node.AddValue(LogicalCompareOperation.NotEqual);
            return node;
        }

        public override Node ExitLt(Token node)
        {
            node.AddValue(LogicalCompareOperation.LessThan);
            return node;
        }

        public override Node ExitGt(Token node)
        {
            node.AddValue(LogicalCompareOperation.GreaterThan);
            return node;
        }

        public override Node ExitLte(Token node)
        {
            node.AddValue(LogicalCompareOperation.LessThanOrEqual);
            return node;
        }

        public override Node ExitGte(Token node)
        {
            node.AddValue(LogicalCompareOperation.GreaterThanOrEqual);
            return node;
        }

        public override Node ExitAnd(Token node)
        {
            node.AddValue(AndOrOperation.And);
            return node;
        }

        public override Node ExitOr(Token node)
        {
            node.AddValue(AndOrOperation.Or);
            return node;
        }

        public override Node ExitXor(Token node)
        {
            node.AddValue("Xor");
            return node;
        }

        public override Node ExitNot(Token node)
        {
            node.AddValue(string.Empty);
            return node;
        }

        public override Node ExitLeftShift(Token node)
        {
            node.AddValue(ShiftOperation.LeftShift);
            return node;
        }

        public override Node ExitRightShift(Token node)
        {
            node.AddValue(ShiftOperation.RightShift);
            return node;
        }

        public override void Child(Production node, Node child)
        {
            base.Child(node, child);
            this.myInUnaryNegate = (node.Id == 2014 & child.Id == 1002);
        }
    }
}