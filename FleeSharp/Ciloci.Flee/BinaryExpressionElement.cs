using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	internal abstract class BinaryExpressionElement : ExpressionElement
	{
		protected ExpressionElement MyLeftChild;

		protected ExpressionElement MyRightChild;

		private Type MyResultType;

		public sealed override Type ResultType
		{
			get
			{
				return this.MyResultType;
			}
		}

		public static BinaryExpressionElement CreateElement(IList childValues, Type elementType)
		{
			BinaryExpressionElement firstElement = (BinaryExpressionElement)Activator.CreateInstance(elementType);
			firstElement.Configure((ExpressionElement)childValues[0], (ExpressionElement)childValues[2], RuntimeHelpers.GetObjectValue(childValues[1]));
			BinaryExpressionElement lastElement = firstElement;
			int num = childValues.Count - 1;
			for (int i = 3; i <= num; i += 2)
			{
				BinaryExpressionElement element = (BinaryExpressionElement)Activator.CreateInstance(elementType);
				element.Configure(lastElement, (ExpressionElement)childValues[i + 1], RuntimeHelpers.GetObjectValue(childValues[i]));
				lastElement = element;
			}
			return lastElement;
		}

		protected abstract void GetOperation(object operation);

		protected void ValidateInternal(object op)
		{
			this.MyResultType = this.GetResultType(this.MyLeftChild.ResultType, this.MyRightChild.ResultType);
			bool flag = this.MyResultType == null;
			if (flag)
			{
				this.ThrowOperandTypeMismatch(RuntimeHelpers.GetObjectValue(op), this.MyLeftChild.ResultType, this.MyRightChild.ResultType);
			}
		}

		protected MethodInfo GetOverloadedBinaryOperator(string name, object operation)
		{
			Type leftType = this.MyLeftChild.ResultType;
			Type rightType = this.MyRightChild.ResultType;
			BinaryOperatorBinder binder = new BinaryOperatorBinder(leftType, rightType);
			bool flag = leftType == rightType;
			MethodInfo GetOverloadedBinaryOperator;
			if (flag)
			{
				GetOverloadedBinaryOperator = Utility.GetOverloadedOperator(name, leftType, binder, new Type[]
				{
					leftType,
					rightType
				});
			}
			else
			{
				MethodInfo leftMethod = Utility.GetOverloadedOperator(name, leftType, binder, new Type[]
				{
					leftType,
					rightType
				});
				MethodInfo rightMethod = Utility.GetOverloadedOperator(name, rightType, binder, new Type[]
				{
					leftType,
					rightType
				});
				bool flag2 = leftMethod == null & rightMethod == null;
				if (flag2)
				{
					GetOverloadedBinaryOperator = null;
				}
				else
				{
					bool flag3 = leftMethod == null;
					if (flag3)
					{
						GetOverloadedBinaryOperator = rightMethod;
					}
					else
					{
						bool flag4 = rightMethod == null;
						if (flag4)
						{
							GetOverloadedBinaryOperator = leftMethod;
						}
						else
						{
							base.ThrowAmbiguousCallException(leftType, rightType, RuntimeHelpers.GetObjectValue(operation));
							GetOverloadedBinaryOperator = null;
						}
					}
				}
			}
			return GetOverloadedBinaryOperator;
		}

		protected void EmitOverloadedOperatorCall(MethodInfo method, FleeILGenerator ilg, IServiceProvider services)
		{
			ParameterInfo[] @params = method.GetParameters();
			ParameterInfo pLeft = @params[0];
			ParameterInfo pRight = @params[1];
			BinaryExpressionElement.EmitChildWithConvert(this.MyLeftChild, pLeft.ParameterType, ilg, services);
			BinaryExpressionElement.EmitChildWithConvert(this.MyRightChild, pRight.ParameterType, ilg, services);
			ilg.Emit(OpCodes.Call, method);
		}

		protected void ThrowOperandTypeMismatch(object operation, Type leftType, Type rightType)
		{
			base.ThrowCompileException("OperationNotDefinedForTypes", CompileExceptionReason.TypeMismatch, new object[]
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
			bool converted = ImplicitConverter.EmitImplicitConvert(child.ResultType, resultType, ilg);
			Debug.Assert(converted, "convert failed");
		}

		protected bool AreBothChildrenOfType(Type target)
		{
			return BinaryExpressionElement.IsChildOfType(this.MyLeftChild, target) & BinaryExpressionElement.IsChildOfType(this.MyRightChild, target);
		}

		protected bool IsEitherChildOfType(Type target)
		{
			return BinaryExpressionElement.IsChildOfType(this.MyLeftChild, target) || BinaryExpressionElement.IsChildOfType(this.MyRightChild, target);
		}

		protected static bool IsChildOfType(ExpressionElement child, Type t)
		{
			return child.ResultType == t;
		}

		private void Configure(ExpressionElement leftChild, ExpressionElement rightChild, object op)
		{
			this.MyLeftChild = leftChild;
			this.MyRightChild = rightChild;
			this.GetOperation(RuntimeHelpers.GetObjectValue(op));
			this.ValidateInternal(RuntimeHelpers.GetObjectValue(op));
		}
	}
}
