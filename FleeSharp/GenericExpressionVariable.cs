using System;

namespace Flee
{
    internal class GenericExpressionVariable<T> : IVariable, IGenericVariable<T>
    {
        private IGenericExpression<T> myExpression;

        public object ValueAsObject
        {
            get
            {
                return this.myExpression;
            }
            set
            {
                this.myExpression = (IGenericExpression<T>)value;
            }
        }

        public Type VariableType => this.myExpression.Context.Options.ResultType;

        public IVariable Clone()
        {
            return new GenericExpressionVariable<T>
            {
                myExpression = this.myExpression
            };
        }

        public T GetValue()
        {
            return this.myExpression.Evaluate();
        }
    }
}