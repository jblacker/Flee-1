namespace Flee
{
    using System;
    using System.Reflection.Emit;

    internal class XorElement : BinaryExpressionElement
    {
        protected override Type GetResultType(Type leftType, Type rightType)
        {
            var bitwiseType = Utility.GetBitwiseOpType(leftType, rightType);
            var flag = bitwiseType != null;
            Type getResultType;
            if (flag)
            {
                getResultType = bitwiseType;
            }
            else
            {
                var flag2 = this.AreBothChildrenOfType(typeof(bool));
                getResultType = flag2 ? typeof(bool) : null;
            }
            return getResultType;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var resultType = this.ResultType;
            this.myLeftChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myLeftChild.ResultType, resultType, ilg);
            this.myRightChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myRightChild.ResultType, resultType, ilg);
            ilg.Emit(OpCodes.Xor);
        }

        protected override void GetOperation(object operation)
        {
        }
    }
}