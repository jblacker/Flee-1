using System;

namespace Flee
{
    internal class GenericVariable<T> : IVariable, IGenericVariable<T>
    {
        public T myValue;

        public Type VariableType => typeof(T);

        public object ValueAsObject
        {
            get
            {
                return this.myValue;
            }
            set
            {
                bool flag = value == null;
                if (flag)
                {
                    this.myValue = default(T);
                }
                else
                {
                    this.myValue = (T)((object)value);
                }
            }
        }

        public IVariable Clone()
        {
            return new GenericVariable<T>
            {
                myValue = this.myValue
            };
        }

        public T GetValue()
        {
            return this.myValue;
        }
    }
}