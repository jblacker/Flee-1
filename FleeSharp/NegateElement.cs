namespace Flee
{
    using System;
    using System.Reflection.Emit;

    internal class NegateElement : UnaryElement
    {
        protected override Type GetResultType(Type childType)
        {
            var tc = Type.GetTypeCode(childType);
            var mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", childType, childType);
            var flag = mi != null;
            Type getResultType;
            if (flag)
            {
                getResultType = mi.ReturnType;
            }
            else
            {
                switch (tc)
                {
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        getResultType = childType;
                        return getResultType;
                    case TypeCode.UInt32:
                        getResultType = typeof(long);
                        return getResultType;
                }
                getResultType = null;
            }
            return getResultType;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var resultType = this.ResultType;
            this.myChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myChild.ResultType, resultType, ilg);
            var mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", resultType, resultType);
            var flag = mi == null;
            if (flag)
            {
                ilg.Emit(OpCodes.Neg);
            }
            else
            {
                ilg.Emit(OpCodes.Call, mi);
            }
        }
    }
}