using System;
using System.Runtime.CompilerServices;

namespace Flee
{
    public class ResolveVariableValueEventArgs : EventArgs
    {
        private object myValue;

        public string VariableName { get; }

        public Type VariableType { get; }

        public object VariableValue
        {
            get
            {
                return this.myValue;
            }
            set
            {
                this.myValue = RuntimeHelpers.GetObjectValue(value);
            }
        }

        internal ResolveVariableValueEventArgs(string name, Type t)
        {
            this.VariableName = name;
            this.VariableType = t;
        }
    }
}