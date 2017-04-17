namespace Flee.CalculationEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class NodeEventArgs : EventArgs
	{
		private string nodeEventName;

		private object nodeEventResult;

		public string Name => this.nodeEventName;

	    public object Result => this.nodeEventResult;

	    internal NodeEventArgs()
		{
		}

		internal void SetData(string name, object result)
		{
			this.nodeEventName = name;
			this.nodeEventResult = RuntimeHelpers.GetObjectValue(result);
		}
	}
}
