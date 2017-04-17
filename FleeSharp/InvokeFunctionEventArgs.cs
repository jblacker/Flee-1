using System;
using System.Runtime.CompilerServices;

namespace Flee
{
    public class InvokeFunctionEventArgs : EventArgs
    {
        private object myFunctionResult;

        public string FunctionName { get; set; }

        public object[] Arguments { get; set; }

        public object Result
        {
            get
            {
                return this.myFunctionResult;
            }
            set
            {
                this.myFunctionResult = RuntimeHelpers.GetObjectValue(value);
            }
        }

        internal InvokeFunctionEventArgs(string name, object[] arguments)
        {
            this.FunctionName = name;
            this.Arguments = arguments;
        }
    }
}