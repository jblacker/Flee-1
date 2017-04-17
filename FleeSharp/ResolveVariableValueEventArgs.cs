using System;
using System.Runtime.CompilerServices;

namespace Flee
{
    public class ResolveVariableValueEventArgs : EventArgs
    {
        private string MyName;

        private Type MyType;

        private object MyValue;

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
        }

        public object VariableValue
        {
            get
            {
                return this.MyValue;
            }
            set
            {
                this.MyValue = RuntimeHelpers.GetObjectValue(value);
            }
        }

        internal ResolveVariableValueEventArgs(string name, Type t)
        {
            this.MyName = name;
            this.MyType = t;
        }
    }
}