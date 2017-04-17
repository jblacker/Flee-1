namespace Flee
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Exceptions;
    using Microsoft.VisualBasic.CompilerServices;
    using PerCederberg.Grammatica.Runtime;

    public sealed class ExpressionContext
    {
        private PropertyDictionary myProperties;

        private readonly object mySyncRoot;

        public ExpressionContext()
            : this(RuntimeHelpers.GetObjectValue(DefaultExpressionOwner.Instance))
        {
        }

        public ExpressionContext(object expressionOwner)
        {
            this.mySyncRoot = RuntimeHelpers.GetObjectValue(new object());
            Utility.AssertNotNull(RuntimeHelpers.GetObjectValue(expressionOwner), "expressionOwner");
            this.myProperties = new PropertyDictionary();
            this.myProperties.SetValue("CalculationEngine", null);
            this.myProperties.SetValue("CalcEngineExpressionName", null);
            this.myProperties.SetValue("IdentifierParser", null);
            this.myProperties.SetValue("ExpressionOwner", RuntimeHelpers.GetObjectValue(expressionOwner));
            this.myProperties.SetValue("ParserOptions", new ExpressionParserOptions(this));
            this.myProperties.SetValue("Options", new ExpressionOptions(this));
            this.myProperties.SetValue("Imports", new ExpressionImports());
            this.Imports.SetContext(this);
            this.Variables = new VariableCollection(this);
            this.myProperties.SetToDefault<bool>("NoClone");
            this.RecreateParser();
        }

        private void AssertTypeIsAccessibleInternal(Type t)
        {
            var isPublic = t.IsPublic;
            var isNested = t.IsNested;
            if (isNested)
            {
                isPublic = t.IsNestedPublic;
            }
            var isSameModuleAsOwner = t.Module == this.ExpressionOwner.GetType().Module;
            var isAccessible = isPublic | isSameModuleAsOwner;
            var flag = !isAccessible;
            if (flag)
            {
                var msg = Utility.GetGeneralErrorMessage("TypeNotAccessibleToExpression", t.Name);
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
            var context = (ExpressionContext) this.MemberwiseClone();
            context.myProperties = this.myProperties.Clone();
            context.myProperties.SetValue("Options", this.Options.Clone());
            context.myProperties.SetValue("ParserOptions", this.ParserOptions.Clone());
            context.myProperties.SetValue("Imports", this.Imports.Clone());
            context.Imports.SetContext(context);
            if (cloneVariables)
            {
                context.Variables = new VariableCollection(this);
                this.Variables.Copy(context.Variables);
            }
            return context;
        }

        internal void AssertTypeIsAccessible(Type t)
        {
            var isNested = t.IsNested;
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
            var mySyncRoot = this.mySyncRoot;
            ObjectFlowControl.CheckForSyncLockOnValueType(mySyncRoot);
            ExpressionElement parse;
            lock (mySyncRoot)
            {
                var sr = new StringReader(expression);
                var parser = this.Parser;
                parser.Reset();
                parser.Tokenizer.Reset(sr);
                var analyzer = (FleeExpressionAnalyzer) parser.Analyzer;
                analyzer.SetServices(services);
                var rootNode = this.DoParse();
                analyzer.Reset();
                var topElement = (ExpressionElement) rootNode.ValuesArrayList[0];
                parse = topElement;
            }
            return parse;
        }

        internal void RecreateParser()
        {
            var mySyncRoot = this.mySyncRoot;
            ObjectFlowControl.CheckForSyncLockOnValueType(mySyncRoot);
            lock (mySyncRoot)
            {
                var analyzer = new FleeExpressionAnalyzer();
                var parser = new ExpressionParser(TextReader.Null, analyzer, this);
                this.myProperties.SetValue("ExpressionParser", parser);
            }
        }

        internal Node DoParse()
        {
            Node doParse;
            try
            {
                doParse = this.Parser.Parse();
            }
            catch (ParserLogException expr10)
            {
                ProjectData.SetProjectError(expr10);
                var ex = expr10;
                throw new ExpressionCompileException(ex);
            }
            return doParse;
        }

        internal void SetCalcEngine(CalculationEngine.CalculationEngine engine, string calcEngineExpressionName)
        {
            this.myProperties.SetValue("CalculationEngine", engine);
            this.myProperties.SetValue("CalcEngineExpressionName", calcEngineExpressionName);
        }

        internal IdentifierAnalyzer ParseIdentifiers(string expression)
        {
            var parser = this.IdentifierParser;
            var sr = new StringReader(expression);
            parser.Reset();
            parser.Tokenizer.Reset(sr);
            var analyzer = (IdentifierAnalyzer) parser.Analyzer;
            analyzer.Reset();
            parser.Parse();
            return (IdentifierAnalyzer) parser.Analyzer;
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

        internal string CalcEngineExpressionName
        {
            get { return this.myProperties.GetValue<string>("CalcEngineExpressionName"); }
        }

        public CalculationEngine.CalculationEngine CalculationEngine
        {
            get { return this.myProperties.GetValue<CalculationEngine.CalculationEngine>("CalculationEngine"); }
        }

        internal object ExpressionOwner
        {
            get { return this.myProperties.GetValue<object>("ExpressionOwner"); }
        }

        private ExpressionParser IdentifierParser
        {
            get
            {
                var parser = this.myProperties.GetValue<ExpressionParser>("IdentifierParser");
                var flag = parser == null;
                if (flag)
                {
                    var analyzer = new IdentifierAnalyzer();
                    parser = new ExpressionParser(TextReader.Null, analyzer, this);
                    this.myProperties.SetValue("IdentifierParser", parser);
                }
                return parser;
            }
        }

        public ExpressionImports Imports
        {
            get { return this.myProperties.GetValue<ExpressionImports>("Imports"); }
        }

        internal bool NoClone
        {
            get { return this.myProperties.GetValue<bool>("NoClone"); }
            set { this.myProperties.SetValue("NoClone", value); }
        }

        public ExpressionOptions Options
        {
            get { return this.myProperties.GetValue<ExpressionOptions>("Options"); }
        }

        internal ExpressionParser Parser
        {
            get { return this.myProperties.GetValue<ExpressionParser>("ExpressionParser"); }
        }

        public ExpressionParserOptions ParserOptions
        {
            get { return this.myProperties.GetValue<ExpressionParserOptions>("ParserOptions"); }
        }

        public VariableCollection Variables { get; private set; }
    }
}