using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Flee
{
    internal abstract class BinaryExpressionElement : ExpressionElement
    {
        protected ExpressionElement myLeftChild;

        protected ExpressionElement myRightChild;

        private Type myResultType;

        public sealed override Type ResultType => this.myResultType;

        public static BinaryExpressionElement CreateElement(IList childValues, Type elementType)
        {
            var firstElement = (BinaryExpressionElement)Activator.CreateInstance(elementType);
            firstElement.Configure((ExpressionElement)childValues[0], (ExpressionElement)childValues[2], RuntimeHelpers.GetObjectValue(childValues[1]));
            var lastElement = firstElement;
            var num = childValues.Count - 1;
            for (var i = 3; i <= num; i += 2)
            {
                var element = (BinaryExpressionElement)Activator.CreateInstance(elementType);
                element.Configure(lastElement, (ExpressionElement)childValues[i + 1], RuntimeHelpers.GetObjectValue(childValues[i]));
                lastElement = element;
            }
            return lastElement;
        }

        protected abstract void GetOperation(object operation);

        protected void ValidateInternal(object op)
        {
            this.myResultType = this.GetResultType(this.myLeftChild.ResultType, this.myRightChild.ResultType);
            var flag = this.myResultType == null;
            if (flag)
            {
                this.ThrowOperandTypeMismatch(RuntimeHelpers.GetObjectValue(op), this.myLeftChild.ResultType, this.myRightChild.ResultType);
            }
        }

        protected MethodInfo GetOverloadedBinaryOperator(string name, object operation)
        {
            var leftType = this.myLeftChild.ResultType;
            var rightType = this.myRightChild.ResultType;
            var binder = new BinaryOperatorBinder(leftType, rightType);
            var flag = leftType == rightType;
            MethodInfo getOverloadedBinaryOperator;
            if (flag)
            {
                getOverloadedBinaryOperator = Utility.GetOverloadedOperator(name, leftType, binder, new Type[]
                {
                    leftType,
                    rightType
                });
            }
            else
            {
                var leftMethod = Utility.GetOverloadedOperator(name, leftType, binder, new Type[]
                {
                    leftType,
                    rightType
                });
                var rightMethod = Utility.GetOverloadedOperator(name, rightType, binder, new Type[]
                {
                    leftType,
                    rightType
                });
                var flag2 = leftMethod == null & rightMethod == null;
                if (flag2)
                {
                    getOverloadedBinaryOperator = null;
                }
                else
                {
                    var flag3 = leftMethod == null;
                    if (flag3)
                    {
                        getOverloadedBinaryOperator = rightMethod;
                    }
                    else
                    {
                        var flag4 = rightMethod == null;
                        if (flag4)
                        {
                            getOverloadedBinaryOperator = leftMethod;
                        }
                        else
                        {
                            this.ThrowAmbiguousCallException(leftType, rightType, RuntimeHelpers.GetObjectValue(operation));
                            getOverloadedBinaryOperator = null;
                        }
                    }
                }
            }
            return getOverloadedBinaryOperator;
        }

        protected void EmitOverloadedOperatorCall(MethodInfo method, FleeILGenerator ilg, IServiceProvider services)
        {
            var @params = method.GetParameters();
            var pLeft = @params[0];
            var pRight = @params[1];
            EmitChildWithConvert(this.myLeftChild, pLeft.ParameterType, ilg, services);
            EmitChildWithConvert(this.myRightChild, pRight.ParameterType, ilg, services);
            ilg.Emit(OpCodes.Call, method);
        }

        protected void ThrowOperandTypeMismatch(object operation, Type leftType, Type rightType)
        {
            this.ThrowCompileException("OperationNotDefinedForTypes", CompileExceptionReason.TypeMismatch, new object[]
            {
                operation,
                leftType.Name,
                rightType.Name
            });
        }

        protected abstract Type GetResultType(Type leftType, Type rightType);

        protected static void EmitChildWithConvert(ExpressionElement child, Type resultType, FleeILGenerator ilg, IServiceProvider services)
        {
            child.Emit(ilg, services);
            var converted = ImplicitConverter.EmitImplicitConvert(child.ResultType, resultType, ilg);
            Debug.Assert(converted, "convert failed");
        }

        protected bool AreBothChildrenOfType(Type target)
        {
            return IsChildOfType(this.myLeftChild, target) & IsChildOfType(this.myRightChild, target);
        }

        protected bool IsEitherChildOfType(Type target)
        {
            return IsChildOfType(this.myLeftChild, target) || IsChildOfType(this.myRightChild, target);
        }

        protected static bool IsChildOfType(ExpressionElement child, Type t)
        {
            return child.ResultType == t;
        }

        private void Configure(ExpressionElement leftChild, ExpressionElement rightChild, object op)
        {
            this.myLeftChild = leftChild;
            this.myRightChild = rightChild;
            this.GetOperation(RuntimeHelpers.GetObjectValue(op));
            this.ValidateInternal(RuntimeHelpers.GetObjectValue(op));
        }
    }
}