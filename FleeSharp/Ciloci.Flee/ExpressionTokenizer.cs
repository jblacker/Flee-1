using Ciloci.Flee.PerCederberg.Grammatica.Runtime;
using System;
using System.IO;

namespace Ciloci.Flee
{
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
			TokenPattern pattern = new TokenPattern(1001, "ADD", TokenPattern.PatternType.STRING, "+");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1002, "SUB", TokenPattern.PatternType.STRING, "-");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1003, "MUL", TokenPattern.PatternType.STRING, "*");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1004, "DIV", TokenPattern.PatternType.STRING, "/");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1005, "POWER", TokenPattern.PatternType.STRING, "^");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1006, "MOD", TokenPattern.PatternType.STRING, "%");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1007, "LEFT_PAREN", TokenPattern.PatternType.STRING, "(");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1008, "RIGHT_PAREN", TokenPattern.PatternType.STRING, ")");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1009, "LEFT_BRACE", TokenPattern.PatternType.STRING, "[");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1010, "RIGHT_BRACE", TokenPattern.PatternType.STRING, "]");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1011, "EQ", TokenPattern.PatternType.STRING, "=");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1012, "LT", TokenPattern.PatternType.STRING, "<");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1013, "GT", TokenPattern.PatternType.STRING, ">");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1014, "LTE", TokenPattern.PatternType.STRING, "<=");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1015, "GTE", TokenPattern.PatternType.STRING, ">=");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1016, "NE", TokenPattern.PatternType.STRING, "<>");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1017, "AND", TokenPattern.PatternType.STRING, "AND");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1018, "OR", TokenPattern.PatternType.STRING, "OR");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1019, "XOR", TokenPattern.PatternType.STRING, "XOR");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1020, "NOT", TokenPattern.PatternType.STRING, "NOT");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1021, "IN", TokenPattern.PatternType.STRING, "in");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1022, "DOT", TokenPattern.PatternType.STRING, ".");
			base.AddPattern(pattern);
			CustomTokenPattern customPattern = new ArgumentSeparatorPattern();
			customPattern.Initialize(1023, "ARGUMENT_SEPARATOR", TokenPattern.PatternType.STRING, ",", this.MyContext);
			base.AddPattern(customPattern);
			pattern = new TokenPattern(1024, "ARRAY_BRACES", TokenPattern.PatternType.STRING, "[]");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1025, "LEFT_SHIFT", TokenPattern.PatternType.STRING, "<<");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1026, "RIGHT_SHIFT", TokenPattern.PatternType.STRING, ">>");
			base.AddPattern(pattern);
			base.AddPattern(new TokenPattern(1027, "WHITESPACE", TokenPattern.PatternType.REGEXP, "\\s+")
			{
				Ignore = true
			});
			pattern = new TokenPattern(1028, "INTEGER", TokenPattern.PatternType.REGEXP, "\\d+(u|l|ul|lu|f|m)?");
			base.AddPattern(pattern);
			customPattern = new RealPattern();
			customPattern.Initialize(1029, "REAL", TokenPattern.PatternType.REGEXP, "\\d{0}\\{1}\\d+([e][+-]\\d{{1,3}})?(d|f|m)?", this.MyContext);
			base.AddPattern(customPattern);
			customPattern = new StringPattern();
			customPattern.Initialize(1030, "STRING_LITERAL", TokenPattern.PatternType.REGEXP, "\"([^\"\\r\\n\\\\]|\\\\u[0-9a-f]{4}|\\\\[\\\\\"'trn])*\"", this.MyContext);
			base.AddPattern(customPattern);
			pattern = new TokenPattern(1031, "CHAR_LITERAL", TokenPattern.PatternType.REGEXP, "'([^'\\r\\n\\\\]|\\\\u[0-9a-f]{4}|\\\\[\\\\\"'trn])'");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1032, "TRUE", TokenPattern.PatternType.STRING, "True");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1033, "FALSE", TokenPattern.PatternType.STRING, "False");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1034, "IDENTIFIER", TokenPattern.PatternType.REGEXP, "[a-z_]\\w*");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1035, "HEX_LITERAL", TokenPattern.PatternType.REGEXP, "0x[0-9a-f]+(u|l|ul|lu)?");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1036, "NULL_LITERAL", TokenPattern.PatternType.STRING, "null");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1037, "TIMESPAN", TokenPattern.PatternType.REGEXP, "##(\\d+\\.)?\\d{2}:\\d{2}(:\\d{2}(\\.\\d{1,7})?)?#");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1038, "DATETIME", TokenPattern.PatternType.REGEXP, "#[^#]+#");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1039, "IF", TokenPattern.PatternType.STRING, "if");
			base.AddPattern(pattern);
			pattern = new TokenPattern(1040, "CAST", TokenPattern.PatternType.STRING, "cast");
			base.AddPattern(pattern);
		}
	}
}
