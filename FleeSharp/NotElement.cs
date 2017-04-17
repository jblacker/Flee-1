using System;
using System.Reflection.Emit;

namespace Flee
{
    internal class NotElement : UnaryElement
    {
        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            bool flag = this.myChild.ResultType == typeof(bool);
            if (flag)
            {
                this.EmitLogical(ilg, services);
            }
            else
            {
                this.myChild.Emit(ilg, services);
                ilg.Emit(OpCodes.Not);
            }
        }

        private void EmitLogical(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myChild.Emit(ilg, services);
            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Ceq);
        }

        protected override Type GetResultType(Type childType)
        {
            bool flag = childType == typeof(bool);
            Type getResultType;
            if (flag)
            {
                getResultType = typeof(bool);
            }
            else
            {
                bool flag2 = Utility.IsIntegralType(childType);
                getResultType = flag2 ? childType : null;
            }
            return getResultType;
        }
    }
}