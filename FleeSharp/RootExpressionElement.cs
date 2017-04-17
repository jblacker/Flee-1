using System;
using System.Reflection.Emit;

namespace Flee
{
    internal class RootExpressionElement : ExpressionElement
    {
        private readonly ExpressionElement myChild;

        private readonly Type myResultType;

        public override Type ResultType => typeof(object);

        public RootExpressionElement(ExpressionElement child, Type resultType)
        {
            this.myChild = child;
            this.myResultType = resultType;
            this.Validate();
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myChild.ResultType, this.myResultType, ilg);
            var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
            var flag = !options.IsGeneric;
            if (flag)
            {
                ImplicitConverter.EmitImplicitConvert(this.myResultType, typeof(object), ilg);
            }
            ilg.Emit(OpCodes.Ret);
        }

        private void Validate()
        {
            var flag = !ImplicitConverter.EmitImplicitConvert(this.myChild.ResultType, this.myResultType, null);
            if (flag)
            {
                this.ThrowCompileException("CannotConvertTypeToExpressionResult", CompileExceptionReason.TypeMismatch, this.myChild.ResultType.Name, this.myResultType.Name);
            }
        }
    }
}