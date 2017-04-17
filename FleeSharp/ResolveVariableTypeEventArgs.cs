using System;

namespace Flee
{
    public class ResolveVariableTypeEventArgs : EventArgs
    {
        public string VariableName { get; }

        public Type VariableType { get; set; }

        internal ResolveVariableTypeEventArgs(string name)
        {
            this.VariableName = name;
        }
    }
}