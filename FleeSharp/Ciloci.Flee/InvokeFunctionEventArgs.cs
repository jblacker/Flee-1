using System;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	public class InvokeFunctionEventArgs : EventArgs
	{
		private string MyName;

		private object[] MyArguments;

		private object MyFunctionResult;

		public string FunctionName
		{
			get
			{
				return this.MyName;
			}
		}

		public object[] Arguments
		{
			get
			{
				return this.MyArguments;
			}
		}

		public object Result
		{
			get
			{
				return this.MyFunctionResult;
			}
			set
			{
				this.MyFunctionResult = RuntimeHelpers.GetObjectValue(value);
			}
		}

		internal InvokeFunctionEventArgs(string name, object[] arguments)
		{
			this.MyName = name;
			this.MyArguments = arguments;
		}
	}
}
