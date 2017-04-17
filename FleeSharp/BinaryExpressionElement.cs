// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace Flee
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    internal abstract class BinaryExpressionElement : ExpressionElement
    {
        protected ExpressionElement myLeftChild;

        private Type myResultType;

        protected ExpressionElement myRightChild;

        public static BinaryExpressionElement CreateElement(IList childValues, Type elementType)
        {
            var firstElement = (BinaryExpressionElement) Activator.CreateInstance(elementType);
            firstElement.Configure((ExpressionElement) childValues[0], (ExpressionElement) childValues[2],
                RuntimeHelpers.GetObjectValue(childValues[1]));
            var lastElement = firstElement;
            var num = childValues.Count - 1;
            for (var i = 3; i <= num; i += 2)
            {
                var element = (BinaryExpressionElement) Activator.CreateInstance(elementType);
                element.Configure(lastElement, (ExpressionElement) childValues[i + 1], RuntimeHelpers.GetObjectValue(childValues[i]));
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
                this.ThrowOperandTypeMismatch(RuntimeHelpers.GetObjectValue(op), this.myLeftChild.ResultType,
                    this.myRightChild.ResultType);
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
                getOverloadedBinaryOperator = Utility.GetOverloadedOperator(name, leftType, binder, leftType, rightType);
            }
            else
            {
                var leftMethod = Utility.GetOverloadedOperator(name, leftType, binder, leftType, rightType);
                var rightMethod = Utility.GetOverloadedOperator(name, rightType, binder, leftType, rightType);
                var flag2 = (leftMethod == null) & (rightMethod == null);
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

        protected void EmitOverloadedOperatorCall(MethodInfo method, FleeIlGenerator ilg, IServiceProvider services)
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
            this.ThrowCompileException("OperationNotDefinedForTypes", CompileExceptionReason.TypeMismatch, operation, leftType.Name,
                rightType.Name);
        }

        protected abstract Type GetResultType(Type leftType, Type rightType);

        protected static void EmitChildWithConvert(ExpressionElement child, Type resultType, FleeIlGenerator ilg,
            IServiceProvider services)
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

        public sealed override Type ResultType => this.myResultType;
    }
}