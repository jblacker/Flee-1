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

namespace Flee.CalculationEngine
{
    using System;
    using System.Collections.Generic;
    using Extensions;

    public class SimpleCalcEngine
    {
        private readonly IDictionary<string, IExpression> expressionDictionary;

        public SimpleCalcEngine()
        {
            this.expressionDictionary = new Dictionary<string, IExpression>(StringComparer.OrdinalIgnoreCase);
            this.Context = new ExpressionContext();
        }

        private void AddCompiledExpression(string expressionName, IExpression expression)
        {
            var flag = this.expressionDictionary.ContainsKey(expressionName);
            if (flag)
            {
                throw new InvalidOperationException($"The calc engine already contains an expression named '{expressionName}'");
            }
            this.expressionDictionary.Add(expressionName, expression);
        }

        private ExpressionContext ParseAndLink(string expressionName, string expression)
        {
            var analyzer = this.Context.ParseIdentifiers(expression);
            var context2 = this.Context.CloneInternal(true);
            this.LinkExpression(expressionName, context2, analyzer);
            context2.NoClone = true;
            this.Context.Variables.Clear();
            return context2;
        }

        private void LinkExpression(string expressionName, ExpressionContext context, IdentifierAnalyzer analyzer)
        {
            analyzer.GetIdentifiers(context).Each(i => this.LinkIdentifier(i, expressionName, context));
        }

        private void LinkIdentifier(string identifier, string expressionName, ExpressionContext context)
        {
            IExpression child;
            var flag = !this.expressionDictionary.TryGetValue(identifier, out child);
            if (flag)
            {
                string msg = $"Expression '{expressionName}' references unknown name '{identifier}'";
                throw new InvalidOperationException(msg);
            }
            context.Variables.Add(identifier, child);
        }

        public void AddDynamic(string expressionName, string expression)
        {
            var linkedContext = this.ParseAndLink(expressionName, expression);
            IExpression e = linkedContext.CompileDynamic(expression);
            this.AddCompiledExpression(expressionName, e);
        }

        public void AddGeneric<T>(string expressionName, string expression)
        {
            var linkedContext = this.ParseAndLink(expressionName, expression);
            IExpression e = linkedContext.CompileGeneric<T>(expression);
            this.AddCompiledExpression(expressionName, e);
        }

        public void Clear()
        {
            this.expressionDictionary.Clear();
        }

        public ExpressionContext Context { get; set; }

        public IExpression this[string name]
        {
            get
            {
                IExpression e;
                this.expressionDictionary.TryGetValue(name, out e);
                return e;
            }
        }
    }
}