using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class Expression<T> : IDynamicExpression, IGenericExpression<T>
    {
        private readonly string myExpression;
        private ExpressionContext myContext;
        private ExpressionOptions myOptions;
        private readonly ExpressionInfo myInfo;
        private ExpressionEvaluator<T> myEvaluator;

        private object myOwner;
        private const string EmitAssemblyName = "FleeExpression";

        private const string DynamicMethodName = "Flee Expression";
        public Expression(string expression, ExpressionContext context, bool isGeneric)
        {
            Utility.AssertNotNull(expression, "expression");
            this.myExpression = expression;
            this.myOwner = context.ExpressionOwner;

            this.myContext = context;

            if (context.NoClone == false)
            {
                this.myContext = context.CloneInternal(false);
            }

            this.myInfo = new ExpressionInfo();

            this.SetupOptions(this.myContext.Options, isGeneric);

            this.myContext.Imports.ImportOwner(this.myOptions.OwnerType);

            this.ValidateOwner(this.myOwner);

            this.Compile(expression, this.myOptions);

            this.myContext.CalculationEngine?.FixTemporaryHead(this, this.myContext, this.myOptions.ResultType);
        }

        private void SetupOptions(ExpressionOptions options, bool isGeneric)
        {
            // Make sure we clone the options
            this.myOptions = options;
            this.myOptions.IsGeneric = isGeneric;

            if (isGeneric)
            {
                this.myOptions.ResultType = typeof(T);
            }

            this.myOptions.SetOwnerType(this.myOwner.GetType());
        }

        private void Compile(string expression, ExpressionOptions options)
        {
            // Add the services that will be used by elements during the compile
            IServiceContainer services = new ServiceContainer();
            this.AddServices(services);

            // Parse and get the root element of the parse tree
            var topElement = this.myContext.Parse(expression, services);

            if (options.ResultType == null)
            {
                options.ResultType = topElement.ResultType;
            }

            var rootElement = new RootExpressionElement(topElement, options.ResultType);

            var dm = this.CreateDynamicMethod();

            var ilg = new FleeILGenerator(dm.GetILGenerator());

            // Emit the IL
            rootElement.Emit(ilg, services);

            ilg.ValidateLength();

            // Emit to an assembly if required
            if (options.EmitToAssembly)
            {
                EmitToAssembly(rootElement, services);
            }

            var delegateType = typeof(ExpressionEvaluator<>).MakeGenericType(typeof(T));
            this.myEvaluator = (ExpressionEvaluator<T>)dm.CreateDelegate(delegateType);
        }

        private DynamicMethod CreateDynamicMethod()
        {
            // Create the dynamic method
            Type[] parameterTypes = {
            typeof(object),
            typeof(ExpressionContext),
            typeof(VariableCollection)
        };
            return new DynamicMethod(DynamicMethodName, typeof(T), parameterTypes, this.myOptions.OwnerType);
        }

        private void AddServices(IServiceContainer dest)
        {
            dest.AddService(typeof(ExpressionOptions), this.myOptions);
            dest.AddService(typeof(ExpressionParserOptions), this.myContext.ParserOptions);
            dest.AddService(typeof(ExpressionContext), this.myContext);
            dest.AddService(typeof(IExpression), this);
            dest.AddService(typeof(ExpressionInfo), this.myInfo);
        }

        private static void EmitToAssembly(ExpressionElement rootElement, IServiceContainer services)
        {
            var assemblyName = new AssemblyName(EmitAssemblyName);

            var assemblyFileName = $"{EmitAssemblyName}.dll";

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyFileName, assemblyFileName);

            var mb = moduleBuilder.DefineGlobalMethod("Evaluate", MethodAttributes.Public | MethodAttributes.Static, typeof(T), new[] {
            typeof(object),
            typeof(ExpressionContext),
            typeof(VariableCollection)
        });
            var ilg = new FleeILGenerator(mb.GetILGenerator());

            rootElement.Emit(ilg, services);

            moduleBuilder.CreateGlobalFunctions();
            assemblyBuilder.Save(assemblyFileName);
        }

        private void ValidateOwner(object owner)
        {
            Utility.AssertNotNull(owner, "owner");
            if (this.myOptions.OwnerType.IsInstanceOfType(owner) == false)
            {
                var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.NewOwnerTypeNotAssignableToCurrentOwner);
                throw new ArgumentException(msg);
            }
        }

        public object Evaluate()
        {
            return this.myEvaluator(this.myOwner, this.myContext, this.myContext.Variables);
        }

        public T EvaluateGeneric()
        {
            return this.myEvaluator(this.myOwner, this.myContext, this.myContext.Variables);
        }
        T IGenericExpression<T>.Evaluate()
        {
            return EvaluateGeneric();
        }

        public IExpression Clone()
        {
            var copy = (Expression<T>)this.MemberwiseClone();
            copy.myContext = this.myContext.CloneInternal(true);
            copy.myOptions = copy.myContext.Options;
            return copy;
        }

        public override string ToString()
        {
            return this.myExpression;
        }

        internal Type ResultType => this.myOptions.ResultType;

        public string Text => this.myExpression;

        public ExpressionInfo Info1 => this.myInfo;

        ExpressionInfo IExpression.Info => Info1;

        public object Owner
        {
            get { return this.myOwner; }
            set
            {
                this.ValidateOwner(value);
                this.myOwner = value;
            }
        }

        public ExpressionContext Context => this.myContext;
    }
}