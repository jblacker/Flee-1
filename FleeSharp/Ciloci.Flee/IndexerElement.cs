using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class IndexerElement : MemberElement
	{
		private ExpressionElement MyIndexerElement;

		private ArgumentList MyIndexerElements;

		private Type ArrayType
		{
			get
			{
				bool isArray = this.IsArray;
				Type ArrayType;
				if (isArray)
				{
					ArrayType = this.MyPrevious.TargetType;
				}
				else
				{
					ArrayType = null;
				}
				return ArrayType;
			}
		}

		private bool IsArray
		{
			get
			{
				return this.MyPrevious.TargetType.IsArray;
			}
		}

		protected override bool RequiresAddress
		{
			get
			{
				return !this.IsArray;
			}
		}

		public override Type ResultType
		{
			get
			{
				bool isArray = this.IsArray;
				Type ResultType;
				if (isArray)
				{
					ResultType = this.ArrayType.GetElementType();
				}
				else
				{
					ResultType = this.MyIndexerElement.ResultType;
				}
				return ResultType;
			}
		}

		protected override bool IsPublic
		{
			get
			{
				bool isArray = this.IsArray;
				return isArray || MemberElement.IsElementPublic((MemberElement)this.MyIndexerElement);
			}
		}

		public override bool IsStatic
		{
			get
			{
				return false;
			}
		}

		public IndexerElement(ArgumentList indexer)
		{
			this.MyIndexerElements = indexer;
		}

		protected override void ResolveInternal()
		{
			Type target = this.MyPrevious.TargetType;
			bool isArray = target.IsArray;
			if (isArray)
			{
				this.SetupArrayIndexer();
			}
			else
			{
				bool flag = !this.FindIndexer(target);
				if (flag)
				{
					base.ThrowCompileException("TypeNotArrayAndHasNoIndexerOfType", CompileExceptionReason.TypeMismatch, new object[]
					{
						target.Name,
						this.MyIndexerElements
					});
				}
			}
		}

		private void SetupArrayIndexer()
		{
			this.MyIndexerElement = this.MyIndexerElements[0];
			bool flag = this.MyIndexerElements.Count > 1;
			if (flag)
			{
				base.ThrowCompileException("MultiArrayIndexNotSupported", CompileExceptionReason.TypeMismatch, new object[0]);
			}
			else
			{
				bool flag2 = !ImplicitConverter.EmitImplicitConvert(this.MyIndexerElement.ResultType, typeof(int), null);
				if (flag2)
				{
					base.ThrowCompileException("ArrayIndexersMustBeOfType", CompileExceptionReason.TypeMismatch, new object[]
					{
						typeof(int).Name
					});
				}
			}
		}

		private bool FindIndexer(Type targetType)
		{
			MemberInfo[] members = targetType.GetDefaultMembers();
			List<MethodInfo> methods = new List<MethodInfo>();
			MemberInfo[] array = members;
			checked
			{
				for (int i = 0; i < array.Length; i++)
				{
					MemberInfo mi = array[i];
					PropertyInfo pi = mi as PropertyInfo;
					bool flag = pi != null;
					if (flag)
					{
						methods.Add(pi.GetGetMethod(true));
					}
				}
				FunctionCallElement func = new FunctionCallElement("Indexer", methods.ToArray(), this.MyIndexerElements);
				func.Resolve(this.MyServices);
				this.MyIndexerElement = func;
				return true;
			}
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			base.Emit(ilg, services);
			bool isArray = this.IsArray;
			if (isArray)
			{
				this.EmitArrayLoad(ilg, services);
			}
			else
			{
				this.EmitIndexer(ilg, services);
			}
		}

		private void EmitArrayLoad(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyIndexerElement.Emit(ilg, services);
			ImplicitConverter.EmitImplicitConvert(this.MyIndexerElement.ResultType, typeof(int), ilg);
			Type elementType = this.ResultType;
			bool flag = !elementType.IsValueType;
			if (flag)
			{
				ilg.Emit(OpCodes.Ldelem_Ref);
			}
			else
			{
				this.EmitValueTypeArrayLoad(ilg, elementType);
			}
		}

		private void EmitValueTypeArrayLoad(FleeILGenerator ilg, Type elementType)
		{
			bool nextRequiresAddress = base.NextRequiresAddress;
			if (nextRequiresAddress)
			{
				ilg.Emit(OpCodes.Ldelema, elementType);
			}
			else
			{
				Utility.EmitArrayLoad(ilg, elementType);
			}
		}

		private void EmitIndexer(FleeILGenerator ilg, IServiceProvider services)
		{
			FunctionCallElement func = (FunctionCallElement)this.MyIndexerElement;
			func.EmitFunctionCall(base.NextRequiresAddress, ilg, services);
		}
	}
}
