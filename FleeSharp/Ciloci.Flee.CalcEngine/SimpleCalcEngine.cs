using System;
using System.Collections.Generic;

namespace Ciloci.Flee.CalcEngine
{
	public class SimpleCalcEngine
	{
		private IDictionary<string, IExpression> MyExpressions;

		private ExpressionContext MyContext;

		public IExpression this[string name]
		{
			get
			{
				IExpression e = null;
				this.MyExpressions.TryGetValue(name, out e);
				return e;
			}
		}

		public ExpressionContext Context
		{
			get
			{
				return this.MyContext;
			}
			set
			{
				this.MyContext = value;
			}
		}

		public SimpleCalcEngine()
		{
			this.MyExpressions = new Dictionary<string, IExpression>(StringComparer.OrdinalIgnoreCase);
			this.MyContext = new ExpressionContext();
		}

		private void AddCompiledExpression(string expressionName, IExpression expression)
		{
			bool flag = this.MyExpressions.ContainsKey(expressionName);
			if (flag)
			{
				throw new InvalidOperationException(string.Format("The calc engine already contains an expression named '{0}'", expressionName));
			}
			this.MyExpressions.Add(expressionName, expression);
		}

		private ExpressionContext ParseAndLink(string expressionName, string expression)
		{
			IdentifierAnalyzer analyzer = this.Context.ParseIdentifiers(expression);
			ExpressionContext context2 = this.MyContext.CloneInternal(true);
			this.LinkExpression(expressionName, context2, analyzer);
			context2.NoClone = true;
			this.MyContext.Variables.Clear();
			return context2;
		}

		private void LinkExpression(string expressionName, ExpressionContext context, IdentifierAnalyzer analyzer)
		{
			try
			{
				IEnumerator<string> enumerator = analyzer.GetIdentifiers(context).GetEnumerator();
				while (enumerator.MoveNext())
				{
					string identifier = enumerator.Current;
					this.LinkIdentifier(identifier, expressionName, context);
				}
			}
			finally
			{
				IEnumerator<string> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}

		private void LinkIdentifier(string identifier, string expressionName, ExpressionContext context)
		{
			IExpression child = null;
			bool flag = !this.MyExpressions.TryGetValue(identifier, out child);
			if (flag)
			{
				string msg = string.Format("Expression '{0}' references unknown name '{1}'", expressionName, identifier);
				throw new InvalidOperationException(msg);
			}
			context.Variables.Add(identifier, child);
		}

		public void AddDynamic(string expressionName, string expression)
		{
			ExpressionContext linkedContext = this.ParseAndLink(expressionName, expression);
			IExpression e = linkedContext.CompileDynamic(expression);
			this.AddCompiledExpression(expressionName, e);
		}

		public void AddGeneric<T>(string expressionName, string expression)
		{
			ExpressionContext linkedContext = this.ParseAndLink(expressionName, expression);
			IExpression e = linkedContext.CompileGeneric<T>(expression);
			this.AddCompiledExpression(expressionName, e);
		}

		public void Clear()
		{
			this.MyExpressions.Clear();
		}
	}
}
