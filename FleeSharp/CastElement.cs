namespace Flee
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class CastElement : ExpressionElement
    {
        private readonly ExpressionElement myCastExpression;

        private readonly Type myDestType;

        public CastElement(ExpressionElement castExpression, string[] destTypeParts, bool isArray, IServiceProvider services)
        {
            this.myCastExpression = castExpression;
            this.myDestType = GetDestType(destTypeParts, services);
            var flag = this.myDestType == null;
            if (flag)
            {
                this.ThrowCompileException("CouldNotResolveType", CompileExceptionReason.UndefinedName,
                    GetDestTypeString(destTypeParts, isArray));
            }
            if (isArray)
            {
                this.myDestType = this.myDestType.MakeArrayType();
            }
            var flag2 = !this.IsValidCast(this.myCastExpression.ResultType, this.myDestType);
            if (flag2)
            {
                this.ThrowInvalidCastException();
            }
        }

        private static string GetDestTypeString(string[] parts, bool isArray)
        {
            var s = string.Join(".", parts);
            if (isArray)
            {
                s += "[]";
            }
            return s;
        }

        private static Type GetDestType(string[] destTypeParts, IServiceProvider services)
        {
            var context = (ExpressionContext) services.GetService(typeof(ExpressionContext));
            Type t = null;
            var flag = destTypeParts.Length == 1;
            if (flag)
            {
                t = ExpressionImports.GetBuiltinType(destTypeParts[0]);
            }
            var flag2 = t != null;
            Type getDestType;
            if (flag2)
            {
                getDestType = t;
            }
            else
            {
                t = context.Imports.FindType(destTypeParts);
                var flag3 = t != null;
                getDestType = flag3 ? t : null;
            }
            return getDestType;
        }

        private bool IsValidCast(Type sourceType, Type destType)
        {
            var flag = sourceType == destType;
            bool isValidCast;
            if (flag)
            {
                isValidCast = true;
            }
            else
            {
                var flag2 = destType.IsAssignableFrom(sourceType);
                if (flag2)
                {
                    isValidCast = true;
                }
                else
                {
                    var flag3 = ImplicitConverter.EmitImplicitConvert(sourceType, destType, null);
                    if (flag3)
                    {
                        isValidCast = true;
                    }
                    else
                    {
                        var flag4 = IsCastableNumericType(sourceType) & IsCastableNumericType(destType);
                        if (flag4)
                        {
                            isValidCast = true;
                        }
                        else
                        {
                            var flag5 = sourceType.IsEnum | destType.IsEnum;
                            if (flag5)
                            {
                                isValidCast = this.IsValidExplicitEnumCast(sourceType, destType);
                            }
                            else
                            {
                                var flag6 = this.GetExplictOverloadedOperator(sourceType, destType) != null;
                                if (flag6)
                                {
                                    isValidCast = true;
                                }
                                else
                                {
                                    var isValueType = sourceType.IsValueType;
                                    if (isValueType)
                                    {
                                        isValidCast = false;
                                    }
                                    else
                                    {
                                        var isValueType2 = destType.IsValueType;
                                        if (isValueType2)
                                        {
                                            var interfaces = destType.GetInterfaces();
                                            isValidCast = IsBaseType(destType, sourceType) |
                                                (Array.IndexOf(interfaces, sourceType) != -1);
                                        }
                                        else
                                        {
                                            isValidCast = this.IsValidExplicitReferenceCast(sourceType, destType);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isValidCast;
        }

        private MethodInfo GetExplictOverloadedOperator(Type sourceType, Type destType)
        {
            var binder = new ExplicitOperatorMethodBinder(destType, sourceType);
            var miSource = Utility.GetOverloadedOperator("Explicit", sourceType, binder, sourceType);
            var miDest = Utility.GetOverloadedOperator("Explicit", destType, binder, sourceType);
            var flag = (miSource == null) & (miDest == null);
            MethodInfo getExplictOverloadedOperator;
            if (flag)
            {
                getExplictOverloadedOperator = null;
            }
            else
            {
                var flag2 = miSource == null;
                if (flag2)
                {
                    getExplictOverloadedOperator = miDest;
                }
                else
                {
                    var flag3 = miDest == null;
                    if (flag3)
                    {
                        getExplictOverloadedOperator = miSource;
                    }
                    else
                    {
                        this.ThrowAmbiguousCallException(sourceType, destType, "Explicit");
                        getExplictOverloadedOperator = null;
                    }
                }
            }
            return getExplictOverloadedOperator;
        }

        private bool IsValidExplicitEnumCast(Type sourceType, Type destType)
        {
            sourceType = GetUnderlyingEnumType(sourceType);
            destType = GetUnderlyingEnumType(destType);
            return this.IsValidCast(sourceType, destType);
        }

        private bool IsValidExplicitReferenceCast(Type sourceType, Type destType)
        {
            Debug.Assert(!sourceType.IsValueType & !destType.IsValueType, "expecting reference types");
            var flag = sourceType == typeof(object);
            bool isValidExplicitReferenceCast = false;
            if (flag)
            {
                isValidExplicitReferenceCast = true;
            }
            else
            {
                var flag2 = sourceType.IsArray & destType.IsArray;
                if (flag2)
                {
                    var flag3 = sourceType.GetArrayRank() != destType.GetArrayRank();
                    if (flag3)
                    {
                        return false;
                    }
                    var se = sourceType.GetElementType();
                    var te = destType.GetElementType();
                    var flag4 = se.IsValueType | te.IsValueType;
                    isValidExplicitReferenceCast = !flag4 && this.IsValidExplicitReferenceCast(se, te);
                }
                else
                {
                    var flag5 = sourceType.IsClass & destType.IsClass;
                    if (flag5)
                    {
                        isValidExplicitReferenceCast = IsBaseType(destType, sourceType);
                    }
                    else
                    {
                        var flag6 = sourceType.IsClass & destType.IsInterface;
                        if (flag6)
                        {
                            isValidExplicitReferenceCast = !sourceType.IsSealed & !ImplementsInterface(sourceType, destType);
                        }
                        else
                        {
                            var flag7 = sourceType.IsInterface & destType.IsClass;
                            if (flag7)
                            {
                                isValidExplicitReferenceCast = !destType.IsSealed | ImplementsInterface(destType, sourceType);
                            }
                            else
                            {
                                var flag8 = sourceType.IsInterface & destType.IsInterface;
                                if (flag8)
                                {
                                    isValidExplicitReferenceCast = !ImplementsInterface(sourceType, destType);
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
            return isValidExplicitReferenceCast;
        }

        private static bool IsBaseType(Type target, Type potentialBase)
        {
            for (var current = target; current != null; current = current.BaseType)
            {
                var flag = current == potentialBase;
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ImplementsInterface(Type target, Type interfaceType)
        {
            var interfaces = target.GetInterfaces();
            return Array.IndexOf(interfaces, interfaceType) != -1;
        }

        private void ThrowInvalidCastException()
        {
            this.ThrowCompileException("CannotConvertType", CompileExceptionReason.InvalidExplicitCast,
                this.myCastExpression.ResultType.Name, this.myDestType.Name);
        }

        private static bool IsCastableNumericType(Type t)
        {
            return t.IsPrimitive & (t != typeof(bool));
        }

        private static Type GetUnderlyingEnumType(Type t)
        {
            var isEnum = t.IsEnum;
            var getUnderlyingEnumType = isEnum ? Enum.GetUnderlyingType(t) : t;
            return getUnderlyingEnumType;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            this.myCastExpression.Emit(ilg, services);
            var sourceType = this.myCastExpression.ResultType;
            var destType = this.myDestType;
            this.EmitCast(ilg, sourceType, destType, services);
        }

        private void EmitCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
        {
            var explicitOperator = this.GetExplictOverloadedOperator(sourceType, destType);
            var flag = sourceType == destType;
            if (!flag)
            {
                var flag2 = explicitOperator != null;
                if (flag2)
                {
                    ilg.Emit(OpCodes.Call, explicitOperator);
                }
                else
                {
                    var flag3 = sourceType.IsEnum | destType.IsEnum;
                    if (flag3)
                    {
                        this.EmitEnumCast(ilg, sourceType, destType, services);
                    }
                    else
                    {
                        var flag4 = ImplicitConverter.EmitImplicitConvert(sourceType, destType, ilg);
                        if (!flag4)
                        {
                            var flag5 = IsCastableNumericType(sourceType) & IsCastableNumericType(destType);
                            if (flag5)
                            {
                                EmitExplicitNumericCast(ilg, sourceType, destType, services);
                            }
                            else
                            {
                                var isValueType = sourceType.IsValueType;
                                if (isValueType)
                                {
                                    Debug.Assert(!destType.IsValueType, "expecting reference type");
                                    ilg.Emit(OpCodes.Box, sourceType);
                                }
                                else
                                {
                                    var isValueType2 = destType.IsValueType;
                                    if (isValueType2)
                                    {
                                        ilg.Emit(OpCodes.Unbox_Any, destType);
                                    }
                                    else
                                    {
                                        var flag6 = !destType.IsAssignableFrom(sourceType);
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
            var flag = !destType.IsValueType;
            if (flag)
            {
                ilg.Emit(OpCodes.Box, sourceType);
            }
            else
            {
                var flag2 = !sourceType.IsValueType;
                if (flag2)
                {
                    ilg.Emit(OpCodes.Unbox_Any, destType);
                }
                else
                {
                    sourceType = GetUnderlyingEnumType(sourceType);
                    destType = GetUnderlyingEnumType(destType);
                    this.EmitCast(ilg, sourceType, destType, services);
                }
            }
        }

        private static void EmitExplicitNumericCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
        {
            var desttc = Type.GetTypeCode(destType);
            var sourcetc = Type.GetTypeCode(sourceType);
            var unsigned = IsUnsignedType(sourceType);
            var options = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));
            var @checked = options.Checked;
            var op = OpCodes.Nop;
            switch (desttc)
            {
                case TypeCode.SByte:
                {
                    var flag = unsigned & @checked;
                    if (flag)
                    {
                        op = OpCodes.Conv_Ovf_I1_Un;
                    }
                    else
                    {
                        var flag2 = @checked;
                        op = flag2 ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1;
                    }
                    break;
                }
                case TypeCode.Byte:
                {
                    var flag3 = unsigned & @checked;
                    if (flag3)
                    {
                        op = OpCodes.Conv_Ovf_U1_Un;
                    }
                    else
                    {
                        var flag4 = @checked;
                        op = flag4 ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1;
                    }
                    break;
                }
                case TypeCode.Int16:
                {
                    var flag5 = unsigned & @checked;
                    if (flag5)
                    {
                        op = OpCodes.Conv_Ovf_I2_Un;
                    }
                    else
                    {
                        var flag6 = @checked;
                        op = flag6 ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2;
                    }
                    break;
                }
                case TypeCode.UInt16:
                {
                    var flag7 = unsigned & @checked;
                    if (flag7)
                    {
                        op = OpCodes.Conv_Ovf_U2_Un;
                    }
                    else
                    {
                        var flag8 = @checked;
                        op = flag8 ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2;
                    }
                    break;
                }
                case TypeCode.Int32:
                {
                    var flag9 = unsigned & @checked;
                    if (flag9)
                    {
                        op = OpCodes.Conv_Ovf_I4_Un;
                    }
                    else
                    {
                        var flag10 = @checked;
                        if (flag10)
                        {
                            op = OpCodes.Conv_Ovf_I4;
                        }
                        else
                        {
                            var flag11 = sourcetc != TypeCode.UInt32;
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
                    var flag12 = unsigned & @checked;
                    if (flag12)
                    {
                        op = OpCodes.Conv_Ovf_U4_Un;
                    }
                    else
                    {
                        var flag13 = @checked;
                        if (flag13)
                        {
                            op = OpCodes.Conv_Ovf_U4;
                        }
                        else
                        {
                            var flag14 = sourcetc != TypeCode.Int32;
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
                    var flag15 = unsigned & @checked;
                    if (flag15)
                    {
                        op = OpCodes.Conv_Ovf_I8_Un;
                    }
                    else
                    {
                        var flag16 = @checked;
                        if (flag16)
                        {
                            op = OpCodes.Conv_Ovf_I8;
                        }
                        else
                        {
                            var flag17 = sourcetc != TypeCode.UInt64;
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
                    var flag18 = unsigned & @checked;
                    if (flag18)
                    {
                        op = OpCodes.Conv_Ovf_U8_Un;
                    }
                    else
                    {
                        var flag19 = @checked;
                        if (flag19)
                        {
                            op = OpCodes.Conv_Ovf_U8;
                        }
                        else
                        {
                            var flag20 = sourcetc != TypeCode.Int64;
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
            var flag21 = !op.Equals(OpCodes.Nop);
            if (flag21)
            {
                ilg.Emit(op);
            }
        }

        private static bool IsUnsignedType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }
            return false;
        }

        public override Type ResultType => this.myDestType;
    }
}