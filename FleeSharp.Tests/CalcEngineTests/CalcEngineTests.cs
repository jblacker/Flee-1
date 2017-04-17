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
namespace FleeSharp.Tests.CalcEngineTests
{
    using CalculationEngine;
    using Exceptions;
    using NUnit.Framework;

    [TestFixture]
    public class CalcEngineTests
    {
        private CalculationEngine ce;
        private ExpressionContext context;
        private VariableCollection variables;

        [SetUp]
        public void SetUp()
        {
            this.ce = new CalculationEngine();
            this.context = new ExpressionContext();
            this.variables = this.context.Variables;
        }

        [Test]
        public void CalcEngine_Returns_Same_Values_As_VB()
        {
            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 1);
            ce.Add("b", "a + y", context);
            ce.Add("c", "b * 2", context);
            ce.Recalculate("a");

            var result = ce.GetResult("c");
            Assert.AreEqual(result, ((100 * 2) + 1) * 2);

            variables["x"] = 345;
            ce.Recalculate("a");
            result = ce.GetResult("c");
            Assert.AreEqual(((345 * 2) + 1) * 2, result);
        }
        [Test]
        public void CalcEngine_Returns_Same_Values_As_VB_Using_Indexer()
        {
            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 1);
            ce.Add("b", "a + y", context);
            ce.Add("c", "b * 2", context);
            ce.Recalculate("a");

            var result = ce.GetResult("c");
            //Assert.AreEqual(result, ((100 * 2) + 1) * 2);

