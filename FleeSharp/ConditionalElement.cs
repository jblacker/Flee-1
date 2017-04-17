namespace Flee
{
    using System;
    using System.Reflection.Emit;

    internal class ConditionalElement : ExpressionElement
    {
        private readonly ExpressionElement myCondition;

        private readonly Type myResultType;

        private readonly ExpressionElement myWhenFalse;

        private readonly ExpressionElement myWhenTrue;

        public ConditionalElement(ExpressionElement condition, ExpressionElement whenTrue, ExpressionElement whenFalse)
        {
            this.myCondition = condition;
            this.myWhenTrue = whenTrue;
            this.myWhenFalse = whenFalse;
            var flag = this.myCondition.ResultType != typeof(bool);
            if (flag)
            {
                this.ThrowCompileException("FirstArgNotBoolean", CompileExceptionReason.TypeMismatch);
            }
            var flag2 = ImplicitConverter.EmitImplicitConvert(this.myWhenFalse.ResultType, this.myWhenTrue.ResultType, null);
            if (flag2)
            {
                this.myResultType = this.myWhenTrue.ResultType;
            }
            else
            {
                var flag3 = ImplicitConverter.EmitImplicitConvert(this.myWhenTrue.ResultType, this.myWhenFalse.ResultType, null);
                if (flag3)
                {
                    this.myResultType = this.myWhenFalse.ResultType;
                }
                else
                {
                    this.ThrowCompileException("NeitherArgIsConvertibleToTheOther", CompileExceptionReason.TypeMismatch,
                        this.myWhenTrue.ResultType.Name, this.myWhenFalse.ResultType.Name);
                }
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var bm = new BranchManager();
            bm.GetLabel("falseLabel", ilg);
            bm.GetLabel("endLabel", ilg);
            var isTemp = ilg.IsTemp;
            if (isTemp)
            {
                this.EmitConditional(ilg, services, bm);
            }
            else
            {
                var ilgTemp = this.CreateTempFleeIlGenerator(ilg);
                Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);
                this.EmitConditional(ilgTemp, services, bm);
                bm.ComputeBranches();
                this.EmitConditional(ilg, services, bm);
            }
        }

        private void EmitConditional(FleeIlGenerator ilg, IServiceProvider services, BranchManager bm)
        {
            var falseLabel = bm.FindLabel("falseLabel");
            var endLabel = bm.FindLabel("endLabel");
            this.myCondition.Emit(ilg, services);
            var isTemp = ilg.IsTemp;
            if (isTemp)
            {
                bm.AddBranch(ilg, falseLabel);
                ilg.Emit(OpCodes.Brfalse_S, falseLabel);
            }
            else
            {
                var flag = !bm.IsLongBranch(ilg, falseLabel);
                ilg.Emit(flag ? OpCodes.Brfalse_S : OpCodes.Brfalse, falseLabel);
            }
            this.myWhenTrue.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myWhenTrue.ResultType, this.myResultType, ilg);
            var isTemp2 = ilg.IsTemp;
            if (isTemp2)
            {
                bm.AddBranch(ilg, endLabel);
                ilg.Emit(OpCodes.Br_S, endLabel);
            }
            else
            {
                var flag2 = !bm.IsLongBranch(ilg, endLabel);
                ilg.Emit(flag2 ? OpCodes.Br_S : OpCodes.Br, endLabel);
            }
            bm.MarkLabel(ilg, falseLabel);
            ilg.MarkLabel(falseLabel);
            this.myWhenFalse.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myWhenFalse.ResultType, this.myResultType, ilg);
            bm.MarkLabel(ilg, endLabel);
            ilg.MarkLabel(endLabel);
        }

        public override Type ResultType => this.myResultType;
    }
}