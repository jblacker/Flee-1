using System;

namespace Ciloci.Flee
{
	public class ResolveVariableTypeEventArgs : EventArgs
	{
		private string MyName;

		private Type MyType;

		public string VariableName
		{
			get
			{
				return this.MyName;
			}
		}

		public Type VariableType
		{
			get
			{
				return this.MyType;
			}
			set
			{
				this.MyType = value;
			}
		}

		internal ResolveVariableTypeEventArgs(string name)
		{
			this.MyName = name;
		}
	}
}
