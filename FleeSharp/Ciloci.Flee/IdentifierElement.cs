using Ciloci.Flee.CalcEngine;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	internal class IdentifierElement : MemberElement
	{
		private FieldInfo MyField;

		private PropertyInfo MyProperty;

		private PropertyDescriptor MyPropertyDescriptor;

		private Type MyVariableType;

		private Type MyCalcEngineReferenceType;

		private Type MemberOwnerType
		{
			get
			{
				bool flag = this.MyField != null;
				Type MemberOwnerType;
				if (flag)
				{
					MemberOwnerType = this.MyField.ReflectedType;
				}
				else
				{
					bool flag2 = this.MyPropertyDescriptor != null;
					if (flag2)
					{
						MemberOwnerType = this.MyPropertyDescriptor.ComponentType;
					}
					else
					{
						bool flag3 = this.MyProperty != null;
						if (flag3)
						{
							MemberOwnerType = this.MyProperty.ReflectedType;
						}
						else
						{
							MemberOwnerType = null;
						}
					}
				}
				return MemberOwnerType;
			}
		}

		public override Type ResultType
		{
			get
			{
				bool flag = this.MyCalcEngineReferenceType != null;
				Type ResultType;
				if (flag)
				{
					ResultType = this.MyCalcEngineReferenceType;
				}
				else
				{
					bool flag2 = this.MyVariableType != null;
					if (flag2)
					{
						ResultType = this.MyVariableType;
					}
					else
					{
						bool flag3 = this.MyPropertyDescriptor != null;
						if (flag3)
						{
							ResultType = this.MyPropertyDescriptor.PropertyType;
						}
						else
						{
							bool flag4 = this.MyField != null;
							if (flag4)
							{
								ResultType = this.MyField.FieldType;
							}
							else
							{
								MethodInfo mi = this.MyProperty.GetGetMethod(true);
								ResultType = mi.ReturnType;
							}
						}
					}
				}
				return ResultType;
			}
		}

		protected override bool RequiresAddress
		{
			get
			{
				return this.MyPropertyDescriptor == null;
			}
		}

		protected override bool IsPublic
		{
			get
			{
				bool flag = this.MyVariableType != null | this.MyCalcEngineReferenceType != null;
				bool IsPublic;
				if (flag)
				{
					IsPublic = true;
				}
				else
				{
					bool flag2 = this.MyVariableType != null;
					if (flag2)
					{
						IsPublic = true;
					}
					else
					{
						bool flag3 = this.MyPropertyDescriptor != null;
						if (flag3)
						{
							IsPublic = true;
						}
						else
						{
							bool flag4 = this.MyField != null;
							if (flag4)
							{
								IsPublic = this.MyField.IsPublic;
							}
							else
							{
								MethodInfo mi = this.MyProperty.GetGetMethod(true);
								IsPublic = mi.IsPublic;
							}
						}
					}
				}
				return IsPublic;
			}
		}

		protected override bool SupportsStatic
		{
			get
			{
				bool flag = this.MyVariableType != null;
				bool SupportsStatic;
				if (flag)
				{
					SupportsStatic = false;
				}
				else
				{
					bool flag2 = this.MyPropertyDescriptor != null;
					if (flag2)
					{
						SupportsStatic = false;
					}
					else
					{
						bool flag3 = this.MyOptions.IsOwnerType(this.MemberOwnerType) && this.MyPrevious == null;
						SupportsStatic = (flag3 || this.MyPrevious == null);
					}
				}
				return SupportsStatic;
			}
		}

		protected override bool SupportsInstance
		{
			get
			{
				bool flag = this.MyVariableType != null;
				bool SupportsInstance;
				if (flag)
				{
					SupportsInstance = true;
				}
				else
				{
					bool flag2 = this.MyPropertyDescriptor != null;
					if (flag2)
					{
						SupportsInstance = true;
					}
					else
					{
						bool flag3 = this.MyOptions.IsOwnerType(this.MemberOwnerType) && this.MyPrevious == null;
						SupportsInstance = (flag3 || this.MyPrevious != null);
					}
				}
				return SupportsInstance;
			}
		}

		public override bool IsStatic
		{
			get
			{
				bool flag = this.MyVariableType != null | this.MyCalcEngineReferenceType != null;
				bool IsStatic;
				if (flag)
				{
					IsStatic = false;
				}
				else
				{
					bool flag2 = this.MyVariableType != null;
					if (flag2)
					{
						IsStatic = false;
					}
					else
					{
						bool flag3 = this.MyField != null;
						if (flag3)
						{
							IsStatic = this.MyField.IsStatic;
						}
						else
						{
							bool flag4 = this.MyPropertyDescriptor != null;
							if (flag4)
							{
								IsStatic = false;
							}
							else
							{
								MethodInfo mi = this.MyProperty.GetGetMethod(true);
								IsStatic = mi.IsStatic;
							}
						}
					}
				}
				return IsStatic;
			}
		}

		public IdentifierElement(string name)
		{
			this.MyName = name;
		}

		protected override void ResolveInternal()
		{
			bool flag = this.ResolveFieldProperty(this.MyPrevious);
			if (flag)
			{
				this.AddReferencedVariable(this.MyPrevious);
			}
			else
			{
				this.MyVariableType = this.MyContext.Variables.GetVariableTypeInternal(this.MyName);
				bool flag2 = this.MyPrevious == null && this.MyVariableType != null;
				if (flag2)
				{
					this.AddReferencedVariable(this.MyPrevious);
				}
				else
				{
					CalculationEngine ce = this.MyContext.CalculationEngine;
					bool flag3 = ce != null;
					if (flag3)
					{
						ce.AddDependency(this.MyName, this.MyContext);
						this.MyCalcEngineReferenceType = ce.ResolveTailType(this.MyName);
					}
					else
					{
						bool flag4 = this.MyPrevious == null;
						if (flag4)
						{
							base.ThrowCompileException("NoIdentifierWithName", CompileExceptionReason.UndefinedName, new object[]
							{
								this.MyName
							});
						}
						else
						{
							base.ThrowCompileException("NoIdentifierWithNameOnType", CompileExceptionReason.UndefinedName, new object[]
							{
								this.MyName,
								this.MyPrevious.TargetType.Name
							});
						}
					}
				}
			}
		}

		private bool ResolveFieldProperty(MemberElement previous)
		{
			MemberInfo[] members = base.GetMembers(MemberTypes.Field | MemberTypes.Property);
			members = base.GetAccessibleMembers(members);
			bool flag = members.Length == 0;
			bool ResolveFieldProperty;
			if (flag)
			{
				ResolveFieldProperty = this.ResolveVirtualProperty(previous);
			}
			else
			{
				bool flag2 = members.Length > 1;
				if (flag2)
				{
					bool flag3 = previous == null;
					if (flag3)
					{
						base.ThrowCompileException("IdentifierIsAmbiguous", CompileExceptionReason.AmbiguousMatch, new object[]
						{
							this.MyName
						});
					}
					else
					{
						base.ThrowCompileException("IdentifierIsAmbiguousOnType", CompileExceptionReason.AmbiguousMatch, new object[]
						{
							this.MyName,
							previous.TargetType.Name
						});
					}
				}
				else
				{
					this.MyField = (members[0] as FieldInfo);
					bool flag4 = this.MyField != null;
					if (flag4)
					{
						ResolveFieldProperty = true;
					}
					else
					{
						this.MyProperty = (PropertyInfo)members[0];
						ResolveFieldProperty = true;
					}
				}
			}
			return ResolveFieldProperty;
		}

		private bool ResolveVirtualProperty(MemberElement previous)
		{
			bool flag = previous == null;
			bool ResolveVirtualProperty;
			if (flag)
			{
				ResolveVirtualProperty = false;
			}
			else
			{
				PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(previous.ResultType);
				this.MyPropertyDescriptor = coll.Find(this.MyName, true);
				ResolveVirtualProperty = (this.MyPropertyDescriptor != null);
			}
			return ResolveVirtualProperty;
		}

		private void AddReferencedVariable(MemberElement previous)
		{
			bool flag = previous != null;
			if (!flag)
			{
				bool flag2 = this.MyVariableType != null || this.MyOptions.IsOwnerType(this.MemberOwnerType);
				if (flag2)
				{
					ExpressionInfo info = (ExpressionInfo)this.MyServices.GetService(typeof(ExpressionInfo));
					info.AddReferencedVariable(this.MyName);
				}
			}
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			base.Emit(ilg, services);
			this.EmitFirst(ilg);
			bool flag = this.MyCalcEngineReferenceType != null;
			if (flag)
			{
				this.EmitReferenceLoad(ilg);
			}
			else
			{
				bool flag2 = this.MyVariableType != null;
				if (flag2)
				{
					this.EmitVariableLoad(ilg);
				}
				else
				{
					bool flag3 = this.MyField != null;
					if (flag3)
					{
						this.EmitFieldLoad(this.MyField, ilg, services);
					}
					else
					{
						bool flag4 = this.MyPropertyDescriptor != null;
						if (flag4)
						{
							this.EmitVirtualPropertyLoad(ilg);
						}
						else
						{
							this.EmitPropertyLoad(this.MyProperty, ilg);
						}
					}
				}
			}
		}

		private void EmitReferenceLoad(FleeILGenerator ilg)
		{
			ilg.Emit(OpCodes.Ldarg_1);
			this.MyContext.CalculationEngine.EmitLoad(this.MyName, ilg);
		}

		private void EmitFirst(FleeILGenerator ilg)
		{
			bool flag = this.MyPrevious != null;
			if (!flag)
			{
				bool isVariable = this.MyVariableType != null;
				bool flag2 = isVariable;
				if (flag2)
				{
					MemberElement.EmitLoadVariables(ilg);
				}
				else
				{
					bool flag3 = this.MyOptions.IsOwnerType(this.MemberOwnerType) & !this.IsStatic;
					if (flag3)
					{
						base.EmitLoadOwner(ilg);
					}
				}
			}
		}

		private void EmitVariableLoad(FleeILGenerator ilg)
		{
			MethodInfo mi = VariableCollection.GetVariableLoadMethod(this.MyVariableType);
			ilg.Emit(OpCodes.Ldstr, this.MyName);
			base.EmitMethodCall(mi, ilg);
		}

		private void EmitFieldLoad(FieldInfo fi, FleeILGenerator ilg, IServiceProvider services)
		{
			bool isLiteral = fi.IsLiteral;
			if (isLiteral)
			{
				IdentifierElement.EmitLiteral(fi, ilg, services);
			}
			else
			{
				bool flag = this.ResultType.IsValueType & base.NextRequiresAddress;
				if (flag)
				{
					IdentifierElement.EmitLdfld(fi, true, ilg);
				}
				else
				{
					IdentifierElement.EmitLdfld(fi, false, ilg);
				}
			}
		}

		private static void EmitLdfld(FieldInfo fi, bool indirect, FleeILGenerator ilg)
		{
			bool isStatic = fi.IsStatic;
			if (isStatic)
			{
				if (indirect)
				{
					ilg.Emit(OpCodes.Ldsflda, fi);
				}
				else
				{
					ilg.Emit(OpCodes.Ldsfld, fi);
				}
			}
			else if (indirect)
			{
				ilg.Emit(OpCodes.Ldflda, fi);
			}
			else
			{
				ilg.Emit(OpCodes.Ldfld, fi);
			}
		}

		private static void EmitLiteral(FieldInfo fi, FleeILGenerator ilg, IServiceProvider services)
		{
			object value = RuntimeHelpers.GetObjectValue(fi.GetValue(null));
			Type t = value.GetType();
			LiteralElement elem;
			switch (Type.GetTypeCode(t))
			{
			case TypeCode.Boolean:
				elem = new BooleanLiteralElement((bool)value);
				goto IL_F4;
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
				elem = new Int32LiteralElement(Convert.ToInt32(RuntimeHelpers.GetObjectValue(value)));
				goto IL_F4;
			case TypeCode.UInt32:
				elem = new UInt32LiteralElement((uint)value);
				goto IL_F4;
			case TypeCode.Int64:
				elem = new Int64LiteralElement((long)value);
				goto IL_F4;
			case TypeCode.UInt64:
				elem = new UInt64LiteralElement((ulong)value);
				goto IL_F4;
			case TypeCode.Single:
				elem = new SingleLiteralElement((float)value);
				goto IL_F4;
			case TypeCode.Double:
				elem = new DoubleLiteralElement((double)value);
				goto IL_F4;
			case TypeCode.String:
				elem = new StringLiteralElement((string)value);
				goto IL_F4;
			}
			elem = null;
			Debug.Fail("Unsupported constant type");
			IL_F4:
			elem.Emit(ilg, services);
		}

		private void EmitPropertyLoad(PropertyInfo pi, FleeILGenerator ilg)
		{
			MethodInfo getter = pi.GetGetMethod(true);
			base.EmitMethodCall(getter, ilg);
		}

		private void EmitVirtualPropertyLoad(FleeILGenerator ilg)
		{
			int index = ilg.GetTempLocalIndex(this.MyPrevious.ResultType);
			Utility.EmitStoreLocal(ilg, index);
			MemberElement.EmitLoadVariables(ilg);
			ilg.Emit(OpCodes.Ldstr, this.MyName);
			Utility.EmitLoadLocal(ilg, index);
			ImplicitConverter.EmitImplicitConvert(this.MyPrevious.ResultType, typeof(object), ilg);
			MethodInfo mi = VariableCollection.GetVirtualPropertyLoadMethod(this.ResultType);
			base.EmitMethodCall(mi, ilg);
		}
	}
}
