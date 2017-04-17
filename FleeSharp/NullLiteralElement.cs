using System;
using System.Reflection.Emit;

namespace Flee
{
    internal class NullLiteralElement : LiteralElement
    {
        public override Type ResultType
        {
            get
            {
                return typeof(Null);
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            ilg.Emit(OpCodes.Ldnull);
        }
    }
}