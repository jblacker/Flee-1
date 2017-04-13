using Ciloci.Flee.CalcEngine;
using Ciloci.Flee.PerCederberg.Grammatica.Runtime;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	public sealed class ExpressionContext
	{
		private PropertyDictionary MyProperties;

		private object MySyncRoot;

		private VariableCollection MyVariables;

		private ExpressionParser IdentifierParser
		{
			get
			{
				ExpressionParser parser = this.MyProperties.GetValue<ExpressionParser>("IdentifierParser");
				bool flag = parser == null;
				if (flag)
				{
					IdentifierAnalyzer analyzer = new IdentifierAnalyzer();
					parser = new ExpressionParser(TextReader.Null, analyzer, this);
					this.MyProperties.SetValue("IdentifierParser", parser);
				}
				return parser;
			}
		}

		internal bool NoClone
		{
			get
			{
				return this.MyProperties.GetValue<bool>("NoClone");
			}
			set
			{
				this.MyProperties.SetValue("NoClone", value);
			}
		}

		internal object ExpressionOwner
		{
			get
			{
				return this.MyProperties.GetValue<object>("ExpressionOwner");
			}
		}

		internal string CalcEngineExpressionName
		{
			get
			{
				return this.MyProperties.GetValue<string>("CalcEngineExpressionName");
			}
		}

		internal ExpressionParser Parser
		{
			get
			{
				return this.MyProperties.GetValue<ExpressionParser>("ExpressionParser");
			}
		}

		public ExpressionOptions Options
		{
			get
			{
				return this.MyProperties.GetValue<ExpressionOptions>("Options");
			}
		}

		public ExpressionImports Imports
		{
			get
			{
				return this.MyProperties.GetValue<ExpressionImports>("Imports");
			}
		}

		public VariableCollection Variables
		{
			get
			{
				return this.MyVariables;
			}
		}

		public CalculationEngine CalculationEngine
		{
			get
			{
				return this.MyProperties.GetValue<CalculationEngine>("CalculationEngine");
			}
		}

		public ExpressionParserOptions ParserOptions
		{
			get
			{
				return this.MyProperties.GetValue<ExpressionParserOptions>("ParserOptions");
			}
		}

		public ExpressionContext() : this(RuntimeHelpers.GetObjectValue(DefaultExpressionOwner.Instance))
		{
		}

		public ExpressionContext(object expressionOwner)
		{
			this.MySyncRoot = RuntimeHelpers.GetObjectValue(new object());
			Utility.AssertNotNull(RuntimeHelpers.GetObjectValue(expressionOwner), "expressionOwner");
			this.MyProperties = new PropertyDictionary();
			this.MyProperties.SetValue("CalculationEngine", null);
			this.MyProperties.SetValue("CalcEngineExpressionName", null);
			this.MyProperties.SetValue("IdentifierParser", null);
			this.MyProperties.SetValue("ExpressionOwner", RuntimeHelpers.GetObjectValue(expressionOwner));
			this.MyProperties.SetValue("ParserOptions", new ExpressionParserOptions(this));
			this.MyProperties.SetValue("Options", new ExpressionOptions(this));
			this.MyProperties.SetValue("Imports", new ExpressionImports());
			this.Imports.SetContext(this);
			this.MyVariables = new VariableCollection(this);
			this.MyProperties.SetToDefault<bool>("NoClone");
			this.RecreateParser();
		}

		private void AssertTypeIsAccessibleInternal(Type t)
		{
			bool isPublic = t.IsPublic;
			bool isNested = t.IsNested;
			if (isNested)
			{
				isPublic = t.IsNestedPublic;
			}
			bool isSameModuleAsOwner = t.Module == this.ExpressionOwner.GetType().Module;
			bool isAccessible = isPublic | isSameModuleAsOwner;
			bool flag = !isAccessible;
			if (flag)
			{
				string msg = Utility.GetGeneralErrorMessage("TypeNotAccessibleToExpression", new object[]
				{
					t.Name
				});
				throw new ArgumentException(msg);
			}
		}

		private void AssertNestedTypeIsAccessible(Type t)
		{
			while (t != null)
			{
				this.AssertTypeIsAccessibleInternal(t);
				t = t.DeclaringType;
			}
		}

		internal ExpressionContext CloneInternal(bool cloneVariables)
		{
			ExpressionContext context = (ExpressionContext)base.MemberwiseClone();
			context.MyProperties = this.MyProperties.Clone();
			context.MyProperties.SetValue("Options", this.Options.Clone());
			context.MyProperties.SetValue("ParserOptions", this.ParserOptions.Clone());
			context.MyProperties.SetValue("Imports", this.Imports.Clone());
			context.Imports.SetContext(context);
			if (cloneVariables)
			{
				context.MyVariables = new VariableCollection(this);
				this.Variables.Copy(context.MyVariables);
			}
			return context;
		}

		internal void AssertTypeIsAccessible(Type t)
		{
			bool isNested = t.IsNested;
			if (isNested)
			{
				this.AssertNestedTypeIsAccessible(t);
			}
			else
			{
				this.AssertTypeIsAccessibleInternal(t);
			}
		}

		internal ExpressionElement Parse(string expression, IServiceProvider services)
		{
			object mySyncRoot = this.MySyncRoot;
			ObjectFlowControl.CheckForSyncLockOnValueType(mySyncRoot);
			ExpressionElement Parse;
			lock (mySyncRoot)
			{
				StringReader sr = new StringReader(expression);
				ExpressionParser parser = this.Parser;
				parser.Reset();
				parser.Tokenizer.Reset(sr);
				FleeExpressionAnalyzer analyzer = (FleeExpressionAnalyzer)parser.Analyzer;
				analyzer.SetServices(services);
				Node rootNode = this.DoParse();
				analyzer.Reset();
				ExpressionElement topElement = (ExpressionElement)rootNode.Values[0];
				Parse = topElement;
			}
			return Parse;
		}

		internal void RecreateParser()
		{
			object mySyncRoot = this.MySyncRoot;
			ObjectFlowControl.CheckForSyncLockOnValueType(mySyncRoot);
			lock (mySyncRoot)
			{
				FleeExpressionAnalyzer analyzer = new FleeExpressionAnalyzer();
				ExpressionParser parser = new ExpressionParser(TextReader.Null, analyzer, this);
				this.MyProperties.SetValue("ExpressionParser", parser);
			}
		}

		internal Node DoParse()
		{
			Node DoParse;
			try
			{
				DoParse = this.Parser.Parse();
			}
			catch (ParserLogException expr_10)
			{
				ProjectData.SetProjectError(expr_10);
				ParserLogException ex = expr_10;
				throw new ExpressionCompileException(ex);
			}
			return DoParse;
		}

		internal void SetCalcEngine(CalculationEngine engine, string calcEngineExpressionName)
		{
			this.MyProperties.SetValue("CalculationEngine", engine);
			this.MyProperties.SetValue("CalcEngineExpressionName", calcEngineExpressionName);
		}

		internal IdentifierAnalyzer ParseIdentifiers(string expression)
		{
			ExpressionParser parser = this.IdentifierParser;
			StringReader sr = new StringReader(expression);
			parser.Reset();
			parser.Tokenizer.Reset(sr);
			IdentifierAnalyzer analyzer = (IdentifierAnalyzer)parser.Analyzer;
			analyzer.Reset();
			parser.Parse();
			return (IdentifierAnalyzer)parser.Analyzer;
		}

		public ExpressionContext Clone()
		{
			return this.CloneInternal(true);
		}

		public IDynamicExpression CompileDynamic(string expression)
		{
			return new Expression<object>(expression, this, false);
		}

		public IGenericExpression<TResultType> CompileGeneric<TResultType>(string expression)
		{
			return new Expression<TResultType>(expression, this, true);
		}
	}
}
