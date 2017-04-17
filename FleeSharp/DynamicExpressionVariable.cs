using System;

namespace Flee
{
    internal class DynamicExpressionVariable<T> : IVariable, IGenericVariable<T>
    {
        private IDynamicExpression myExpression;

        public object ValueAsObject
        {
            get
            {
                return this.myExpression;
            }
            set
            {
                this.myExpression = (IDynamicExpression)value;
            }
        }

        public Type VariableType => this.myExpression.Context.Options.ResultType;

        public IVariable Clone()
        {
            return new DynamicExpressionVariable<T>
            {
                myExpression = this.myExpression
            };
        }

        public T GetValue()
        {
            return (T)((object)this.myExpression.Evaluate());
        }
    }
}