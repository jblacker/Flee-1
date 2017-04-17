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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Xml.XPath;
    using Exceptions;
    using NUnit.Framework;

    public abstract class ExpressionTestBase
    {

        private const char CommentChar = '\'';
        private const char SeparatorChar = ';';
        protected static readonly CultureInfo testCulture = CultureInfo.GetCultureInfo("en-CA");
        protected ExpressionContext myCurrentContext;
        protected ExpressionContext myGenericContext;
        protected ExpressionContext myValidCastsContext;
        protected ExpressionOwner myValidExpressionsOwner;

        protected ExpressionTestBase()
        {
            this.myValidExpressionsOwner = new ExpressionOwner();
            this.myValidExpressionsOwner = new ExpressionOwner();
            this.myGenericContext = this.CreateGenericContext(this.myValidExpressionsOwner);
            var context = new ExpressionContext(this.myValidExpressionsOwner);
            context.Options.OwnerMemberAccess = BindingFlags.Public | BindingFlags.NonPublic;
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(Convert), "Convert");
            context.Imports.AddType(typeof(Guid));
            context.Imports.AddType(typeof(Version));
            context.Imports.AddType(typeof(DayOfWeek));
            context.Imports.AddType(typeof(DayOfWeek), "DayOfWeek");
            context.Imports.AddType(typeof(ValueType));
            context.Imports.AddType(typeof(IComparable));
            context.Imports.AddType(typeof(ICloneable));
            context.Imports.AddType(typeof(Array));
            context.Imports.AddType(typeof(Delegate));
            context.Imports.AddType(typeof(AppDomainInitializer));
            context.Imports.AddType(typeof(Encoding));
            context.Imports.AddType(typeof(ASCIIEncoding));
            context.Imports.AddType(typeof(ArgumentException));
            this.myValidCastsContext = context;
            TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(int))), typeof(int));
            TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(string))),
                typeof(string));
            this.Initialize();
        }

        protected virtual void Initialize()
        {
        }

        protected ExpressionContext CreateGenericContext(object owner)
        {
            var flag = owner == null;
            ExpressionContext context;
            if (flag)
            {
                context = new ExpressionContext();
            }
            else
            {
                context = new ExpressionContext(RuntimeHelpers.GetObjectValue(owner));
            }
            context.Options.OwnerMemberAccess = BindingFlags.Public | BindingFlags.NonPublic;
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(Math), "Math");
            context.Imports.AddType(typeof(Uri), "Uri");
            context.Imports.AddType(typeof(Mouse), "Mouse");
            context.Imports.AddType(typeof(Monitor), "Monitor");
            context.Imports.AddType(typeof(DateTime), "DateTime");
            context.Imports.AddType(typeof(Convert), "Convert");
            context.Imports.AddType(typeof(Type), "Type");
            context.Imports.AddType(typeof(DayOfWeek), "DayOfWeek");
            context.Imports.AddType(typeof(ConsoleModifiers), "ConsoleModifiers");
            var ns = new NamespaceImport("ns1");
            var ns2 = new NamespaceImport("ns2") { new TypeImport(typeof(Math)) };
            ns.Add(ns2);
            context.Imports.RootImport.Add(ns);
            context.Variables.Add("varInt32", 100);
            context.Variables.Add("varDecimal", 100m);
            context.Variables.Add("varString", "string");
            return context;
        }

        protected IGenericExpression<T> CreateGenericExpression<T>(string expression)
        {
            return this.CreateGenericExpression<T>(expression, new ExpressionContext());
        }

        protected IGenericExpression<T> CreateGenericExpression<T>(string expression, ExpressionContext context)
        {
            return context.CompileGeneric<T>(expression);
        }

        protected IDynamicExpression CreateDynamicExpression(string expression)
        {
            return this.CreateDynamicExpression(expression, new ExpressionContext());
        }

        protected IDynamicExpression CreateDynamicExpression(string expression, ExpressionContext context)
        {
            return context.CompileDynamic(expression);
        }

        protected void AssertCompileException(string expression)
        {
            try
            {
                this.CreateDynamicExpression(expression);
                Assert.Fail();
            }
            catch (ExpressionCompileException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        protected void AssertCompileException(string expression, ExpressionContext context,
            CompileExceptionReason expectedReason = (CompileExceptionReason)(-1))
        {
            try
            {
                this.CreateDynamicExpression(expression, context);
                Assert.Fail("Compile exception expected");
            }
            catch (ExpressionCompileException expressionCompileException)
            {
                var ex = expressionCompileException;
                var flag = expectedReason != (CompileExceptionReason)(-1);
                if (flag)
                {
                    Assert.AreEqual((int)expectedReason, (int)ex.Reason,
                        string.Format("Expected reason '{0}' but got '{1}'", expectedReason, ex.Reason));
                }
            }
        }

        protected void DoTest(IDynamicExpression e, string result, Type resultType, CultureInfo testCulture)
        {
            var flag = resultType == typeof(object);
            if (flag)
            {
                var expectedType = Type.GetType(result, false, true);
                var flag2 = expectedType == null;
                if (flag2)
                {
                    result = $"{this.GetType().Namespace}.{result}";
                    expectedType = this.GetType().Assembly.GetType(result, true, true);
                }
                var expressionResult = RuntimeHelpers.GetObjectValue(e.Evaluate());
                var flag3 = expectedType == typeof(void);
                if (flag3)
                {
                    Assert.IsNull(RuntimeHelpers.GetObjectValue(expressionResult));
                }
                else
                {
                    Assert.IsInstanceOf(expectedType, RuntimeHelpers.GetObjectValue(expressionResult));
                }
            }
            else
            {
                var tc = TypeDescriptor.GetConverter(resultType);
                var expectedResult = RuntimeHelpers.GetObjectValue(tc.ConvertFromString(null, testCulture, result));
                var actualResult = RuntimeHelpers.GetObjectValue(e.Evaluate());
                expectedResult = RuntimeHelpers.GetObjectValue(this.RoundIfReal(RuntimeHelpers.GetObjectValue(expectedResult)));
                actualResult = RuntimeHelpers.GetObjectValue(this.RoundIfReal(RuntimeHelpers.GetObjectValue(actualResult)));
                Assert.AreEqual(RuntimeHelpers.GetObjectValue(expectedResult), RuntimeHelpers.GetObjectValue(actualResult));
            }
        }

        protected object RoundIfReal(object value)
        {
            var flag = value.GetType() == typeof(double);
            object roundIfReal;
            if (flag)
            {
                var d = (double)value;
                d = Math.Round(d, 4);
                roundIfReal = d;
            }
            else
            {
                var flag2 = value.GetType() == typeof(float);
                if (flag2)
                {
                    var s = (float)value;
                    s = (float)Math.Round(s, 4);
                    roundIfReal = s;
                }
                else
                {
                    roundIfReal = value;
                }
            }
            return roundIfReal;
        }

        protected void ProcessScriptTests(string scriptFileName, LineProcessor processor)
        {
            this.WriteMessage("Testing: {0}", scriptFileName);
            var instream = this.GetScriptFile(scriptFileName);
            var sr = new StreamReader(instream);
            try
            {
                this.ProcessLines(sr, processor);
            }
            finally
            {
                sr.Close();
            }
        }

        private void ProcessLines(TextReader sr, LineProcessor processor)
        {
            while (sr.Peek() != -1)
            {
                var line = sr.ReadLine();
                this.ProcessLine(line, processor);
            }
        }

        private void ProcessLine(string line, LineProcessor processor)
        {
            var flag = line.StartsWith("'");
            if (!flag)
            {
                try
                {
                    var arr = line.Split(';');
                    processor(arr);
                }
                catch (Exception exception)
                {
                    this.WriteMessage("Failed line: {0}", line);
                    throw;
                }
            }
        }

        protected Stream GetScriptFile(string fileName)
        {
            var a = Assembly.GetExecutingAssembly();
            return a.GetManifestResourceStream(this.GetType(), fileName);
        }

        protected string GetIndividualTest(string testName)
        {
            var a = Assembly.GetExecutingAssembly();
            using (var s = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\TestScripts\\IndividualTests.xml",FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var doc = new XPathDocument(s);
                var nav = doc.CreateNavigator();
                nav = nav.SelectSingleNode($"Tests/Test[@Name='{testName}']");
                var str = (string) nav.TypedValue;
                s.Close();
                return str;
            }
        }

        protected void WriteMessage(string msg, params object[] args)
        {
            msg = string.Format(msg, args);
            Console.WriteLine(msg);
        }

        protected static object Parse(string s)
        {
            bool b;
            var flag = bool.TryParse(s, out b);
            object parse;
            if (flag)
            {
                parse = b;
            }
            else
            {
                int i;
                var flag2 = int.TryParse(s, NumberStyles.Integer, testCulture, out i);
                if (flag2)
                {
                    parse = i;
                }
                else
                {
                    var arg481 = NumberStyles.Float;
                    IFormatProvider arg482 = testCulture;
                    double a = i;
                    var flag3 = double.TryParse(s, arg481, arg482, out a);
                    i = checked((int)Math.Round(a));
                    var flag4 = flag3;
                    if (flag4)
                    {
                        double d = 0;
                        parse = d;
                    }
                    else
                    {
                        DateTime dt;
                        var flag5 = DateTime.TryParse(s, testCulture, DateTimeStyles.None, out dt);
                        if (flag5)
                        {
                            parse = dt;
                        }
                        else
                        {
                            parse = s;
                        }
                    }
                }
            }
            return parse;
        }

        protected static IDictionary<string, object> ParseQueryString(string s)
        {
            var arr = s.Split('&');
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var array = arr;
            checked
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var part = array[i];
                    var arr2 = part.Split('=');
                    dict.Add(arr2[0], RuntimeHelpers.GetObjectValue(Parse(arr2[1])));
                }
                return dict;
            }
        }

        protected delegate void LineProcessor(string[] lineParts);
    }


}