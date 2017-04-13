using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	internal class Expression<T> : IExpression, IDynamicExpression, IGenericExpression<T>
	{
		private readonly string myExpression;

		private ExpressionContext myContext;

		private ExpressionOptions myOptions;

		private readonly ExpressionInfo myInfo;

		private ExpressionEvaluator<T> myEvaluator;

		private object myOwner;

		private const string EmitAssemblyName = "FleeExpression";

		private const string DynamicMethodName = "Flee Expression";

		internal Type ResultType => this.myOptions.ResultType;

	    public string Text => this.myExpression;

	    public ExpressionInfo Info1 => this.myInfo;

	    public object Owner
		{
			get
			{
				return this.myOwner;
			}
			set
			{
				this.ValidateOwner(RuntimeHelpers.GetObjectValue(value));
				this.myOwner = RuntimeHelpers.GetObjectValue(value);
			}
		}

		public ExpressionContext Context => this.myContext;

	    public Expression(string expression, ExpressionContext context, bool isGeneric)
		{
			Utility.AssertNotNull(expression, "expression");
			this.myExpression = expression;
			this.myOwner = RuntimeHelpers.GetObjectValue(context.ExpressionOwner);
			this.myContext = context;
			bool flag = !context.NoClone;
			if (flag)
			{
				this.myContext = context.CloneInternal(false);
			}
			this.myInfo = new ExpressionInfo();
			this.SetupOptions(this.myContext.Options, isGeneric);
			this.myContext.Imports.ImportOwner(this.myOptions.OwnerType);
			this.ValidateOwner(RuntimeHelpers.GetObjectValue(this.myOwner));
			this.Compile(expression, this.myOptions);
			bool flag2 = this.myContext.CalculationEngine != null;
			if (flag2)
			{
				this.myContext.CalculationEngine.FixTemporaryHead(this, this.myContext, this.myOptions.ResultType);
			}
		}

		private void SetupOptions(ExpressionOptions options, bool isGeneric)
		{
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
			IServiceContainer services = new ServiceContainer();
			this.AddServices(services);
			ExpressionElement topElement = this.myContext.Parse(expression, services);
			bool flag = options.ResultType == null;
			if (flag)
			{
				options.ResultType = topElement.ResultType;
			}
			RootExpressionElement rootElement = new RootExpressionElement(topElement, options.ResultType);
			DynamicMethod dm = this.CreateDynamicMethod();
			FleeILGenerator ilg = new FleeILGenerator(dm.GetILGenerator(), 0, false);
			rootElement.Emit(ilg, services);
			ilg.ValidateLength();
			bool emitToAssembly = options.EmitToAssembly;
			if (emitToAssembly)
			{
				Expression<T>.EmitToAssembly(rootElement, services);
			}
			Type delegateType = typeof(ExpressionEvaluator<>).MakeGenericType(new Type[]
			{
				typeof(T)
			});
			this.myEvaluator = (ExpressionEvaluator<T>)dm.CreateDelegate(delegateType);
		}

		private DynamicMethod CreateDynamicMethod()
		{
			Type[] parameterTypes = new Type[]
			{
				typeof(object),
				typeof(ExpressionContext),
				typeof(VariableCollection)
			};
			return new DynamicMethod("Flee Expression", typeof(T), parameterTypes, this.myOptions.OwnerType);
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
			AssemblyName assemblyName = new AssemblyName("FleeExpression");
			string assemblyFileName = string.Format("{0}.dll", "FleeExpression");
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyFileName, assemblyFileName);
			MethodBuilder mb = moduleBuilder.DefineGlobalMethod("Evaluate", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, typeof(T), new Type[]
			{
				typeof(object),
				typeof(ExpressionContext),
				typeof(VariableCollection)
			});
			FleeILGenerator ilg = new FleeILGenerator(mb.GetILGenerator(), 0, false);
			rootElement.Emit(ilg, services);
			moduleBuilder.CreateGlobalFunctions();
			assemblyBuilder.Save(assemblyFileName);
		}

		private void ValidateOwner(object owner)
		{
			Utility.AssertNotNull(RuntimeHelpers.GetObjectValue(owner), "owner");
			bool flag = !this.myOptions.OwnerType.IsAssignableFrom(owner.GetType());
			if (flag)
			{
				string msg = Utility.GetGeneralErrorMessage("NewOwnerTypeNotAssignableToCurrentOwner", new object[0]);
				throw new ArgumentException(msg);
			}
		}

		public object Evaluate()
		{
			return this.myEvaluator(RuntimeHelpers.GetObjectValue(this.myOwner), this.myContext, this.myContext.Variables);
		}

		public T EvaluateGeneric()
		{
			return this.myEvaluator(RuntimeHelpers.GetObjectValue(this.myOwner), this.myContext, this.myContext.Variables);
		}

		public IExpression Clone()
		{
			Expression<T> copy = (Expression<T>)base.MemberwiseClone();
			copy.myContext = this.myContext.CloneInternal(true);
			copy.myOptions = copy.myContext.Options;
			return copy;
		}

		public override string ToString()
		{
			return this.myExpression;
		}

	    public ExpressionInfo Info { get; }
	    T IGenericExpression<T>.Evaluate()
	    {
	        throw new NotImplementedException();
	    }
	}
}
