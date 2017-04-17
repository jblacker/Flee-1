using System;
using System.Reflection.Emit;

namespace Flee
{
    internal class XorElement : BinaryExpressionElement
    {
        protected override Type GetResultType(Type leftType, Type rightType)
        {
            Type bitwiseType = Utility.GetBitwiseOpType(leftType, rightType);
            bool flag = bitwiseType != null;
            Type GetResultType;
            if (flag)
            {
                GetResultType = bitwiseType;
            }
            else
            {
                bool flag2 = this.AreBothChildrenOfType(typeof(bool));
                if (flag2)
                {
                    GetResultType = typeof(bool);
                }
                else
                {
                    GetResultType = null;
                }
            }
            return GetResultType;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            Type resultType = this.ResultType;
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