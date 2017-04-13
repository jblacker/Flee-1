using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal abstract class ExpressionElement
	{
		public abstract Type ResultType
		{
			get;
		}

		protected string Name
		{
			get
			{
				string key = base.GetType().Name;
				string value = FleeResourceManager.Instance.GetElementNameString(key);
				Debug.Assert(value != null, string.Format("Element name for '{0}' not in resource file", key));
				return value;
			}
		}

		internal ExpressionElement()
		{
		}

		public abstract void Emit(FleeILGenerator ilg, IServiceProvider services);

		public override string ToString()
		{
			return this.Name;
		}

		protected void ThrowCompileException(string messageKey, CompileExceptionReason reason, params object[] arguments)
		{
			string messageTemplate = FleeResourceManager.Instance.GetCompileErrorString(messageKey);
			string message = string.Format(messageTemplate, arguments);
			message = this.Name + ": " + message;
			throw new ExpressionCompileException(message, reason);
		}

		protected void ThrowAmbiguousCallException(Type leftType, Type rightType, object operation)
		{
			this.ThrowCompileException("AmbiguousOverloadedOperator", CompileExceptionReason.AmbiguousMatch, new object[]
			{
				leftType.Name,
				rightType.Name,
				operation
			});
		}

		protected FleeILGenerator CreateTempFleeILGenerator(FleeILGenerator ilgCurrent)
		{
			DynamicMethod dm = new DynamicMethod("temp", typeof(int), null, base.GetType());
			return new FleeILGenerator(dm.GetILGenerator(), ilgCurrent.Length, true);
		}
	}
}
