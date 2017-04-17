using System.IO;

namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class ExpressionParser : RecursiveDescentParser
    {
        private enum SynteticPatterns
        {
            SUBPRODUCTION_1 = 3001,
            SUBPRODUCTION_2,
            SUBPRODUCTION_3,
            SUBPRODUCTION_4,
            SUBPRODUCTION_5,
            SUBPRODUCTION_6,
            SUBPRODUCTION_7,
            SUBPRODUCTION_8,
            SUBPRODUCTION_9,
            SUBPRODUCTION_10,
            SUBPRODUCTION_11,
            SUBPRODUCTION_12,
            SUBPRODUCTION_13,
            SUBPRODUCTION_14,
            SUBPRODUCTION_15,
            SUBPRODUCTION_16
        }

        public ExpressionParser(TextReader input, Analyzer analyzer, ExpressionContext context) : base(new ExpressionTokenizer(input, context), analyzer)
        {
            this.CreatePatterns();
        }

        public ExpressionParser(TextReader input) : base(new ExpressionTokenizer(input))
        {
            this.CreatePatterns();
        }

        public ExpressionParser(TextReader input, Analyzer analyzer) : base(new ExpressionTokenizer(input), analyzer)
        {
            this.CreatePatterns();
        }

        private void CreatePatterns()
        {
            ProductionPattern pattern = new ProductionPattern(2001, "Expression");
            ProductionPatternAlternative alt = new ProductionPatternAlternative();
            alt.AddProduction(2002, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2002, "XorExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2003, 1, 1);
            alt.AddProduction(3001, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2003, "OrExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2004, 1, 1);
            alt.AddProduction(3002, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2004, "AndExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2005, 1, 1);
            alt.AddProduction(3003, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2005, "NotExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1020, 0, 1);
            alt.AddProduction(2006, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2006, "InExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2009, 1, 1);
            alt.AddProduction(3004, 0, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2007, "InTargetExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2019, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2008, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2008, "InListTargetExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1007, 1, 1);
            alt.AddProduction(2026, 1, 1);
            alt.AddToken(1008, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2009, "CompareExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2010, 1, 1);
            alt.AddProduction(3006, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2010, "ShiftExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2011, 1, 1);
            alt.AddProduction(3008, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2011, "AdditiveExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2012, 1, 1);
            alt.AddProduction(3010, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2012, "MultiplicativeExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2013, 1, 1);
            alt.AddProduction(3012, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2013, "PowerExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2014, 1, 1);
            alt.AddProduction(3013, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2014, "NegateExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1002, 0, 1);
            alt.AddProduction(2015, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2015, "MemberExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2017, 1, 1);
            alt.AddProduction(3014, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2016, "MemberAccessExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1022, 1, 1);
            alt.AddProduction(2018, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2017, "BasicExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2027, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2029, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2018, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2020, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2018, "MemberFunctionExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2019, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2025, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2019, "FieldPropertyExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1034, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2020, "SpecialFunctionExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2021, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2022, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2021, "IfExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1039, 1, 1);
            alt.AddToken(1007, 1, 1);
            alt.AddProduction(2001, 1, 1);
            alt.AddToken(1023, 1, 1);
            alt.AddProduction(2001, 1, 1);
            alt.AddToken(1023, 1, 1);
            alt.AddProduction(2001, 1, 1);
            alt.AddToken(1008, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2022, "CastExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1040, 1, 1);
            alt.AddToken(1007, 1, 1);
            alt.AddProduction(2001, 1, 1);
            alt.AddToken(1023, 1, 1);
            alt.AddProduction(2023, 1, 1);
            alt.AddToken(1008, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2023, "CastTypeExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1034, 1, 1);
            alt.AddProduction(3015, 0, -1);
            alt.AddToken(1024, 0, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2024, "IndexExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1009, 1, 1);
            alt.AddProduction(2026, 1, 1);
            alt.AddToken(1010, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2025, "FunctionCallExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1034, 1, 1);
            alt.AddToken(1007, 1, 1);
            alt.AddProduction(2026, 0, 1);
            alt.AddToken(1008, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2026, "ArgumentList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2001, 1, 1);
            alt.AddProduction(3016, 0, -1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2027, "LiteralExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1028, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1029, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1030, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2028, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1035, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1031, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1036, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1038, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1037, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2028, "BooleanLiteralExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1032, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1033, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(2029, "ExpressionGroup");
            alt = new ProductionPatternAlternative();
            alt.AddToken(1007, 1, 1);
            alt.AddProduction(2001, 1, 1);
            alt.AddToken(1008, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3001, "Subproduction1");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1019, 1, 1);
            alt.AddProduction(2003, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3002, "Subproduction2");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1018, 1, 1);
            alt.AddProduction(2004, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3003, "Subproduction3");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1017, 1, 1);
            alt.AddProduction(2005, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3004, "Subproduction4");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1021, 1, 1);
            alt.AddProduction(2007, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3005, "Subproduction5");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1011, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1013, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1012, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1015, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1014, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1016, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3006, "Subproduction6");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction(3005, 1, 1);
            alt.AddProduction(2010, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3007, "Subproduction7");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1025, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1026, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3008, "Subproduction8");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction(3007, 1, 1);
            alt.AddProduction(2011, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3009, "Subproduction9");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1001, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1002, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3010, "Subproduction10");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction(3009, 1, 1);
            alt.AddProduction(2012, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3011, "Subproduction11");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1003, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1004, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken(1006, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3012, "Subproduction12");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction(3011, 1, 1);
            alt.AddProduction(2013, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3013, "Subproduction13");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1005, 1, 1);
            alt.AddProduction(2014, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3014, "Subproduction14");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2016, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction(2024, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3015, "Subproduction15");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1022, 1, 1);
            alt.AddToken(1034, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
            pattern = new ProductionPattern(3016, "Subproduction16");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken(1023, 1, 1);
            alt.AddProduction(2001, 1, 1);
            pattern.AddAlternative(alt);
            this.AddPattern(pattern);
        }
    }
}