            variables["x"] = 345;
            ce.Recalculate("a");
            result = ce.GetResult("c");
            Assert.AreEqual(((345 * 2) + 1) * 2, result);
        }

        [Test]
        public void Test_Mutiple_Identical_References_Are_Same_As_VB()
        {
            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            ce.Add("b", "a + a + a", context);
            ce.Recalculate("a");
            var result = ce.GetResult("b");
            Assert.AreEqual((100 * 2) * 3, result);
        }

        [Test]
        public void Complex_Test_Results_Are_Identical_To_VB()
        {

            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 24);
            ce.Add("b", "y * 2", context);
            ce.Add("c", "a + b", context);
            ce.Add("d", "80", context);
            ce.Add("e", "a + b + c + d", context);
            ce.Recalculate("d");
            ce.Recalculate(new[] { "a", "b" });
            var result = ce.GetResult("e");
            Assert.AreEqual((100 * 2) + (24 * 2) + ((100 * 2) + (24 * 2)) + 80, result);
        }

        [Test]
        public void Recalculate_Non_Source_Is_Same_As_VB()
        {
            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 1);
            ce.Add("b", "a + y", context);
            ce.Add("c", "b * 2", context);
            ce.Recalculate("a", "b");
            var result = ce.GetResult("c");
            Assert.AreEqual(((100) * 2 + 1) * 2, result);
        }

        [Test]
        public void Partial_Recaclutate_Same_As_VB()
        {

            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 1);
            ce.Add("b", "a + y", context);
            ce.Add("c", "b * 2", context);
            ce.Recalculate("a");
            variables["y"] = 222;
            ce.Recalculate("b");
            var result = ce.GetResult("c");
            Assert.AreEqual(((100 * 2) + 222) * 2, result);
        }

        [Test]
        public void First_Circular_Reference_Should_Throw()
        {
            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 1);
            ce.Add("b", "a + y + b", context);

            Assert.Throws<CircularReferenceException>(() => ce.Recalculate("a"));
        }

        [Test]
        public void Second_Circular_Reference_Should_Throw()
        {
            variables.Add("x", 100);
            ce.Add("a", "x * 2", context);
            variables.Add("y", 1);
            ce.Add("b", "a + y + b", context);

            Assert.Throws<CircularReferenceException>(() => ce.Recalculate("b"));
        }

        [Test]
        public void Test_With_Reference_Types_Is_Same_As_VB()
        {
            variables.Add("x", "string");
            var exp = "x + \" \"";
            ce.Add("a", exp, context);
            variables.Add("y", "word");
            ce.Add("b", "y", context);
            ce.Add("c", "a + b", context);
            ce.Recalculate("b", "a");
            var result = ce.GetResult("c");
            var expected = "string word";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void First_Remove_Tests_Are_Same_As_VB()
        {
            ce.Add("a", "100", context);
            ce.Add("b", "200", context);
            ce.Add("c", "a + b", context);
            ce.Add("d", "300", context);
            ce.Add("e", "c + d", context);
            ce.Remove("a");

            // Only b and d should remain

            Assert.AreEqual(2, ce.Count);

            ce.Remove("b");
            Assert.AreEqual(1, ce.Count);
            ce.Remove("d");
            Assert.AreEqual(0, ce.Count);

            // b and d should have no dependents or precedents

            Assert.IsFalse(ce.HasDependents("b"));
            Assert.IsFalse(ce.HasDependents("d"));
            Assert.IsFalse(ce.HasPrecedents("b"));
            Assert.IsFalse(ce.HasPrecedents("d"));
        }

        [Test]
        public void Second_Remove_Tests_Are_Same_As_VB()
        {
            ce.Add("a", "100", context);
            ce.Add("b", "200", context);
            ce.Add("c", "a + b", context);
            ce.Add("d", "300", context);
            ce.Add("e", "c + d + a", context);

            // Only b and d should remain
            ce.Remove("a");
            Assert.AreEqual(2, ce.Count);

            ce.Remove("b");
            Assert.AreEqual(1, ce.Count);

            ce.Remove("d");
            Assert.AreEqual(0, ce.Count);
        }

        [Test]
        public void Third_Remove_Tests_Are_The_Same_As_VB()
        {
            ce.Add("a", "100", context);
            ce.Add("b", "200", context);
            ce.Add("c", "a + b", context);
            ce.Add("d", "300 + c", context);
            ce.Add("e", "c + d", context);

            ce.Remove("d");
            Assert.AreEqual(3, ce.Count);

            ce.Recalculate("c");
            ce.Remove("c");
            Assert.AreEqual(2, ce.Count);

            ce.Remove("a");
            Assert.AreEqual(1, ce.Count);

            ce.Remove("b");
            Assert.AreEqual(0, ce.Count);
        }

        [Test]
        public void Fourth_Remove_Tests_Are_The_Same_As_VB()
        {
            ce.Add("a", "100", context);
            ce.Add("b", "200", context);
            ce.Add("c", "a + b", context);
            ce.Add("d", "300 + c", context);
            ce.Add("e", "c + d", context);

            ce.Remove("a");
            Assert.AreEqual(1, ce.Count);

            ce.Remove("b");
            Assert.AreEqual(0, ce.Count);
        }

        [Test]
        public void Batch_Load_Tests_Are_The_Same_As_VB()
        {
            int interest = 2;

            this.variables.Add("interest", interest);
            var loader = ce.CreateBatchLoader();

            loader.Add("c", "a + b", context);
            loader.Add("a", "100 + interest", context);
            loader.Add("b", "a + 1 + a", context);

            // Test an expression with a reference in a string

            var refString = "\"str \\\" str\" + a + \"b\"";



            loader.Add("d", refString, context);
            ce.BatchLoad(loader);
            var result = ce.GetResult("b");
            Assert.AreEqual((100 + interest) + 1 + (100 + interest), result);

            interest = 300;
            this.variables["interest"] = interest;
            ce.Recalculate("a");
            result = ce.GetResult("b");
            Assert.AreEqual((100 + interest) + 1 + (100 + interest), result);
            result = ce.GetResult("c");
            Assert.AreEqual((100 + interest) + 1 + (100 + interest) + (100 + interest), result);
            Assert.AreEqual("str \" str400b", ce.GetResult("d"));
        }

        [Test]
        public void Test_Calc_Engine_Atom()
        {
            ce.Add("a", "\"abc\"", context);
            ce.Add("b", "a.length", context);
            ce.Add("c", "a.startswith(\"a\")", context);

            var result = ce.GetResult("b");
            Assert.AreEqual("abc".Length, result);
            Assert.AreEqual(true, ce.GetResult("c"));
        }

        [Test]
        public void Test_Dependency_Management_Is_Same_As_VB()
        {
            ce.Add("a", "100", context);
            ce.Add("b", "100", context);

            // Nothing should point to a and b
            Assert.IsFalse(ce.HasPrecedents("a"));
            Assert.IsFalse(ce.HasPrecedents("b"));
            Assert.IsFalse(ce.HasDependents("a"));
            Assert.IsFalse(ce.HasDependents("b"));

            ce.Add("c", "a + b", context);
            ce.Add("d", "a + c", context);

            // a and b still have nothing pointing to them
            Assert.IsFalse(ce.HasPrecedents("a"));
            Assert.IsFalse(ce.HasPrecedents("b"));

            // but they have dependents
            Assert.IsTrue(ce.HasDependents("a"));
            Assert.IsTrue(ce.HasDependents("b"));

            // c and d have precedents
            Assert.IsTrue(ce.HasPrecedents("d"));
            Assert.IsTrue(ce.HasPrecedents("c"));

            // and only c should have dependents
            Assert.IsTrue(ce.HasDependents("c"));
            Assert.IsFalse(ce.HasDependents("d"));

            // test our counts
            Assert.AreEqual(2, ce.GetDependents("a").Length);
            Assert.AreEqual(1, ce.GetDependents("b").Length);
            Assert.AreEqual(1, ce.GetDependents("c").Length);
            Assert.AreEqual(0, ce.GetDependents("d").Length);

            Assert.AreEqual(0, ce.GetPrecedents("a").Length);
            Assert.AreEqual(0, ce.GetPrecedents("b").Length);
            Assert.AreEqual(2, ce.GetPrecedents("c").Length);
            Assert.AreEqual(2, ce.GetPrecedents("d").Length);

            ce.Remove("d");

            Assert.IsFalse(ce.HasDependents("c"));
            Assert.IsFalse(ce.HasDependents("d"));
            Assert.IsFalse(ce.HasPrecedents("d"));
            Assert.IsTrue(ce.HasPrecedents("c"));

            Assert.AreEqual(1, ce.GetDependents("a").Length);
            Assert.AreEqual(1, ce.GetDependents("b").Length);
            Assert.AreEqual(0, ce.GetDependents("c").Length);

            ce.Remove("c");

            Assert.IsFalse(ce.HasPrecedents("c"));
            Assert.IsFalse(ce.HasDependents("c"));
            Assert.IsFalse(ce.HasDependents("a"));
            Assert.IsFalse(ce.HasDependents("b"));

            Assert.AreEqual(0, ce.GetDependents("a").Length);
            Assert.AreEqual(0, ce.GetDependents("b").Length);

            ce.Remove("a");
            ce.Remove("b");

            Assert.IsFalse(ce.HasDependents("a"));
            Assert.IsFalse(ce.HasPrecedents("a"));
            Assert.IsFalse(ce.HasDependents("b"));
            Assert.IsFalse(ce.HasPrecedents("b"));

            Assert.AreEqual(0, ce.GetDependents("a").Length);
            Assert.AreEqual(0, ce.GetDependents("b").Length);
            Assert.AreEqual(0, ce.GetPrecedents("a").Length);
            Assert.AreEqual(0, ce.GetPrecedents("b").Length);
        }

        [Test]
        public void Informational_Methods_Can_Be_Called_With_Non_Existant_Expression()
        {
            //Test that our informational methods can be called with a non-existant expression

            var engine = new CalculationEngine();

            Assert.IsFalse(engine.HasDependents("zz"));
            Assert.IsFalse(engine.HasPrecedents("zz"));
            Assert.AreEqual(0, engine.GetDependents("zz").Length);
            Assert.AreEqual(0, engine.GetPrecedents("zz").Length);
        }

        [Test]
        public void Clear_Works_The_Same_As_The_VB()
        {
            ce.Add("a", "100", context);
            ce.Add("b", "a + 2", context);
            ce.Clear();
            
            Assert.IsFalse(ce.HasDependents("a"));
            Assert.IsFalse(ce.HasPrecedents("b"));
            Assert.AreEqual(0, ce.Count);
        }
    }
}