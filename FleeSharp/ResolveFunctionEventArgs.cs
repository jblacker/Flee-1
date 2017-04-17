using System;

namespace Flee
{
    public class ResolveFunctionEventArgs : EventArgs
    {
        public string FunctionName { get; }

        public Type[] ArgumentTypes { get; }

        public Type ReturnType { get; set; }

        internal ResolveFunctionEventArgs(string name, Type[] argumentTypes)
        {
            this.FunctionName = name;
            this.ArgumentTypes = argumentTypes;
        }
    }
}