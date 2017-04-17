using System;

namespace Flee
{
    internal abstract class UnaryElement : ExpressionElement
    {
        protected ExpressionElement myChild;

        private Type myResultType;

        public override Type ResultType => this.myResultType;

        public void SetChild(ExpressionElement child)
        {
            this.myChild = child;
            this.myResultType = this.GetResultType(child.ResultType);
            bool flag = this.myResultType == null;
            if (flag)
            {
                this.ThrowCompileException("OperationNotDefinedForType", CompileExceptionReason.TypeMismatch, new object[]
                {
                    this.myChild.ResultType.Name
                });
            }
        }

        protected abstract Type GetResultType(Type childType);
    }
}