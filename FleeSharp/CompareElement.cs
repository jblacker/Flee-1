namespace Flee
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class CompareElement : BinaryExpressionElement
    {
        private LogicalCompareOperation myOperation;

        public void Initialize(ExpressionElement leftChild, ExpressionElement rightChild, LogicalCompareOperation op)
        {
            this.myLeftChild = leftChild;
            this.myRightChild = rightChild;
            this.myOperation = op;
        }

        public void Validate()
        {
            this.ValidateInternal(this.myOperation);
        }

        protected override void GetOperation(object operation)
        {
            this.myOperation = (LogicalCompareOperation) operation;
        }

        protected override Type GetResultType(Type leftType, Type rightType)
        {
            var binaryResultType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
            var overloadedOperator = this.GetOverloadedCompareOperator();
            var isEqualityOp = IsOpTypeEqualOrNotEqual(this.myOperation);
            var flag = (leftType == typeof(string)) & (rightType == typeof(string)) & isEqualityOp;
            Type getResultType;
            if (flag)
            {
                getResultType = typeof(bool);
            }
            else
            {
                var flag2 = overloadedOperator != null;
                if (flag2)
                {
                    getResultType = overloadedOperator.ReturnType;
                }
                else
                {
                    var flag3 = binaryResultType != null;
                    if (flag3)
                    {
                        getResultType = typeof(bool);
                    }
                    else
                    {
                        var flag4 = (leftType == typeof(bool)) & (rightType == typeof(bool)) & isEqualityOp;
                        if (flag4)
                        {
                            getResultType = typeof(bool);
                        }
                        else
                        {
                            var flag5 = this.AreBothChildrenReferenceTypes() & isEqualityOp;
                            if (flag5)
                            {
                                getResultType = typeof(bool);
                            }
                            else
                            {
                                var flag6 = this.AreBothChildrenSameEnum();
                                getResultType = flag6 ? typeof(bool) : null;
                            }
                        }
                    }
                }
            }
            return getResultType;
        }

        private MethodInfo GetOverloadedCompareOperator()
        {
            var name = GetCompareOperatorName(this.myOperation);
            return this.GetOverloadedBinaryOperator(name, this.myOperation);
        }

        private static string GetCompareOperatorName(LogicalCompareOperation op)
        {
            string getCompareOperatorName = null;
            switch (op)
            {
                case LogicalCompareOperation.LessThan:
                    getCompareOperatorName = "LessThan";
                    break;
                case LogicalCompareOperation.GreaterThan:
                    getCompareOperatorName = "GreaterThan";
                    break;
                case LogicalCompareOperation.Equal:
                    getCompareOperatorName = "Equality";
                    break;
                case LogicalCompareOperation.NotEqual:
                    getCompareOperatorName = "Inequality";
                    break;
                case LogicalCompareOperation.LessThanOrEqual:
                    getCompareOperatorName = "LessThanOrEqual";
                    break;
                case LogicalCompareOperation.GreaterThanOrEqual:
                    getCompareOperatorName = "GreaterThanOrEqual";
                    break;
                default:
                    Debug.Assert(false, "unknown compare type");
                    break;
            }
            return getCompareOperatorName;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            var binaryResultType = ImplicitConverter.GetBinaryResultType(this.myLeftChild.ResultType, this.myRightChild.ResultType);
            var overloadedOperator = this.GetOverloadedCompareOperator();
            var flag = this.AreBothChildrenOfType(typeof(string));
            if (flag)
            {
                this.myLeftChild.Emit(ilg, services);
                this.myRightChild.Emit(ilg, services);
                EmitStringEquality(ilg, this.myOperation, services);
            }
            else
            {
                var flag2 = overloadedOperator != null;
                if (flag2)
                {
                    this.EmitOverloadedOperatorCall(overloadedOperator, ilg, services);
                }
                else
                {
                    var flag3 = binaryResultType != null;
                    if (flag3)
                    {
                        EmitChildWithConvert(this.myLeftChild, binaryResultType, ilg, services);
                        EmitChildWithConvert(this.myRightChild, binaryResultType, ilg, services);
                        this.EmitCompareOperation(ilg, this.myOperation);
                    }
                    else
                    {
                        var flag4 = this.AreBothChildrenOfType(typeof(bool));
                        if (flag4)
                        {
                            this.EmitRegular(ilg, services);
                        }
                        else
                        {
                            var flag5 = this.AreBothChildrenReferenceTypes();
                            if (flag5)
                            {
                                this.EmitRegular(ilg, services);
                            }
                            else
                            {
                                var flag6 = this.myLeftChild.ResultType.IsEnum & this.myRightChild.ResultType.IsEnum;
                                if (flag6)
                                {
                                    this.EmitRegular(ilg, services);
                                }
                                else
                                {
                                    Debug.Fail("unknown operand types");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void EmitRegular(FleeILGenerator ilg, IServiceProvider services)
        {
            this.myLeftChild.Emit(ilg, services);
            this.myRightChild.Emit(ilg, services);
            this.EmitCompareOperation(ilg, this.myOperation);
        }

        private static void EmitStringEquality(FleeILGenerator ilg, LogicalCompareOperation op, IServiceProvider services)
        {
            var options = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));
            var ic = new Int32LiteralElement((int) options.StringComparison);
            ic.Emit(ilg, services);
            var mi = typeof(string).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public, null, new[]
            {
                typeof(string),
                typeof(string),
                typeof(StringComparison)
            }, null);
            ilg.Emit(OpCodes.Call, mi);
            var flag = op == LogicalCompareOperation.NotEqual;
            if (flag)
            {
                ilg.Emit(OpCodes.Ldc_I4_0);
                ilg.Emit(OpCodes.Ceq);
            }
        }

        private static bool IsOpTypeEqualOrNotEqual(LogicalCompareOperation op)
        {
            return (op == LogicalCompareOperation.Equal) | (op == LogicalCompareOperation.NotEqual);
        }

        private bool AreBothChildrenReferenceTypes()
        {
            return !this.myLeftChild.ResultType.IsValueType & !this.myRightChild.ResultType.IsValueType;
        }

        private bool AreBothChildrenSameEnum()
        {
            return this.myLeftChild.ResultType.IsEnum && this.myLeftChild.ResultType == this.myRightChild.ResultType;
        }

        private void EmitCompareOperation(FleeILGenerator ilg, LogicalCompareOperation op)
        {
            var ltOpcode = this.GetCompareGtltOpcode(false);
            var gtOpcode = this.GetCompareGtltOpcode(true);
            switch (op)
            {
                case LogicalCompareOperation.LessThan:
                    ilg.Emit(ltOpcode);
                    break;
                case LogicalCompareOperation.GreaterThan:
                    ilg.Emit(gtOpcode);
                    break;
                case LogicalCompareOperation.Equal:
                    ilg.Emit(OpCodes.Ceq);
                    break;
                case LogicalCompareOperation.NotEqual:
                    ilg.Emit(OpCodes.Ceq);
                    ilg.Emit(OpCodes.Ldc_I4_0);
                    ilg.Emit(OpCodes.Ceq);
                    break;
                case LogicalCompareOperation.LessThanOrEqual:
                    ilg.Emit(gtOpcode);
                    ilg.Emit(OpCodes.Ldc_I4_0);
                    ilg.Emit(OpCodes.Ceq);
                    break;
                case LogicalCompareOperation.GreaterThanOrEqual:
                    ilg.Emit(ltOpcode);
                    ilg.Emit(OpCodes.Ldc_I4_0);
                    ilg.Emit(OpCodes.Ceq);
                    break;
                default:
                    Debug.Fail("Unknown op type");
                    break;
            }
        }

        private OpCode GetCompareGtltOpcode(bool greaterThan)
        {
            var leftType = this.myLeftChild.ResultType;
            var flag = leftType == this.myRightChild.ResultType;
            OpCode getCompareGtltOpcode;
            if (flag)
            {
                var flag2 = (leftType == typeof(uint)) | (leftType == typeof(ulong));
                if (flag2)
                {
                    getCompareGtltOpcode = greaterThan ? OpCodes.Cgt_Un : OpCodes.Clt_Un;
                }
                else
                {
                    getCompareGtltOpcode = GetCompareOpcode(greaterThan);
                }
            }
            else
            {
                getCompareGtltOpcode = GetCompareOpcode(greaterThan);
            }
            return getCompareGtltOpcode;
        }

        private static OpCode GetCompareOpcode(bool greaterThan)
        {
            var getCompareOpcode = greaterThan ? OpCodes.Cgt : OpCodes.Clt;
            return getCompareOpcode;
        }
    }
}