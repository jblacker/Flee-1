using System;
using System.Reflection.Emit;

namespace Flee
{
    internal class NullLiteralElement : LiteralElement
    {
        public override Type ResultType => typeof(Null);

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            ilg.Emit(OpCodes.Ldnull);
        }
    }
}