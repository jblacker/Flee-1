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

namespace FleeSharp
{
    internal enum ExpressionConstants
    {
        ADD = 1001,
        SUB,
        MUL,
        DIV,
        POWER,
        MOD,
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        EQ,
        LT,
        GT,
        LTE,
        GTE,
        NE,
        AND,
        OR,
        XOR,
        NOT,
        IN,
        DOT,
        ARGUMENT_SEPARATOR,
        ARRAY_BRACES,
        LEFT_SHIFT,
        RIGHT_SHIFT,
        WHITESPACE,
        INTEGER,
        REAL,
        STRING_LITERAL,
        CHAR_LITERAL,
        TRUE,
        FALSE,
        IDENTIFIER,
        HEX_LITERAL,
        NULL_LITERAL,
        TIMESPAN,
        DATETIME,
        IF,
        CAST,
        EXPRESSION = 2001,
        XOR_EXPRESSION,
        OR_EXPRESSION,
        AND_EXPRESSION,
        NOT_EXPRESSION,
        IN_EXPRESSION,
        IN_TARGET_EXPRESSION,
        IN_LIST_TARGET_EXPRESSION,
        COMPARE_EXPRESSION,
        SHIFT_EXPRESSION,
        ADDITIVE_EXPRESSION,
        MULTIPLICATIVE_EXPRESSION,
        POWER_EXPRESSION,
        NEGATE_EXPRESSION,
        MEMBER_EXPRESSION,
        MEMBER_ACCESS_EXPRESSION,
        BASIC_EXPRESSION,
        MEMBER_FUNCTION_EXPRESSION,
        FIELD_PROPERTY_EXPRESSION,
        SPECIAL_FUNCTION_EXPRESSION,
        IF_EXPRESSION,
        CAST_EXPRESSION,
        CAST_TYPE_EXPRESSION,
        INDEX_EXPRESSION,
        FUNCTION_CALL_EXPRESSION,
        ARGUMENT_LIST,
        LITERAL_EXPRESSION,
        BOOLEAN_LITERAL_EXPRESSION,
        EXPRESSION_GROUP
    }
}