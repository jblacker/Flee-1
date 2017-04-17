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
    using PerCederberg.Grammatica.Runtime;

    internal abstract class ExpressionAnalyzer : Analyzer
    {
        public override void Enter(Node node)
        {
            var id = node.Id;
            switch (id)
            {
                case 1001:
                    this.EnterAdd((Token) node);
                    break;
                case 1002:
                    this.EnterSub((Token) node);
                    break;
                case 1003:
                    this.EnterMul((Token) node);
                    break;
                case 1004:
                    this.EnterDiv((Token) node);
                    break;
                case 1005:
                    this.EnterPower((Token) node);
                    break;
                case 1006:
                    this.EnterMod((Token) node);
                    break;
                case 1007:
                    this.EnterLeftParen((Token) node);
                    break;
                case 1008:
                    this.EnterRightParen((Token) node);
                    break;
                case 1009:
                    this.EnterLeftBrace((Token) node);
                    break;
                case 1010:
                    this.EnterRightBrace((Token) node);
                    break;
                case 1011:
                    this.EnterEq((Token) node);
                    break;
                case 1012:
                    this.EnterLt((Token) node);
                    break;
                case 1013:
                    this.EnterGt((Token) node);
                    break;
                case 1014:
                    this.EnterLte((Token) node);
                    break;
                case 1015:
                    this.EnterGte((Token) node);
                    break;
                case 1016:
                    this.EnterNe((Token) node);
                    break;
                case 1017:
                    this.EnterAnd((Token) node);
                    break;
                case 1018:
                    this.EnterOr((Token) node);
                    break;
                case 1019:
                    this.EnterXor((Token) node);
                    break;
                case 1020:
                    this.EnterNot((Token) node);
                    break;
                case 1021:
                    this.EnterIn((Token) node);
                    break;
                case 1022:
                    this.EnterDot((Token) node);
                    break;
                case 1023:
                    this.EnterArgumentSeparator((Token) node);
                    break;
                case 1024:
                    this.EnterArrayBraces((Token) node);
                    break;
                case 1025:
                    this.EnterLeftShift((Token) node);
                    break;
                case 1026:
                    this.EnterRightShift((Token) node);
                    break;
                case 1027:
                    break;
                case 1028:
                    this.EnterInteger((Token) node);
                    break;
                case 1029:
                    this.EnterReal((Token) node);
                    break;
                case 1030:
                    this.EnterStringLiteral((Token) node);
                    break;
                case 1031:
                    this.EnterCharLiteral((Token) node);
                    break;
                case 1032:
                    this.EnterTrue((Token) node);
                    break;
                case 1033:
                    this.EnterFalse((Token) node);
                    break;
                case 1034:
                    this.EnterIdentifier((Token) node);
                    break;
                case 1035:
                    this.EnterHexLiteral((Token) node);
                    break;
                case 1036:
                    this.EnterNullLiteral((Token) node);
                    break;
                case 1037:
                    this.EnterTimespan((Token) node);
                    break;
                case 1038:
                    this.EnterDatetime((Token) node);
                    break;
                case 1039:
                    this.EnterIf((Token) node);
                    break;
                case 1040:
                    this.EnterCast((Token) node);
                    break;
                default:
                    switch (id)
                    {
                        case 2001:
                            this.EnterExpression((Production) node);
                            break;
                        case 2002:
                            this.EnterXorExpression((Production) node);
                            break;
                        case 2003:
                            this.EnterOrExpression((Production) node);
                            break;
                        case 2004:
                            this.EnterAndExpression((Production) node);
                            break;
                        case 2005:
                            this.EnterNotExpression((Production) node);
                            break;
                        case 2006:
                            this.EnterInExpression((Production) node);
                            break;
                        case 2007:
                            this.EnterInTargetExpression((Production) node);
                            break;
                        case 2008:
                            this.EnterInListTargetExpression((Production) node);
                            break;
                        case 2009:
                            this.EnterCompareExpression((Production) node);
                            break;
                        case 2010:
                            this.EnterShiftExpression((Production) node);
                            break;
                        case 2011:
                            this.EnterAdditiveExpression((Production) node);
                            break;
                        case 2012:
                            this.EnterMultiplicativeExpression((Production) node);
                            break;
                        case 2013:
                            this.EnterPowerExpression((Production) node);
                            break;
                        case 2014:
                            this.EnterNegateExpression((Production) node);
                            break;
                        case 2015:
                            this.EnterMemberExpression((Production) node);
                            break;
                        case 2016:
                            this.EnterMemberAccessExpression((Production) node);
                            break;
                        case 2017:
                            this.EnterBasicExpression((Production) node);
                            break;
                        case 2018:
                            this.EnterMemberFunctionExpression((Production) node);
                            break;
                        case 2019:
                            this.EnterFieldPropertyExpression((Production) node);
                            break;
                        case 2020:
                            this.EnterSpecialFunctionExpression((Production) node);
                            break;
                        case 2021:
                            this.EnterIfExpression((Production) node);
                            break;
                        case 2022:
                            this.EnterCastExpression((Production) node);
                            break;
                        case 2023:
                            this.EnterCastTypeExpression((Production) node);
                            break;
                        case 2024:
                            this.EnterIndexExpression((Production) node);
                            break;
                        case 2025:
                            this.EnterFunctionCallExpression((Production) node);
                            break;
                        case 2026:
                            this.EnterArgumentList((Production) node);
                            break;
                        case 2027:
                            this.EnterLiteralExpression((Production) node);
                            break;
                        case 2028:
                            this.EnterBooleanLiteralExpression((Production) node);
                            break;
                        case 2029:
                            this.EnterExpressionGroup((Production) node);
                            break;
                    }
                    break;
            }
        }

        public override Node Exit(Node node)
        {
            var id = node.Id;
            Node exitNode;
            switch (id)
            {
                case 1001:
                    exitNode = this.ExitAdd((Token) node);
                    return exitNode;
                case 1002:
                    exitNode = this.ExitSub((Token) node);
                    return exitNode;
                case 1003:
                    exitNode = this.ExitMul((Token) node);
                    return exitNode;
                case 1004:
                    exitNode = this.ExitDiv((Token) node);
                    return exitNode;
                case 1005:
                    exitNode = this.ExitPower((Token) node);
                    return exitNode;
                case 1006:
                    exitNode = this.ExitMod((Token) node);
                    return exitNode;
                case 1007:
                    exitNode = this.ExitLeftParen((Token) node);
                    return exitNode;
                case 1008:
                    exitNode = this.ExitRightParen((Token) node);
                    return exitNode;
                case 1009:
                    exitNode = this.ExitLeftBrace((Token) node);
                    return exitNode;
                case 1010:
                    exitNode = this.ExitRightBrace((Token) node);
                    return exitNode;
                case 1011:
                    exitNode = this.ExitEq((Token) node);
                    return exitNode;
                case 1012:
                    exitNode = this.ExitLt((Token) node);
                    return exitNode;
                case 1013:
                    exitNode = this.ExitGt((Token) node);
                    return exitNode;
                case 1014:
                    exitNode = this.ExitLte((Token) node);
                    return exitNode;
                case 1015:
                    exitNode = this.ExitGte((Token) node);
                    return exitNode;
                case 1016:
                    exitNode = this.ExitNe((Token) node);
                    return exitNode;
                case 1017:
                    exitNode = this.ExitAnd((Token) node);
                    return exitNode;
                case 1018:
                    exitNode = this.ExitOr((Token) node);
                    return exitNode;
                case 1019:
                    exitNode = this.ExitXor((Token) node);
                    return exitNode;
                case 1020:
                    exitNode = this.ExitNot((Token) node);
                    return exitNode;
                case 1021:
                    exitNode = this.ExitIn((Token) node);
                    return exitNode;
                case 1022:
                    exitNode = this.ExitDot((Token) node);
                    return exitNode;
                case 1023:
                    exitNode = this.ExitArgumentSeparator((Token) node);
                    return exitNode;
                case 1024:
                    exitNode = this.ExitArrayBraces((Token) node);
                    return exitNode;
                case 1025:
                    exitNode = this.ExitLeftShift((Token) node);
                    return exitNode;
                case 1026:
                    exitNode = this.ExitRightShift((Token) node);
                    return exitNode;
                case 1027:
                    break;
                case 1028:
                    exitNode = this.ExitInteger((Token) node);
                    return exitNode;
                case 1029:
                    exitNode = this.ExitReal((Token) node);
                    return exitNode;
                case 1030:
                    exitNode = this.ExitStringLiteral((Token) node);
                    return exitNode;
                case 1031:
                    exitNode = this.ExitCharLiteral((Token) node);
                    return exitNode;
                case 1032:
                    exitNode = this.ExitTrue((Token) node);
                    return exitNode;
                case 1033:
                    exitNode = this.ExitFalse((Token) node);
                    return exitNode;
                case 1034:
                    exitNode = this.ExitIdentifier((Token) node);
                    return exitNode;
                case 1035:
                    exitNode = this.ExitHexLiteral((Token) node);
                    return exitNode;
                case 1036:
                    exitNode = this.ExitNullLiteral((Token) node);
                    return exitNode;
                case 1037:
                    exitNode = this.ExitTimespan((Token) node);
                    return exitNode;
                case 1038:
                    exitNode = this.ExitDatetime((Token) node);
                    return exitNode;
                case 1039:
                    exitNode = this.ExitIf((Token) node);
                    return exitNode;
                case 1040:
                    exitNode = this.ExitCast((Token) node);
                    return exitNode;
                default:
                    switch (id)
                    {
                        case 2001:
                            exitNode = this.ExitExpression((Production) node);
                            return exitNode;
                        case 2002:
                            exitNode = this.ExitXorExpression((Production) node);
                            return exitNode;
                        case 2003:
                            exitNode = this.ExitOrExpression((Production) node);
                            return exitNode;
                        case 2004:
                            exitNode = this.ExitAndExpression((Production) node);
                            return exitNode;
                        case 2005:
                            exitNode = this.ExitNotExpression((Production) node);
                            return exitNode;
                        case 2006:
                            exitNode = this.ExitInExpression((Production) node);
                            return exitNode;
                        case 2007:
                            exitNode = this.ExitInTargetExpression((Production) node);
                            return exitNode;
                        case 2008:
                            exitNode = this.ExitInListTargetExpression((Production) node);
                            return exitNode;
                        case 2009:
                            exitNode = this.ExitCompareExpression((Production) node);
                            return exitNode;
                        case 2010:
                            exitNode = this.ExitShiftExpression((Production) node);
                            return exitNode;
                        case 2011:
                            exitNode = this.ExitAdditiveExpression((Production) node);
                            return exitNode;
                        case 2012:
                            exitNode = this.ExitMultiplicativeExpression((Production) node);
                            return exitNode;
                        case 2013:
                            exitNode = this.ExitPowerExpression((Production) node);
                            return exitNode;
                        case 2014:
                            exitNode = this.ExitNegateExpression((Production) node);
                            return exitNode;
                        case 2015:
                            exitNode = this.ExitMemberExpression((Production) node);
                            return exitNode;
                        case 2016:
                            exitNode = this.ExitMemberAccessExpression((Production) node);
                            return exitNode;
                        case 2017:
                            exitNode = this.ExitBasicExpression((Production) node);
                            return exitNode;
                        case 2018:
                            exitNode = this.ExitMemberFunctionExpression((Production) node);
                            return exitNode;
                        case 2019:
                            exitNode = this.ExitFieldPropertyExpression((Production) node);
                            return exitNode;
                        case 2020:
                            exitNode = this.ExitSpecialFunctionExpression((Production) node);
                            return exitNode;
                        case 2021:
                            exitNode = this.ExitIfExpression((Production) node);
                            return exitNode;
                        case 2022:
                            exitNode = this.ExitCastExpression((Production) node);
                            return exitNode;
                        case 2023:
                            exitNode = this.ExitCastTypeExpression((Production) node);
                            return exitNode;
                        case 2024:
                            exitNode = this.ExitIndexExpression((Production) node);
                            return exitNode;
                        case 2025:
                            exitNode = this.ExitFunctionCallExpression((Production) node);
                            return exitNode;
                        case 2026:
                            exitNode = this.ExitArgumentList((Production) node);
                            return exitNode;
                        case 2027:
                            exitNode = this.ExitLiteralExpression((Production) node);
                            return exitNode;
                        case 2028:
                            exitNode = this.ExitBooleanLiteralExpression((Production) node);
                            return exitNode;
                        case 2029:
                            exitNode = this.ExitExpressionGroup((Production) node);
                            return exitNode;
                    }
                    break;
            }
            exitNode = node;
            return exitNode;
        }

        public override void Child(Production node, Node child)
        {
            switch (node.Id)
            {
                case 2001:
                    this.ChildExpression(node, child);
                    break;
                case 2002:
                    this.ChildXorExpression(node, child);
                    break;
                case 2003:
                    this.ChildOrExpression(node, child);
                    break;
                case 2004:
                    this.ChildAndExpression(node, child);
                    break;
                case 2005:
                    this.ChildNotExpression(node, child);
                    break;
                case 2006:
                    this.ChildInExpression(node, child);
                    break;
                case 2007:
                    this.ChildInTargetExpression(node, child);
                    break;
                case 2008:
                    this.ChildInListTargetExpression(node, child);
                    break;
                case 2009:
                    this.ChildCompareExpression(node, child);
                    break;
                case 2010:
                    this.ChildShiftExpression(node, child);
                    break;
                case 2011:
                    this.ChildAdditiveExpression(node, child);
                    break;
                case 2012:
                    this.ChildMultiplicativeExpression(node, child);
                    break;
                case 2013:
                    this.ChildPowerExpression(node, child);
                    break;
                case 2014:
                    this.ChildNegateExpression(node, child);
                    break;
                case 2015:
                    this.ChildMemberExpression(node, child);
                    break;
                case 2016:
                    this.ChildMemberAccessExpression(node, child);
                    break;
                case 2017:
                    this.ChildBasicExpression(node, child);
                    break;
                case 2018:
                    this.ChildMemberFunctionExpression(node, child);
                    break;
                case 2019:
                    this.ChildFieldPropertyExpression(node, child);
                    break;
                case 2020:
                    this.ChildSpecialFunctionExpression(node, child);
                    break;
                case 2021:
                    this.ChildIfExpression(node, child);
                    break;
                case 2022:
                    this.ChildCastExpression(node, child);
                    break;
                case 2023:
                    this.ChildCastTypeExpression(node, child);
                    break;
                case 2024:
                    this.ChildIndexExpression(node, child);
                    break;
                case 2025:
                    this.ChildFunctionCallExpression(node, child);
                    break;
                case 2026:
                    this.ChildArgumentList(node, child);
                    break;
                case 2027:
                    this.ChildLiteralExpression(node, child);
                    break;
                case 2028:
                    this.ChildBooleanLiteralExpression(node, child);
                    break;
                case 2029:
                    this.ChildExpressionGroup(node, child);
                    break;
            }
        }

        public virtual void EnterAdd(Token node)
        {
        }

        public virtual Node ExitAdd(Token node)
        {
            return node;
        }

        public virtual void EnterSub(Token node)
        {
        }

        public virtual Node ExitSub(Token node)
        {
            return node;
        }

        public virtual void EnterMul(Token node)
        {
        }

        public virtual Node ExitMul(Token node)
        {
            return node;
        }

        public virtual void EnterDiv(Token node)
        {
        }

        public virtual Node ExitDiv(Token node)
        {
            return node;
        }

        public virtual void EnterPower(Token node)
        {
        }

        public virtual Node ExitPower(Token node)
        {
            return node;
        }

        public virtual void EnterMod(Token node)
        {
        }

        public virtual Node ExitMod(Token node)
        {
            return node;
        }

        public virtual void EnterLeftParen(Token node)
        {
        }

        public virtual Node ExitLeftParen(Token node)
        {
            return node;
        }

        public virtual void EnterRightParen(Token node)
        {
        }

        public virtual Node ExitRightParen(Token node)
        {
            return node;
        }

        public virtual void EnterLeftBrace(Token node)
        {
        }

        public virtual Node ExitLeftBrace(Token node)
        {
            return node;
        }

        public virtual void EnterRightBrace(Token node)
        {
        }

        public virtual Node ExitRightBrace(Token node)
        {
            return node;
        }

        public virtual void EnterEq(Token node)
        {
        }

        public virtual Node ExitEq(Token node)
        {
            return node;
        }

        public virtual void EnterLt(Token node)
        {
        }

        public virtual Node ExitLt(Token node)
        {
            return node;
        }

        public virtual void EnterGt(Token node)
        {
        }

        public virtual Node ExitGt(Token node)
        {
            return node;
        }

        public virtual void EnterLte(Token node)
        {
        }

        public virtual Node ExitLte(Token node)
        {
            return node;
        }

        public virtual void EnterGte(Token node)
        {
        }

        public virtual Node ExitGte(Token node)
        {
            return node;
        }

        public virtual void EnterNe(Token node)
        {
        }

        public virtual Node ExitNe(Token node)
        {
            return node;
        }

        public virtual void EnterAnd(Token node)
        {
        }

        public virtual Node ExitAnd(Token node)
        {
            return node;
        }

        public virtual void EnterOr(Token node)
        {
        }

        public virtual Node ExitOr(Token node)
        {
            return node;
        }

        public virtual void EnterXor(Token node)
        {
        }

        public virtual Node ExitXor(Token node)
        {
            return node;
        }

        public virtual void EnterNot(Token node)
        {
        }

        public virtual Node ExitNot(Token node)
        {
            return node;
        }

        public virtual void EnterIn(Token node)
        {
        }

        public virtual Node ExitIn(Token node)
        {
            return node;
        }

        public virtual void EnterDot(Token node)
        {
        }

        public virtual Node ExitDot(Token node)
        {
            return node;
        }

        public virtual void EnterArgumentSeparator(Token node)
        {
        }

        public virtual Node ExitArgumentSeparator(Token node)
        {
            return node;
        }

        public virtual void EnterArrayBraces(Token node)
        {
        }

        public virtual Node ExitArrayBraces(Token node)
        {
            return node;
        }

        public virtual void EnterLeftShift(Token node)
        {
        }

        public virtual Node ExitLeftShift(Token node)
        {
            return node;
        }

        public virtual void EnterRightShift(Token node)
        {
        }

        public virtual Node ExitRightShift(Token node)
        {
            return node;
        }

        public virtual void EnterInteger(Token node)
        {
        }

        public virtual Node ExitInteger(Token node)
        {
            return node;
        }

        public virtual void EnterReal(Token node)
        {
        }

        public virtual Node ExitReal(Token node)
        {
            return node;
        }

        public virtual void EnterStringLiteral(Token node)
        {
        }

        public virtual Node ExitStringLiteral(Token node)
        {
            return node;
        }

        public virtual void EnterCharLiteral(Token node)
        {
        }

        public virtual Node ExitCharLiteral(Token node)
        {
            return node;
        }

        public virtual void EnterTrue(Token node)
        {
        }

        public virtual Node ExitTrue(Token node)
        {
            return node;
        }

        public virtual void EnterFalse(Token node)
        {
        }

        public virtual Node ExitFalse(Token node)
        {
            return node;
        }

        public virtual void EnterIdentifier(Token node)
        {
        }

        public virtual Node ExitIdentifier(Token node)
        {
            return node;
        }

        public virtual void EnterHexLiteral(Token node)
        {
        }

        public virtual Node ExitHexLiteral(Token node)
        {
            return node;
        }

        public virtual void EnterNullLiteral(Token node)
        {
        }

        public virtual Node ExitNullLiteral(Token node)
        {
            return node;
        }

        public virtual void EnterTimespan(Token node)
        {
        }

        public virtual Node ExitTimespan(Token node)
        {
            return node;
        }

        public virtual void EnterDatetime(Token node)
        {
        }

        public virtual Node ExitDatetime(Token node)
        {
            return node;
        }

        public virtual void EnterIf(Token node)
        {
        }

        public virtual Node ExitIf(Token node)
        {
            return node;
        }

        public virtual void EnterCast(Token node)
        {
        }

        public virtual Node ExitCast(Token node)
        {
            return node;
        }

        public virtual void EnterExpression(Production node)
        {
        }

        public virtual Node ExitExpression(Production node)
        {
            return node;
        }

        public virtual void ChildExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterXorExpression(Production node)
        {
        }

        public virtual Node ExitXorExpression(Production node)
        {
            return node;
        }

        public virtual void ChildXorExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterOrExpression(Production node)
        {
        }

        public virtual Node ExitOrExpression(Production node)
        {
            return node;
        }

        public virtual void ChildOrExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterAndExpression(Production node)
        {
        }

        public virtual Node ExitAndExpression(Production node)
        {
            return node;
        }

        public virtual void ChildAndExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterNotExpression(Production node)
        {
        }

        public virtual Node ExitNotExpression(Production node)
        {
            return node;
        }

        public virtual void ChildNotExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterInExpression(Production node)
        {
        }

        public virtual Node ExitInExpression(Production node)
        {
            return node;
        }

        public virtual void ChildInExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterInTargetExpression(Production node)
        {
        }

        public virtual Node ExitInTargetExpression(Production node)
        {
            return node;
        }

        public virtual void ChildInTargetExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterInListTargetExpression(Production node)
        {
        }

        public virtual Node ExitInListTargetExpression(Production node)
        {
            return node;
        }

        public virtual void ChildInListTargetExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterCompareExpression(Production node)
        {
        }

        public virtual Node ExitCompareExpression(Production node)
        {
            return node;
        }

        public virtual void ChildCompareExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterShiftExpression(Production node)
        {
        }

        public virtual Node ExitShiftExpression(Production node)
        {
            return node;
        }

        public virtual void ChildShiftExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterAdditiveExpression(Production node)
        {
        }

        public virtual Node ExitAdditiveExpression(Production node)
        {
            return node;
        }

        public virtual void ChildAdditiveExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterMultiplicativeExpression(Production node)
        {
        }

        public virtual Node ExitMultiplicativeExpression(Production node)
        {
            return node;
        }

        public virtual void ChildMultiplicativeExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterPowerExpression(Production node)
        {
        }

        public virtual Node ExitPowerExpression(Production node)
        {
            return node;
        }

        public virtual void ChildPowerExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterNegateExpression(Production node)
        {
        }

        public virtual Node ExitNegateExpression(Production node)
        {
            return node;
        }

        public virtual void ChildNegateExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterMemberExpression(Production node)
        {
        }

        public virtual Node ExitMemberExpression(Production node)
        {
            return node;
        }

        public virtual void ChildMemberExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterMemberAccessExpression(Production node)
        {
        }

        public virtual Node ExitMemberAccessExpression(Production node)
        {
            return node;
        }

        public virtual void ChildMemberAccessExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterBasicExpression(Production node)
        {
        }

        public virtual Node ExitBasicExpression(Production node)
        {
            return node;
        }

        public virtual void ChildBasicExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterMemberFunctionExpression(Production node)
        {
        }

        public virtual Node ExitMemberFunctionExpression(Production node)
        {
            return node;
        }

        public virtual void ChildMemberFunctionExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterFieldPropertyExpression(Production node)
        {
        }

        public virtual Node ExitFieldPropertyExpression(Production node)
        {
            return node;
        }

        public virtual void ChildFieldPropertyExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterSpecialFunctionExpression(Production node)
        {
        }

        public virtual Node ExitSpecialFunctionExpression(Production node)
        {
            return node;
        }

        public virtual void ChildSpecialFunctionExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterIfExpression(Production node)
        {
        }

        public virtual Node ExitIfExpression(Production node)
        {
            return node;
        }

        public virtual void ChildIfExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterCastExpression(Production node)
        {
        }

        public virtual Node ExitCastExpression(Production node)
        {
            return node;
        }

        public virtual void ChildCastExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterCastTypeExpression(Production node)
        {
        }

        public virtual Node ExitCastTypeExpression(Production node)
        {
            return node;
        }

        public virtual void ChildCastTypeExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterIndexExpression(Production node)
        {
        }

        public virtual Node ExitIndexExpression(Production node)
        {
            return node;
        }

        public virtual void ChildIndexExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterFunctionCallExpression(Production node)
        {
        }

        public virtual Node ExitFunctionCallExpression(Production node)
        {
            return node;
        }

        public virtual void ChildFunctionCallExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterArgumentList(Production node)
        {
        }

        public virtual Node ExitArgumentList(Production node)
        {
            return node;
        }

        public virtual void ChildArgumentList(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterLiteralExpression(Production node)
        {
        }

        public virtual Node ExitLiteralExpression(Production node)
        {
            return node;
        }

        public virtual void ChildLiteralExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterBooleanLiteralExpression(Production node)
        {
        }

        public virtual Node ExitBooleanLiteralExpression(Production node)
        {
            return node;
        }

        public virtual void ChildBooleanLiteralExpression(Production node, Node child)
        {
            node.AddChild(child);
        }

        public virtual void EnterExpressionGroup(Production node)
        {
        }

        public virtual Node ExitExpressionGroup(Production node)
        {
            return node;
        }

        public virtual void ChildExpressionGroup(Production node, Node child)
        {
            node.AddChild(child);
        }
    }
}