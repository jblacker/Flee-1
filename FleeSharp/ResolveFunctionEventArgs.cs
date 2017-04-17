using System;

namespace Flee
{
    public class ResolveFunctionEventArgs : EventArgs
    {
        private string MyName;

        private Type[] MyArgumentTypes;

        private Type MyReturnType;

        public string FunctionName
        {
            get
            {
                return this.MyName;
            }
        }

        public Type[] ArgumentTypes
        {
            get
            {
                return this.MyArgumentTypes;
            }
        }

        public Type ReturnType
        {
            get
            {
                return this.MyReturnType;
            }
            set
            {
                this.MyReturnType = value;
            }
        }

        internal ResolveFunctionEventArgs(string name, Type[] argumentTypes)
        {
            this.MyName = name;
            this.MyArgumentTypes = argumentTypes;
        }
    }
}