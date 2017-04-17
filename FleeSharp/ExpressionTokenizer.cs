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
    using System.IO;
    using PerCederberg.Grammatica.Runtime;

    internal class ExpressionTokenizer : Tokenizer
    {
        private readonly ExpressionContext myContext;

        public ExpressionTokenizer(TextReader input, ExpressionContext context)
            : base(input, true)
        {
            this.myContext = context;
            this.CreatePatterns();
        }

        public ExpressionTokenizer(TextReader input)
            : base(input, true)
        {
            this.CreatePatterns();
        }

        private void CreatePatterns()
        {
            var pattern = new TokenPattern((int) ExpressionConstants.ADD, "ADD", TokenPattern.PatternType.String, "+");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.SUB, "SUB", TokenPattern.PatternType.String, "-");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.MUL, "MUL", TokenPattern.PatternType.String, "*");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.DIV, "DIV", TokenPattern.PatternType.String, "/");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.POWER, "POWER", TokenPattern.PatternType.String, "^");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.MOD, "MOD", TokenPattern.PatternType.String, "%");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.LEFT_PAREN, "LEFT_PAREN", TokenPattern.PatternType.String, "(");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.RIGHT_PAREN, "RIGHT_PAREN", TokenPattern.PatternType.String, ")");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.LEFT_BRACE, "LEFT_BRACE", TokenPattern.PatternType.String, "[");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.RIGHT_BRACE, "RIGHT_BRACE", TokenPattern.PatternType.String, "]");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.EQ, "EQ", TokenPattern.PatternType.String, "=");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.LT, "LT", TokenPattern.PatternType.String, "<");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.GT, "GT", TokenPattern.PatternType.String, ">");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.LTE, "LTE", TokenPattern.PatternType.String, "<=");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.GTE, "GTE", TokenPattern.PatternType.String, ">=");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.NE, "NE", TokenPattern.PatternType.String, "<>");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.AND, "AND", TokenPattern.PatternType.String, "AND");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.OR, "OR", TokenPattern.PatternType.String, "OR");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.XOR, "XOR", TokenPattern.PatternType.String, "XOR");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.NOT, "NOT", TokenPattern.PatternType.String, "NOT");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.IN, "IN", TokenPattern.PatternType.String, "in");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.DOT, "DOT", TokenPattern.PatternType.String, ".");
            this.AddPattern(pattern);
            CustomTokenPattern customPattern = new ArgumentSeparatorPattern();
            customPattern.Initialize((int) ExpressionConstants.ARGUMENT_SEPARATOR, "ARGUMENT_SEPARATOR",
                TokenPattern.PatternType.String, ",", this.myContext);
            this.AddPattern(customPattern);
            pattern = new TokenPattern((int) ExpressionConstants.ARRAY_BRACES, "ARRAY_BRACES", TokenPattern.PatternType.String, "[]");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.LEFT_SHIFT, "LEFT_SHIFT", TokenPattern.PatternType.String, "<<");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.RIGHT_SHIFT, "RIGHT_SHIFT", TokenPattern.PatternType.String, ">>");
            this.AddPattern(pattern);
            this.AddPattern(new TokenPattern((int) ExpressionConstants.WHITESPACE, "WHITESPACE", TokenPattern.PatternType.Regexp,
                "\\s+")
            {
                Ignore = true
            });
            pattern = new TokenPattern((int) ExpressionConstants.INTEGER, "INTEGER", TokenPattern.PatternType.Regexp,
                "\\d+(u|l|ul|lu|f|m)?");
            this.AddPattern(pattern);
            customPattern = new RealPattern();
            customPattern.Initialize((int) ExpressionConstants.REAL, "REAL", TokenPattern.PatternType.Regexp,
                "\\d{0}\\{1}\\d+([e][+-]\\d{{1,3}})?(d|f|m)?", this.myContext);
            this.AddPattern(customPattern);
            customPattern = new StringPattern();
            customPattern.Initialize((int) ExpressionConstants.STRING_LITERAL, "STRING_LITERAL", TokenPattern.PatternType.Regexp,
                "\"([^\"\\r\\n\\\\]|\\\\u[0-9a-f]{4}|\\\\[\\\\\"'trn])*\"", this.myContext);
            this.AddPattern(customPattern);
            pattern = new TokenPattern((int) ExpressionConstants.CHAR_LITERAL, "CHAR_LITERAL", TokenPattern.PatternType.Regexp,
                "'([^'\\r\\n\\\\]|\\\\u[0-9a-f]{4}|\\\\[\\\\\"'trn])'");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.TRUE, "TRUE", TokenPattern.PatternType.String, "True");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.FALSE, "FALSE", TokenPattern.PatternType.String, "False");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.IDENTIFIER, "IDENTIFIER", TokenPattern.PatternType.Regexp,
                "[a-z_]\\w*");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.HEX_LITERAL, "HEX_LITERAL", TokenPattern.PatternType.Regexp,
                "0x[0-9a-f]+(u|l|ul|lu)?");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.NULL_LITERAL, "NULL_LITERAL", TokenPattern.PatternType.String, "null");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.TIMESPAN, "TIMESPAN", TokenPattern.PatternType.Regexp,
                "##(\\d+\\.)?\\d{2}:\\d{2}(:\\d{2}(\\.\\d{1,7})?)?#");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.DATETIME, "DATETIME", TokenPattern.PatternType.Regexp, "#[^#]+#");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.IF, "IF", TokenPattern.PatternType.String, "if");
            this.AddPattern(pattern);
            pattern = new TokenPattern((int) ExpressionConstants.CAST, "CAST", TokenPattern.PatternType.String, "cast");
            this.AddPattern(pattern);
        }
    }
}