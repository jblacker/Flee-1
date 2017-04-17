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
namespace FleeSharp.Tests.ExpressionTests
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using CalculationEngine;
    using NUnit.Framework;

    [TestFixture]
    public class BenchmarkTests : ExpressionTestBase
    {
        [Test(Description = "Test that setting variables is fast")]
        public void TestFastVariables()
        {
            var context = new ExpressionContext();
            var vars = context.Variables;
            vars.DefineVariable("a", typeof(int));
            vars.DefineVariable("b", typeof(int));
            var e = this.CreateDynamicExpression("a + b * (a ^ 2)", context);
            var sw = new Stopwatch();
            sw.Start();
            var i = 0;
            checked
            {
                do
                {
                    var result = RuntimeHelpers.GetObjectValue(e.Evaluate());
                    vars["a"] = 200;
                    vars["b"] = 300;
                    i++;
                }
                while (i <= 99999);
                sw.Stop();
                this.PrintSpeedMessage("Fast variables", 100000, sw);
                Assert.Less(new decimal(sw.ElapsedMilliseconds), new decimal(200L), "Test time above expected value");
            }
        }

        [Test(Description = "Test the speed of the simple calc engine")]
        public void TestSimpleCalcEngine()
        {
            var engine = new SimpleCalcEngine();
            engine.Context.Imports.AddType(typeof(Math));
            var sw = new Stopwatch();
            var prev = "1";
            sw.Start();
            var i = 0;
            checked
            {
                string cur;
                do
                {
                    cur = $"i_{Guid.NewGuid().ToString("N")}";
                    string expression = $"{prev} + 1 + cos(3.14)";
                    engine.AddGeneric<double>(cur, expression);
                    prev = cur;
                    i++;
                }
                while (i <= 1999);
                sw.Stop();
                this.PrintSpeedMessage("Simple calc engine (population)", 2000, sw);
                var e = (IGenericExpression<double>)engine[cur];
                sw.Reset();
                sw.Start();
                var result = e.Evaluate();
                sw.Stop();
                this.PrintSpeedMessage("Simple calc engine (evaluation)", 2000, sw);
            }
        }

        [Test(Description = "Test how fast we parse/compile an expression")]
        public void TestParseCompileSpeed()
        {
            var expressionText = base.GetIndividualTest("LongBranch1");
            var context = new ExpressionContext();
            var vc = context.Variables;
            vc.Add("M0100_ASSMT_REASON", "0");
            vc.Add("M0220_PRIOR_NOCHG_14D", "1");
            vc.Add("M0220_PRIOR_UNKNOWN", "1");
            vc.Add("M0220_PRIOR_UR_INCON", "1");
            vc.Add("M0220_PRIOR_CATH", "1");
            vc.Add("M0220_PRIOR_INTRACT_PAIN", "1");
            vc.Add("M0220_PRIOR_IMPR_DECSN", "1");
            vc.Add("M0220_PRIOR_DISRUPTIVE", "1");
            vc.Add("M0220_PRIOR_MEM_LOSS", "1");
            vc.Add("M0220_PRIOR_NONE", "1");
            vc.Add("M0220_PRIOR_UR_INCON_bool", true);
            vc.Add("M0220_PRIOR_CATH_bool", true);
            vc.Add("M0220_PRIOR_INTRACT_PAIN_bool", true);
            vc.Add("M0220_PRIOR_IMPR_DECSN_bool", true);
            vc.Add("M0220_PRIOR_DISRUPTIVE_bool", true);
            vc.Add("M0220_PRIOR_MEM_LOSS_bool", true);
            vc.Add("M0220_PRIOR_NONE_bool", true);
            vc.Add("M0220_PRIOR_NOCHG_14D_bool", true);
            vc.Add("M0220_PRIOR_UNKNOWN_bool", true);
            var sw = new Stopwatch();
            var e = base.CreateDynamicExpression(expressionText, context);
            sw.Start();
            e = base.CreateDynamicExpression(expressionText, context);
            e = base.CreateDynamicExpression(base.GetIndividualTest("LongBranch2"));
            e = base.CreateDynamicExpression("if(1 > 0, 1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0, 20.0)");
            sw.Stop();
            var timePerExpression = (float)((double)sw.ElapsedMilliseconds / 3.0);
            Assert.Less(timePerExpression, 20f);
            base.WriteMessage("Parse/Compile speed = {0:n0}ms", new object[]
            {
                timePerExpression
            });
        }

        private void PrintSpeedMessage(string title, int iterations, Stopwatch sw)
        {
            base.WriteMessage("{0}: {1:n0} iterations in {2:n2}ms = {3:n2} iterations/sec", new object[]
            {
                title,
                iterations,
                sw.ElapsedMilliseconds,
                (double)iterations / ((double)sw.ElapsedMilliseconds / 1000.0)
            });
        }
    }
}
