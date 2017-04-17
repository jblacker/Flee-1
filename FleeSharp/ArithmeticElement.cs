using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class ArithmeticElement : BinaryExpressionElement
    {
        private static readonly MethodInfo ourPowerMethodInfo;

        private static readonly MethodInfo ourStringConcatMethodInfo;

        private static readonly MethodInfo ourObjectConcatMethodInfo;

        private BinaryArithmeticOperation myOperation;

        private bool IsOptimizablePower
        {
            get
            {
                var flag = this.myOperation != BinaryArithmeticOperation.Power;
                bool isOptimizablePower;
                if (flag)
                {
                    isOptimizablePower = false;
                }
                else
                {
                    var right = this.myRightChild as Int32LiteralElement;
                    var flag2 = right == null;
                    isOptimizablePower = (!flag2 && right.Value >= 0);
                }
                return isOptimizablePower;
            }
        }

        static ArithmeticElement()
        {
            ourPowerMethodInfo = typeof(Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
            ourStringConcatMethodInfo = typeof(string).GetMethod("Concat", BindingFlags.Static | BindingFlags.Public, null, new[]
            {
                typeof(string),
                typeof(string)
            }, null);
            ourObjectConcatMethodInfo = typeof(string).GetMethod("Concat", BindingFlags.Static | BindingFlags.Public, null, new[]
            {
                typeof(object),
                typeof(object)
            }, null);
        }

        protected override void GetOperation(object operation)
        {
            this.myOperation = (BinaryArithmeticOperation)operation;
        }

        protected override Type GetResultType(Type leftType, Type rightType)
        {
            var binaryResultType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
            var overloadedMethod = this.GetOverloadedArithmeticOperator();
            var flag = overloadedMethod != null;
            Type getResultType;
            if (flag)
            {
                getResultType = overloadedMethod.ReturnType;
            }
            else
            {
                var flag2 = binaryResultType != null;
                if (flag2)
                {
                    var flag3 = this.myOperation == BinaryArithmeticOperation.Power;
                    getResultType = flag3 ? this.GetPowerResultType(leftType) : binaryResultType;
                }
                else
                {
                    var flag4 = this.IsEitherChildOfType(typeof(string)) & (this.myOperation == BinaryArithmeticOperation.Add);
                    getResultType = flag4 ? typeof(string) : null;
                }
            }
            return getResultType;
        }


        // ReSharper disable UnusedParameter
        private Type GetPowerResultType(Type leftType)
        // ReSharper restore UnusedParameter
        {
            var isOptimizablePower = this.IsOptimizablePower;
            var getPowerResultType = isOptimizablePower ? leftType : typeof(double);
            return getPowerResultType;
        }

        private MethodInfo GetOverloadedArithmeticOperator()
        {
            var name = GetOverloadedOperatorFunctionName(this.myOperation);
            return this.GetOverloadedBinaryOperator(name, this.myOperation);
        }

        private static string GetOverloadedOperatorFunctionName(BinaryArithmeticOperation op)
        {
            string getOverloadedOperatorFunctionName = null;
            switch (op)
            {
                case BinaryArithmeticOperation.Add:
                    getOverloadedOperatorFunctionName = "Addition";
                    break;
                case BinaryArithmeticOperation.Subtract:
                    getOverloadedOperatorFunctionName = "Subtraction";
                    break;
                case BinaryArithmeticOperation.Multiply:
                    getOverloadedOperatorFunctionName = "Multiply";
                    break;
                case BinaryArithmeticOperation.Divide:
                    getOverloadedOperatorFunctionName = "Division";
                    break;
                case BinaryArithmeticOperation.Mod:
                    getOverloadedOperatorFunctionName = "Modulus";
                    break;
                case BinaryArithmeticOperation.Power:
                    getOverloadedOperatorFunctionName = "Exponent";
                    break;
                default:
                    Debug.Assert(false, "unknown operator type");
                    break;
            }
            return getOverloadedOperatorFunctionName;
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var overloadedMethod = this.GetOverloadedArithmeticOperator();
            var flag = overloadedMethod != null;
            if (flag)
            {
                this.EmitOverloadedOperatorCall(overloadedMethod, ilg, services);
            }
            else
            {
                var flag2 = this.IsEitherChildOfType(typeof(string));
                if (flag2)
                {
                    this.EmitStringConcat(ilg, services);
                }
                else
                {
                    this.EmitArithmeticOperation(this.myOperation, ilg, services);
                }
            }
        }

        private static bool IsUnsignedForArithmetic(Type t)
        {
            return t == typeof(uint) | t == typeof(ulong);
        }

        private void EmitArithmeticOperation(BinaryArithmeticOperation op, FleeIlGenerator ilg, IServiceProvider services)
        {
            var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
            var unsigned = IsUnsignedForArithmetic(this.myLeftChild.ResultType) & IsUnsignedForArithmetic(this.myRightChild.ResultType);
            var integral = Utility.IsIntegralType(this.myLeftChild.ResultType) & Utility.IsIntegralType(this.myRightChild.ResultType);
            var emitOverflow = integral & options.Checked;
            EmitChildWithConvert(this.myLeftChild, this.ResultType, ilg, services);
            var flag = !this.IsOptimizablePower;
            if (flag)
            {
                EmitChildWithConvert(this.myRightChild, this.ResultType, ilg, services);
            }
            switch (op)
            {
                case BinaryArithmeticOperation.Add:
                    {
                        var flag2 = emitOverflow;
                        if (flag2)
                        {
                            var flag3 = unsigned;
                            ilg.Emit(flag3 ? OpCodes.Add_Ovf_Un : OpCodes.Add_Ovf);
                        }
                        else
                        {
                            ilg.Emit(OpCodes.Add);
                        }
                        break;
                    }
                case BinaryArithmeticOperation.Subtract:
                    {
                        var flag4 = emitOverflow;
                        if (flag4)
                        {
                            var flag5 = unsigned;
                            ilg.Emit(flag5 ? OpCodes.Sub_Ovf_Un : OpCodes.Sub_Ovf);
                        }
                        else
                        {
                            ilg.Emit(OpCodes.Sub);
                        }
                        break;
                    }
                case BinaryArithmeticOperation.Multiply:
                    this.EmitMultiply(ilg, emitOverflow, unsigned);
                    break;
                case BinaryArithmeticOperation.Divide:
                    {
                        var flag6 = unsigned;
                        ilg.Emit(flag6 ? OpCodes.Div_Un : OpCodes.Div);
                        break;
                    }
                case BinaryArithmeticOperation.Mod:
                    {
                        var flag7 = unsigned;
                        ilg.Emit(flag7 ? OpCodes.Rem_Un : OpCodes.Rem);
                        break;
                    }
                case BinaryArithmeticOperation.Power:
                    this.EmitPower(ilg, emitOverflow, unsigned);
                    break;
                default:
                    Debug.Fail("Unknown op type");
                    break;
            }
        }

        private void EmitPower(FleeIlGenerator ilg, bool emitOverflow, bool unsigned)
        {
            var isOptimizablePower = this.IsOptimizablePower;
            if (isOptimizablePower)
            {
                this.EmitOptimizedPower(ilg, emitOverflow, unsigned);
            }
            else
            {
                ilg.Emit(OpCodes.Call, ourPowerMethodInfo);
            }
        }

        private void EmitOptimizedPower(FleeIlGenerator ilg, bool emitOverflow, bool unsigned)
        {
            var right = (Int32LiteralElement)this.myRightChild;
            var flag = right.Value == 0;
            if (flag)
            {
                ilg.Emit(OpCodes.Pop);
                LiteralElement.EmitLoad(1, ilg);
                ImplicitConverter.EmitImplicitNumericConvert(typeof(int), this.myLeftChild.ResultType, ilg);
            }
            else
            {
                var flag2 = right.Value == 1;
                if (!flag2)
                {
                    var num = right.Value - 1;
                    for (var i = 1; i <= num; i++)
                    {
                        ilg.Emit(OpCodes.Dup);
                    }
                    var num2 = right.Value - 1;
                    for (var j = 1; j <= num2; j++)
                    {
                        this.EmitMultiply(ilg, emitOverflow, unsigned);
                    }
                }
            }
        }

        private void EmitMultiply(FleeIlGenerator ilg, bool emitOverflow, bool unsigned)
        {
            if (emitOverflow)
            {
                ilg.Emit(unsigned ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf);
            }
            else
            {
                ilg.Emit(OpCodes.Mul);
            }
        }

        private void EmitStringConcat(FleeIlGenerator ilg, IServiceProvider services)
        {
            var flag = this.AreBothChildrenOfType(typeof(string));
            MethodInfo concatMethodInfo;
            Type argType;
            if (flag)
            {
                concatMethodInfo = ourStringConcatMethodInfo;
                argType = typeof(string);
            }
            else
            {
                Debug.Assert(this.IsEitherChildOfType(typeof(string)), "one child must be a string");
                concatMethodInfo = ourObjectConcatMethodInfo;
                argType = typeof(object);
            }
            this.myLeftChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myLeftChild.ResultType, argType, ilg);
            this.myRightChild.Emit(ilg, services);
            ImplicitConverter.EmitImplicitConvert(this.myRightChild.ResultType, argType, ilg);
            ilg.Emit(OpCodes.Call, concatMethodInfo);
        }
    }
}