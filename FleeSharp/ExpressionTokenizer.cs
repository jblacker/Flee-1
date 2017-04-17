using System.IO;

namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class ExpressionTokenizer : Tokenizer
    {
        private ExpressionContext MyContext;

        public ExpressionTokenizer(TextReader input, ExpressionContext context) : base(input, true)
        {
            this.MyContext = context;
            this.CreatePatterns();
        }

        public ExpressionTokenizer(TextReader input) : base(input, true)
        {
            this.CreatePatterns();
        }

        private void CreatePatterns()
        {
            TokenPattern pattern = new TokenPattern(1001, "ADD", TokenPattern.PatternType.String, "+");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1002, "SUB", TokenPattern.PatternType.String, "-");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1003, "MUL", TokenPattern.PatternType.String, "*");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1004, "DIV", TokenPattern.PatternType.String, "/");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1005, "POWER", TokenPattern.PatternType.String, "^");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1006, "MOD", TokenPattern.PatternType.String, "%");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1007, "LEFT_PAREN", TokenPattern.PatternType.String, "(");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1008, "RIGHT_PAREN", TokenPattern.PatternType.String, ")");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1009, "LEFT_BRACE", TokenPattern.PatternType.String, "[");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1010, "RIGHT_BRACE", TokenPattern.PatternType.String, "]");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1011, "EQ", TokenPattern.PatternType.String, "=");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1012, "LT", TokenPattern.PatternType.String, "<");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1013, "GT", TokenPattern.PatternType.String, ">");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1014, "LTE", TokenPattern.PatternType.String, "<=");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1015, "GTE", TokenPattern.PatternType.String, ">=");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1016, "NE", TokenPattern.PatternType.String, "<>");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1017, "AND", TokenPattern.PatternType.String, "AND");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1018, "OR", TokenPattern.PatternType.String, "OR");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1019, "XOR", TokenPattern.PatternType.String, "XOR");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1020, "NOT", TokenPattern.PatternType.String, "NOT");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1021, "IN", TokenPattern.PatternType.String, "in");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1022, "DOT", TokenPattern.PatternType.String, ".");
            this.AddPattern(pattern);
            CustomTokenPattern customPattern = new ArgumentSeparatorPattern();
            customPattern.Initialize(1023, "ARGUMENT_SEPARATOR", TokenPattern.PatternType.String, ",", this.MyContext);
            this.AddPattern(customPattern);
            pattern = new TokenPattern(1024, "ARRAY_BRACES", TokenPattern.PatternType.String, "[]");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1025, "LEFT_SHIFT", TokenPattern.PatternType.String, "<<");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1026, "RIGHT_SHIFT", TokenPattern.PatternType.String, ">>");
            this.AddPattern(pattern);
            this.AddPattern(new TokenPattern(1027, "WHITESPACE", TokenPattern.PatternType.Regexp, "\\s+")
            {
                Ignore = true
            });
            pattern = new TokenPattern(1028, "INTEGER", TokenPattern.PatternType.Regexp, "\\d+(u|l|ul|lu|f|m)?");
            this.AddPattern(pattern);
            customPattern = new RealPattern();
            customPattern.Initialize(1029, "REAL", TokenPattern.PatternType.Regexp, "\\d{0}\\{1}\\d+([e][+-]\\d{{1,3}})?(d|f|m)?", this.MyContext);
            this.AddPattern(customPattern);
            customPattern = new StringPattern();
            customPattern.Initialize(1030, "STRING_LITERAL", TokenPattern.PatternType.Regexp, "\"([^\"\\r\\n\\\\]|\\\\u[0-9a-f]{4}|\\\\[\\\\\"'trn])*\"", this.MyContext);
            this.AddPattern(customPattern);
            pattern = new TokenPattern(1031, "CHAR_LITERAL", TokenPattern.PatternType.Regexp, "'([^'\\r\\n\\\\]|\\\\u[0-9a-f]{4}|\\\\[\\\\\"'trn])'");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1032, "TRUE", TokenPattern.PatternType.String, "True");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1033, "FALSE", TokenPattern.PatternType.String, "False");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1034, "IDENTIFIER", TokenPattern.PatternType.Regexp, "[a-z_]\\w*");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1035, "HEX_LITERAL", TokenPattern.PatternType.Regexp, "0x[0-9a-f]+(u|l|ul|lu)?");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1036, "NULL_LITERAL", TokenPattern.PatternType.String, "null");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1037, "TIMESPAN", TokenPattern.PatternType.Regexp, "##(\\d+\\.)?\\d{2}:\\d{2}(:\\d{2}(\\.\\d{1,7})?)?#");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1038, "DATETIME", TokenPattern.PatternType.Regexp, "#[^#]+#");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1039, "IF", TokenPattern.PatternType.String, "if");
            this.AddPattern(pattern);
            pattern = new TokenPattern(1040, "CAST", TokenPattern.PatternType.String, "cast");
            this.AddPattern(pattern);
        }
    }
}