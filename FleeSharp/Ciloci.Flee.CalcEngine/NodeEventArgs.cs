using System;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee.CalcEngine
{
	public sealed class NodeEventArgs : EventArgs
	{
		private string MyName;

		private object MyResult;

		public string Name
		{
			get
			{
				return this.MyName;
			}
		}

		public object Result
		{
			get
			{
				return this.MyResult;
			}
		}

		internal NodeEventArgs()
		{
		}

		internal void SetData(string name, object result)
		{
			this.MyName = name;
			this.MyResult = RuntimeHelpers.GetObjectValue(result);
		}
	}
}
