using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class CastElement : ExpressionElement
	{
		private ExpressionElement MyCastExpression;

		private Type MyDestType;

		public override Type ResultType
		{
			get
			{
				return this.MyDestType;
			}
		}

		public CastElement(ExpressionElement castExpression, string[] destTypeParts, bool isArray, IServiceProvider services)
		{
			this.MyCastExpression = castExpression;
			this.MyDestType = CastElement.GetDestType(destTypeParts, services);
			bool flag = this.MyDestType == null;
			if (flag)
			{
				base.ThrowCompileException("CouldNotResolveType", CompileExceptionReason.UndefinedName, new object[]
				{
					CastElement.GetDestTypeString(destTypeParts, isArray)
				});
			}
			if (isArray)
			{
				this.MyDestType = this.MyDestType.MakeArrayType();
			}
			bool flag2 = !this.IsValidCast(this.MyCastExpression.ResultType, this.MyDestType);
			if (flag2)
			{
				this.ThrowInvalidCastException();
			}
		}

		private static string GetDestTypeString(string[] parts, bool isArray)
		{
			string s = string.Join(".", parts);
			if (isArray)
			{
				s += "[]";
			}
			return s;
		}

		private static Type GetDestType(string[] destTypeParts, IServiceProvider services)
		{
			ExpressionContext context = (ExpressionContext)services.GetService(typeof(ExpressionContext));
			Type t = null;
			bool flag = destTypeParts.Length == 1;
			if (flag)
			{
				t = ExpressionImports.GetBuiltinType(destTypeParts[0]);
			}
			bool flag2 = t != null;
			Type GetDestType;
			if (flag2)
			{
				GetDestType = t;
			}
			else
			{
				t = context.Imports.FindType(destTypeParts);
				bool flag3 = t != null;
				if (flag3)
				{
					GetDestType = t;
				}
				else
				{
					GetDestType = null;
				}
			}
			return GetDestType;
		}

		private bool IsValidCast(Type sourceType, Type destType)
		{
			bool flag = sourceType == destType;
			bool IsValidCast;
			if (flag)
			{
				IsValidCast = true;
			}
			else
			{
				bool flag2 = destType.IsAssignableFrom(sourceType);
				if (flag2)
				{
					IsValidCast = true;
				}
				else
				{
					bool flag3 = ImplicitConverter.EmitImplicitConvert(sourceType, destType, null);
					if (flag3)
					{
						IsValidCast = true;
					}
					else
					{
						bool flag4 = CastElement.IsCastableNumericType(sourceType) & CastElement.IsCastableNumericType(destType);
						if (flag4)
						{
							IsValidCast = true;
						}
						else
						{
							bool flag5 = sourceType.IsEnum | destType.IsEnum;
							if (flag5)
							{
								IsValidCast = this.IsValidExplicitEnumCast(sourceType, destType);
							}
							else
							{
								bool flag6 = this.GetExplictOverloadedOperator(sourceType, destType) != null;
								if (flag6)
								{
									IsValidCast = true;
								}
								else
								{
									bool isValueType = sourceType.IsValueType;
									if (isValueType)
									{
										IsValidCast = false;
									}
									else
									{
										bool isValueType2 = destType.IsValueType;
										if (isValueType2)
										{
											Type[] interfaces = destType.GetInterfaces();
											IsValidCast = (CastElement.IsBaseType(destType, sourceType) | Array.IndexOf<Type>(interfaces, sourceType) != -1);
										}
										else
										{
											IsValidCast = this.IsValidExplicitReferenceCast(sourceType, destType);
										}
									}
								}
							}
						}
					}
				}
			}
			return IsValidCast;
		}

		private MethodInfo GetExplictOverloadedOperator(Type sourceType, Type destType)
		{
			ExplicitOperatorMethodBinder binder = new ExplicitOperatorMethodBinder(destType, sourceType);
			MethodInfo miSource = Utility.GetOverloadedOperator("Explicit", sourceType, binder, new Type[]
			{
				sourceType
			});
			MethodInfo miDest = Utility.GetOverloadedOperator("Explicit", destType, binder, new Type[]
			{
				sourceType
			});
			bool flag = miSource == null & miDest == null;
			MethodInfo GetExplictOverloadedOperator;
			if (flag)
			{
				GetExplictOverloadedOperator = null;
			}
			else
			{
				bool flag2 = miSource == null;
				if (flag2)
				{
					GetExplictOverloadedOperator = miDest;
				}
				else
				{
					bool flag3 = miDest == null;
					if (flag3)
					{
						GetExplictOverloadedOperator = miSource;
					}
					else
					{
						base.ThrowAmbiguousCallException(sourceType, destType, "Explicit");
						GetExplictOverloadedOperator = null;
					}
				}
			}
			return GetExplictOverloadedOperator;
		}

		private bool IsValidExplicitEnumCast(Type sourceType, Type destType)
		{
			sourceType = CastElement.GetUnderlyingEnumType(sourceType);
			destType = CastElement.GetUnderlyingEnumType(destType);
			return this.IsValidCast(sourceType, destType);
		}

		private bool IsValidExplicitReferenceCast(Type sourceType, Type destType)
		{
			Debug.Assert(!sourceType.IsValueType & !destType.IsValueType, "expecting reference types");
			bool flag = sourceType == typeof(object);
			bool IsValidExplicitReferenceCast;
			if (flag)
			{
				IsValidExplicitReferenceCast = true;
			}
			else
			{
				bool flag2 = sourceType.IsArray & destType.IsArray;
				if (flag2)
				{
					bool flag3 = sourceType.GetArrayRank() != destType.GetArrayRank();
					if (flag3)
					{
						IsValidExplicitReferenceCast = false;
					}
					else
					{
						Type SE = sourceType.GetElementType();
						Type TE = destType.GetElementType();
						bool flag4 = SE.IsValueType | TE.IsValueType;
						IsValidExplicitReferenceCast = (!flag4 && this.IsValidExplicitReferenceCast(SE, TE));
					}
				}
				else
				{
					bool flag5 = sourceType.IsClass & destType.IsClass;
					if (flag5)
					{
						IsValidExplicitReferenceCast = CastElement.IsBaseType(destType, sourceType);
					}
					else
					{
						bool flag6 = sourceType.IsClass & destType.IsInterface;
						if (flag6)
						{
							IsValidExplicitReferenceCast = (!sourceType.IsSealed & !CastElement.ImplementsInterface(sourceType, destType));
						}
						else
						{
							bool flag7 = sourceType.IsInterface & destType.IsClass;
							if (flag7)
							{
								IsValidExplicitReferenceCast = (!destType.IsSealed | CastElement.ImplementsInterface(destType, sourceType));
							}
							else
							{
								bool flag8 = sourceType.IsInterface & destType.IsInterface;
								if (flag8)
								{
									IsValidExplicitReferenceCast = !CastElement.ImplementsInterface(sourceType, destType);
								}
								else
								{
									Debug.Assert(false, "unknown explicit cast");
								}
							}
						}
					}
				}
			}
			return IsValidExplicitReferenceCast;
		}

		private static bool IsBaseType(Type target, Type potentialBase)
		{
			bool IsBaseType;
			for (Type current = target; current != null; current = current.BaseType)
			{
				bool flag = current == potentialBase;
				if (flag)
				{
					IsBaseType = true;
					return IsBaseType;
				}
			}
			IsBaseType = false;
			return IsBaseType;
		}

		private static bool ImplementsInterface(Type target, Type interfaceType)
		{
			Type[] interfaces = target.GetInterfaces();
			return Array.IndexOf<Type>(interfaces, interfaceType) != -1;
		}

		private void ThrowInvalidCastException()
		{
			base.ThrowCompileException("CannotConvertType", CompileExceptionReason.InvalidExplicitCast, new object[]
			{
				this.MyCastExpression.ResultType.Name,
				this.MyDestType.Name
			});
		}

		private static bool IsCastableNumericType(Type t)
		{
			return t.IsPrimitive & t != typeof(bool);
		}

		private static Type GetUnderlyingEnumType(Type t)
		{
			bool isEnum = t.IsEnum;
			Type GetUnderlyingEnumType;
			if (isEnum)
			{
				GetUnderlyingEnumType = Enum.GetUnderlyingType(t);
			}
			else
			{
				GetUnderlyingEnumType = t;
			}
			return GetUnderlyingEnumType;
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyCastExpression.Emit(ilg, services);
			Type sourceType = this.MyCastExpression.ResultType;
			Type destType = this.MyDestType;
			this.EmitCast(ilg, sourceType, destType, services);
		}

		private void EmitCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
		{
			MethodInfo explicitOperator = this.GetExplictOverloadedOperator(sourceType, destType);
			bool flag = sourceType == destType;
			if (!flag)
			{
				bool flag2 = explicitOperator != null;
				if (flag2)
				{
					ilg.Emit(OpCodes.Call, explicitOperator);
				}
				else
				{
					bool flag3 = sourceType.IsEnum | destType.IsEnum;
					if (flag3)
					{
						this.EmitEnumCast(ilg, sourceType, destType, services);
					}
					else
					{
						bool flag4 = ImplicitConverter.EmitImplicitConvert(sourceType, destType, ilg);
						if (!flag4)
						{
							bool flag5 = CastElement.IsCastableNumericType(sourceType) & CastElement.IsCastableNumericType(destType);
							if (flag5)
							{
								CastElement.EmitExplicitNumericCast(ilg, sourceType, destType, services);
							}
							else
							{
								bool isValueType = sourceType.IsValueType;
								if (isValueType)
								{
									Debug.Assert(!destType.IsValueType, "expecting reference type");
									ilg.Emit(OpCodes.Box, sourceType);
								}
								else
								{
									bool isValueType2 = destType.IsValueType;
									if (isValueType2)
									{
										ilg.Emit(OpCodes.Unbox_Any, destType);
									}
									else
									{
										bool flag6 = !destType.IsAssignableFrom(sourceType);
										if (flag6)
										{
											ilg.Emit(OpCodes.Castclass, destType);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void EmitEnumCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
		{
			bool flag = !destType.IsValueType;
			if (flag)
			{
				ilg.Emit(OpCodes.Box, sourceType);
			}
			else
			{
				bool flag2 = !sourceType.IsValueType;
				if (flag2)
				{
					ilg.Emit(OpCodes.Unbox_Any, destType);
				}
				else
				{
					sourceType = CastElement.GetUnderlyingEnumType(sourceType);
					destType = CastElement.GetUnderlyingEnumType(destType);
					this.EmitCast(ilg, sourceType, destType, services);
				}
			}
		}

		private static void EmitExplicitNumericCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
		{
			TypeCode desttc = Type.GetTypeCode(destType);
			TypeCode sourcetc = Type.GetTypeCode(sourceType);
			bool unsigned = CastElement.IsUnsignedType(sourceType);
			ExpressionOptions options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
			bool @checked = options.Checked;
			OpCode op = OpCodes.Nop;
			switch (desttc)
			{
			case TypeCode.SByte:
			{
				bool flag = unsigned & @checked;
				if (flag)
				{
					op = OpCodes.Conv_Ovf_I1_Un;
				}
				else
				{
					bool flag2 = @checked;
					if (flag2)
					{
						op = OpCodes.Conv_Ovf_I1;
					}
					else
					{
						op = OpCodes.Conv_I1;
					}
				}
				break;
			}
			case TypeCode.Byte:
			{
				bool flag3 = unsigned & @checked;
				if (flag3)
				{
					op = OpCodes.Conv_Ovf_U1_Un;
				}
				else
				{
					bool flag4 = @checked;
					if (flag4)
					{
						op = OpCodes.Conv_Ovf_U1;
					}
					else
					{
						op = OpCodes.Conv_U1;
					}
				}
				break;
			}
			case TypeCode.Int16:
			{
				bool flag5 = unsigned & @checked;
				if (flag5)
				{
					op = OpCodes.Conv_Ovf_I2_Un;
				}
				else
				{
					bool flag6 = @checked;
					if (flag6)
					{
						op = OpCodes.Conv_Ovf_I2;
					}
					else
					{
						op = OpCodes.Conv_I2;
					}
				}
				break;
			}
			case TypeCode.UInt16:
			{
				bool flag7 = unsigned & @checked;
				if (flag7)
				{
					op = OpCodes.Conv_Ovf_U2_Un;
				}
				else
				{
					bool flag8 = @checked;
					if (flag8)
					{
						op = OpCodes.Conv_Ovf_U2;
					}
					else
					{
						op = OpCodes.Conv_U2;
					}
				}
				break;
			}
			case TypeCode.Int32:
			{
				bool flag9 = unsigned & @checked;
				if (flag9)
				{
					op = OpCodes.Conv_Ovf_I4_Un;
				}
				else
				{
					bool flag10 = @checked;
					if (flag10)
					{
						op = OpCodes.Conv_Ovf_I4;
					}
					else
					{
						bool flag11 = sourcetc != TypeCode.UInt32;
						if (flag11)
						{
							op = OpCodes.Conv_I4;
						}
					}
				}
				break;
			}
			case TypeCode.UInt32:
			{
				bool flag12 = unsigned & @checked;
				if (flag12)
				{
					op = OpCodes.Conv_Ovf_U4_Un;
				}
				else
				{
					bool flag13 = @checked;
					if (flag13)
					{
						op = OpCodes.Conv_Ovf_U4;
					}
					else
					{
						bool flag14 = sourcetc != TypeCode.Int32;
						if (flag14)
						{
							op = OpCodes.Conv_U4;
						}
					}
				}
				break;
			}
			case TypeCode.Int64:
			{
				bool flag15 = unsigned & @checked;
				if (flag15)
				{
					op = OpCodes.Conv_Ovf_I8_Un;
				}
				else
				{
					bool flag16 = @checked;
					if (flag16)
					{
						op = OpCodes.Conv_Ovf_I8;
					}
					else
					{
						bool flag17 = sourcetc != TypeCode.UInt64;
						if (flag17)
						{
							op = OpCodes.Conv_I8;
						}
					}
				}
				break;
			}
			case TypeCode.UInt64:
			{
				bool flag18 = unsigned & @checked;
				if (flag18)
				{
					op = OpCodes.Conv_Ovf_U8_Un;
				}
				else
				{
					bool flag19 = @checked;
					if (flag19)
					{
						op = OpCodes.Conv_Ovf_U8;
					}
					else
					{
						bool flag20 = sourcetc != TypeCode.Int64;
						if (flag20)
						{
							op = OpCodes.Conv_U8;
						}
					}
				}
				break;
			}
			case TypeCode.Single:
				op = OpCodes.Conv_R4;
				break;
			default:
				Debug.Assert(false, "Unknown cast dest type");
				break;
			}
			bool flag21 = !op.Equals(OpCodes.Nop);
			if (flag21)
			{
				ilg.Emit(op);
			}
		}

		private static bool IsUnsignedType(Type t)
		{
			bool IsUnsignedType;
			switch (Type.GetTypeCode(t))
			{
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				IsUnsignedType = true;
				return IsUnsignedType;
			}
			IsUnsignedType = false;
			return IsUnsignedType;
		}
	}
}
