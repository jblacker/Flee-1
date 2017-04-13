using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class ConditionalElement : ExpressionElement
	{
		private ExpressionElement MyCondition;

		private ExpressionElement MyWhenTrue;

		private ExpressionElement MyWhenFalse;

		private Type MyResultType;

		public override Type ResultType
		{
			get
			{
				return this.MyResultType;
			}
		}

		public ConditionalElement(ExpressionElement condition, ExpressionElement whenTrue, ExpressionElement whenFalse)
		{
			this.MyCondition = condition;
			this.MyWhenTrue = whenTrue;
			this.MyWhenFalse = whenFalse;
			bool flag = this.MyCondition.ResultType != typeof(bool);
			if (flag)
			{
				base.ThrowCompileException("FirstArgNotBoolean", CompileExceptionReason.TypeMismatch, new object[0]);
			}
			bool flag2 = ImplicitConverter.EmitImplicitConvert(this.MyWhenFalse.ResultType, this.MyWhenTrue.ResultType, null);
			if (flag2)
			{
				this.MyResultType = this.MyWhenTrue.ResultType;
			}
			else
			{
				bool flag3 = ImplicitConverter.EmitImplicitConvert(this.MyWhenTrue.ResultType, this.MyWhenFalse.ResultType, null);
				if (flag3)
				{
					this.MyResultType = this.MyWhenFalse.ResultType;
				}
				else
				{
					base.ThrowCompileException("NeitherArgIsConvertibleToTheOther", CompileExceptionReason.TypeMismatch, new object[]
					{
						this.MyWhenTrue.ResultType.Name,
						this.MyWhenFalse.ResultType.Name
					});
				}
			}
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			BranchManager bm = new BranchManager();
			bm.GetLabel("falseLabel", ilg);
			bm.GetLabel("endLabel", ilg);
			bool isTemp = ilg.IsTemp;
			if (isTemp)
			{
				this.EmitConditional(ilg, services, bm);
			}
			else
			{
				FleeILGenerator ilgTemp = base.CreateTempFleeILGenerator(ilg);
				Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);
				this.EmitConditional(ilgTemp, services, bm);
				bm.ComputeBranches();
				this.EmitConditional(ilg, services, bm);
			}
		}

		private void EmitConditional(FleeILGenerator ilg, IServiceProvider services, BranchManager bm)
		{
			Label falseLabel = bm.FindLabel("falseLabel");
			Label endLabel = bm.FindLabel("endLabel");
			this.MyCondition.Emit(ilg, services);
			bool isTemp = ilg.IsTemp;
			if (isTemp)
			{
				bm.AddBranch(ilg, falseLabel);
				ilg.Emit(OpCodes.Brfalse_S, falseLabel);
			}
			else
			{
				bool flag = !bm.IsLongBranch(ilg, falseLabel);
				if (flag)
				{
					ilg.Emit(OpCodes.Brfalse_S, falseLabel);
				}
				else
				{
					ilg.Emit(OpCodes.Brfalse, falseLabel);
				}
			}
			this.MyWhenTrue.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyWhenTrue.ResultType, this.MyResultType, ilg);
			bool isTemp2 = ilg.IsTemp;
			if (isTemp2)
			{
				bm.AddBranch(ilg, endLabel);
				ilg.Emit(OpCodes.Br_S, endLabel);
			}
			else
			{
				bool flag2 = !bm.IsLongBranch(ilg, endLabel);
				if (flag2)
				{
					ilg.Emit(OpCodes.Br_S, endLabel);
				}
				else
				{
					ilg.Emit(OpCodes.Br, endLabel);
				}
			}
			bm.MarkLabel(ilg, falseLabel);
			ilg.MarkLabel(falseLabel);
			this.MyWhenFalse.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyWhenFalse.ResultType, this.MyResultType, ilg);
			bm.MarkLabel(ilg, endLabel);
			ilg.MarkLabel(endLabel);
		}
	}
}
