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

namespace FleeSharp
{
    using System;
    using System.ComponentModel.Design;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class Expression<T> : IDynamicExpression, IGenericExpression<T>
    {
        private const string EmitAssemblyName = "FleeExpression";

        private const string DynamicMethodName = "Flee Expression";
        private ExpressionEvaluator<T> myEvaluator;
        private ExpressionOptions myOptions;

        private object myOwner;

        public Expression(string expression, ExpressionContext context, bool isGeneric)
        {
            Utility.AssertNotNull(expression, "expression");
            this.Text = expression;
            this.myOwner = context.ExpressionOwner;

            this.Context = context;

            if (context.NoClone == false)
            {
                this.Context = context.CloneInternal(false);
            }

            this.Info1 = new ExpressionInfo();

            this.SetupOptions(this.Context.Options, isGeneric);

            this.Context.Imports.ImportOwner(this.myOptions.OwnerType);

            this.ValidateOwner(this.myOwner);

            this.Compile(expression, this.myOptions);

            this.Context.CalculationEngine?.FixTemporaryHead(this, this.Context, this.myOptions.ResultType);
        }

        public IExpression Clone()
        {
            var copy = (Expression<T>) this.MemberwiseClone();
            copy.Context = this.Context.CloneInternal(true);
            copy.myOptions = copy.Context.Options;
            return copy;
        }

        public ExpressionContext Context { get; private set; }

        public object Evaluate()
        {
            return this.myEvaluator(this.myOwner, this.Context, this.Context.Variables);
        }

        T IGenericExpression<T>.Evaluate()
        {
            return this.EvaluateGeneric();
        }

        ExpressionInfo IExpression.Info => this.Info1;

        public object Owner
        {
            get { return this.myOwner; }
            set
            {
                this.ValidateOwner(value);
                this.myOwner = value;
            }
        }

        public string Text { get; }

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
            var topElement = this.Context.Parse(expression, services);

            if (options.ResultType == null)
            {
                options.ResultType = topElement.ResultType;
            }

            var rootElement = new RootExpressionElement(topElement, options.ResultType);

            var dm = this.CreateDynamicMethod();

            var ilg = new FleeIlGenerator(dm.GetILGenerator());

            // Emit the IL
            rootElement.Emit(ilg, services);

            ilg.ValidateLength();

            // Emit to an assembly if required
            if (options.EmitToAssembly)
            {
                EmitToAssembly(rootElement, services);
            }

            var delegateType = typeof(ExpressionEvaluator<>).MakeGenericType(typeof(T));
            this.myEvaluator = (ExpressionEvaluator<T>) dm.CreateDelegate(delegateType);
        }

        private DynamicMethod CreateDynamicMethod()
        {
            // Create the dynamic method
            Type[] parameterTypes =
            {
                typeof(object),
                typeof(ExpressionContext),
                typeof(VariableCollection)
            };
            return new DynamicMethod(DynamicMethodName, typeof(T), parameterTypes, this.myOptions.OwnerType);
        }

        private void AddServices(IServiceContainer dest)
        {
            dest.AddService(typeof(ExpressionOptions), this.myOptions);
            dest.AddService(typeof(ExpressionParserOptions), this.Context.ParserOptions);
            dest.AddService(typeof(ExpressionContext), this.Context);
            dest.AddService(typeof(IExpression), this);
            dest.AddService(typeof(ExpressionInfo), this.Info1);
        }

        private static void EmitToAssembly(ExpressionElement rootElement, IServiceContainer services)
        {
            var assemblyName = new AssemblyName(EmitAssemblyName);

            var assemblyFileName = $"{EmitAssemblyName}.dll";

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyFileName, assemblyFileName);

            var mb = moduleBuilder.DefineGlobalMethod("Evaluate", MethodAttributes.Public | MethodAttributes.Static, typeof(T), new[]
            {
                typeof(object),
                typeof(ExpressionContext),
                typeof(VariableCollection)
            });
            var ilg = new FleeIlGenerator(mb.GetILGenerator());

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

        public T EvaluateGeneric()
        {
            return this.myEvaluator(this.myOwner, this.Context, this.Context.Variables);
        }

        public override string ToString()
        {
            return this.Text;
        }

        public ExpressionInfo Info1 { get; }

        internal Type ResultType => this.myOptions.ResultType;
    }
}