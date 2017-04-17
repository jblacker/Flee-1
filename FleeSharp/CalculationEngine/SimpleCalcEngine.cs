namespace Flee.CalculationEngine
{
    using System;
    using System.Collections.Generic;
    using Extensions;

    public class SimpleCalcEngine
	{
		private readonly IDictionary<string, IExpression> expressionDictionary;

	    public IExpression this[string name]
		{
			get
			{
				IExpression e;
				this.expressionDictionary.TryGetValue(name, out e);
				return e;
			}
		}

		public ExpressionContext Context { get; set; }

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
	}
}
