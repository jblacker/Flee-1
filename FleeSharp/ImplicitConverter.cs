using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal class ImplicitConverter
    {
        private static Type[,] OurBinaryResultTable;

        private static Type[] OurBinaryTypes;

        static ImplicitConverter()
        {
            Type[] types = new Type[]
            {
                typeof(char),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double)
            };
            OurBinaryTypes = types;
            Type[,] table = new Type[types.Length - 1 + 1, types.Length - 1 + 1];
            OurBinaryResultTable = table;
            FillIdentities(types, table);
            AddEntry(typeof(uint), typeof(ulong), typeof(ulong));
            AddEntry(typeof(int), typeof(long), typeof(long));
            AddEntry(typeof(uint), typeof(long), typeof(long));
            AddEntry(typeof(int), typeof(uint), typeof(long));
            AddEntry(typeof(uint), typeof(float), typeof(float));
            AddEntry(typeof(uint), typeof(double), typeof(double));
            AddEntry(typeof(int), typeof(float), typeof(float));
            AddEntry(typeof(int), typeof(double), typeof(double));
            AddEntry(typeof(long), typeof(float), typeof(float));
            AddEntry(typeof(long), typeof(double), typeof(double));
            AddEntry(typeof(ulong), typeof(float), typeof(float));
            AddEntry(typeof(ulong), typeof(double), typeof(double));
            AddEntry(typeof(float), typeof(double), typeof(double));
            AddEntry(typeof(byte), typeof(byte), typeof(int));
            AddEntry(typeof(byte), typeof(sbyte), typeof(int));
            AddEntry(typeof(byte), typeof(short), typeof(int));
            AddEntry(typeof(byte), typeof(ushort), typeof(int));
            AddEntry(typeof(byte), typeof(int), typeof(int));
            AddEntry(typeof(byte), typeof(uint), typeof(uint));
            AddEntry(typeof(byte), typeof(long), typeof(long));
            AddEntry(typeof(byte), typeof(ulong), typeof(ulong));
            AddEntry(typeof(byte), typeof(float), typeof(float));
            AddEntry(typeof(byte), typeof(double), typeof(double));
            AddEntry(typeof(sbyte), typeof(sbyte), typeof(int));
            AddEntry(typeof(sbyte), typeof(short), typeof(int));
            AddEntry(typeof(sbyte), typeof(ushort), typeof(int));
            AddEntry(typeof(sbyte), typeof(int), typeof(int));
            AddEntry(typeof(sbyte), typeof(uint), typeof(long));
            AddEntry(typeof(sbyte), typeof(long), typeof(long));
            AddEntry(typeof(sbyte), typeof(float), typeof(float));
            AddEntry(typeof(sbyte), typeof(double), typeof(double));
            AddEntry(typeof(short), typeof(short), typeof(int));
            AddEntry(typeof(short), typeof(ushort), typeof(int));
            AddEntry(typeof(short), typeof(int), typeof(int));
            AddEntry(typeof(short), typeof(uint), typeof(long));
            AddEntry(typeof(short), typeof(long), typeof(long));
            AddEntry(typeof(short), typeof(float), typeof(float));
            AddEntry(typeof(short), typeof(double), typeof(double));
            AddEntry(typeof(ushort), typeof(ushort), typeof(int));
            AddEntry(typeof(ushort), typeof(short), typeof(int));
            AddEntry(typeof(ushort), typeof(int), typeof(int));
            AddEntry(typeof(ushort), typeof(uint), typeof(uint));
            AddEntry(typeof(ushort), typeof(long), typeof(long));
            AddEntry(typeof(ushort), typeof(ulong), typeof(ulong));
            AddEntry(typeof(ushort), typeof(float), typeof(float));
            AddEntry(typeof(ushort), typeof(double), typeof(double));
            AddEntry(typeof(char), typeof(char), typeof(int));
            AddEntry(typeof(char), typeof(ushort), typeof(ushort));
            AddEntry(typeof(char), typeof(int), typeof(int));
            AddEntry(typeof(char), typeof(uint), typeof(uint));
            AddEntry(typeof(char), typeof(long), typeof(long));
            AddEntry(typeof(char), typeof(ulong), typeof(ulong));
            AddEntry(typeof(char), typeof(float), typeof(float));
            AddEntry(typeof(char), typeof(double), typeof(double));
        }

        private ImplicitConverter()
        {
        }

        private static void FillIdentities(Type[] typeList, Type[,] table)
        {
            int num = typeList.Length - 1;
            for (int i = 0; i <= num; i++)
            {
                Type t = typeList[i];
                table[i, i] = t;
            }
        }

        private static void AddEntry(Type t1, Type t2, Type result)
        {
            int index = GetTypeIndex(t1);
            int index2 = GetTypeIndex(t2);
            OurBinaryResultTable[index, index2] = result;
            OurBinaryResultTable[index2, index] = result;
        }

        private static int GetTypeIndex(Type t)
        {
            return Array.IndexOf<Type>(OurBinaryTypes, t);
        }

        public static bool EmitImplicitConvert(Type sourceType, Type destType, FleeILGenerator ilg)
        {
            bool flag = sourceType == destType;
            bool EmitImplicitConvert;
            if (flag)
            {
                EmitImplicitConvert = true;
            }
            else
            {
                bool flag2 = EmitOverloadedImplicitConvert(sourceType, destType, ilg);
                if (flag2)
                {
                    EmitImplicitConvert = true;
                }
                else
                {
                    bool flag3 = ImplicitConvertToReferenceType(sourceType, destType, ilg);
                    EmitImplicitConvert = (flag3 || ImplicitConvertToValueType(sourceType, destType, ilg));
                }
            }
            return EmitImplicitConvert;
        }

        private static bool EmitOverloadedImplicitConvert(Type sourceType, Type destType, FleeILGenerator ilg)
        {
            MethodInfo mi = Utility.GetSimpleOverloadedOperator("Implicit", sourceType, destType);
            bool flag = mi == null;
            bool EmitOverloadedImplicitConvert;
            if (flag)
            {
                EmitOverloadedImplicitConvert = false;
            }
            else
            {
                bool flag2 = ilg != null;
                if (flag2)
                {
                    ilg.Emit(OpCodes.Call, mi);
                }
                EmitOverloadedImplicitConvert = true;
            }
            return EmitOverloadedImplicitConvert;
        }

        private static bool ImplicitConvertToReferenceType(Type sourceType, Type destType, FleeILGenerator ilg)
        {
            bool isValueType = destType.IsValueType;
            bool ImplicitConvertToReferenceType;
            if (isValueType)
            {
                ImplicitConvertToReferenceType = false;
            }
            else
            {
                bool flag = sourceType == typeof(Null);
                if (flag)
                {
                    ImplicitConvertToReferenceType = true;
                }
                else
                {
                    bool flag2 = !destType.IsAssignableFrom(sourceType);
                    if (flag2)
                    {
                        ImplicitConvertToReferenceType = false;
                    }
                    else
                    {
                        bool isValueType2 = sourceType.IsValueType;
                        if (isValueType2)
                        {
                            bool flag3 = ilg != null;
                            if (flag3)
                            {
                                ilg.Emit(OpCodes.Box, sourceType);
                            }
                        }
                        ImplicitConvertToReferenceType = true;
                    }
                }
            }
            return ImplicitConvertToReferenceType;
        }

        private static bool ImplicitConvertToValueType(Type sourceType, Type destType, FleeILGenerator ilg)
        {
            bool flag = !sourceType.IsValueType & !destType.IsValueType;
            bool ImplicitConvertToValueType;
            if (flag)
            {
                ImplicitConvertToValueType = false;
            }
            else
            {
                bool flag2 = sourceType.IsEnum | destType.IsEnum;
                ImplicitConvertToValueType = (!flag2 && EmitImplicitNumericConvert(sourceType, destType, ilg));
            }
            return ImplicitConvertToValueType;
        }

        public static bool EmitImplicitNumericConvert(Type sourceType, Type destType, FleeILGenerator ilg)
        {
            TypeCode sourceTypeCode = Type.GetTypeCode(sourceType);
            bool EmitImplicitNumericConvert;
            switch (Type.GetTypeCode(destType))
            {
                case TypeCode.Int16:
                    EmitImplicitNumericConvert = ImplicitConvertToInt16(sourceTypeCode, ilg);
                    break;
                case TypeCode.UInt16:
                    EmitImplicitNumericConvert = ImplicitConvertToUInt16(sourceTypeCode, ilg);
                    break;
                case TypeCode.Int32:
                    EmitImplicitNumericConvert = ImplicitConvertToInt32(sourceTypeCode, ilg);
                    break;
                case TypeCode.UInt32:
                    EmitImplicitNumericConvert = ImplicitConvertToUInt32(sourceTypeCode, ilg);
                    break;
                case TypeCode.Int64:
                    EmitImplicitNumericConvert = ImplicitConvertToInt64(sourceTypeCode, ilg);
                    break;
                case TypeCode.UInt64:
                    EmitImplicitNumericConvert = ImplicitConvertToUInt64(sourceTypeCode, ilg);
                    break;
                case TypeCode.Single:
                    EmitImplicitNumericConvert = ImplicitConvertToSingle(sourceTypeCode, ilg);
                    break;
                case TypeCode.Double:
                    EmitImplicitNumericConvert = ImplicitConvertToDouble(sourceTypeCode, ilg);
                    break;
                default:
                    EmitImplicitNumericConvert = false;
                    break;
            }
            return EmitImplicitNumericConvert;
        }

        private static bool ImplicitConvertToInt16(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            return sourceTypeCode - TypeCode.SByte <= 2;
        }

        private static bool ImplicitConvertToUInt16(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            bool ImplicitConvertToUInt16;
            switch (sourceTypeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                    ImplicitConvertToUInt16 = true;
                    return ImplicitConvertToUInt16;
            }
            ImplicitConvertToUInt16 = false;
            return ImplicitConvertToUInt16;
        }

        private static bool ImplicitConvertToInt32(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            return sourceTypeCode - TypeCode.Char <= 5;
        }

        private static bool ImplicitConvertToUInt32(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            return sourceTypeCode - TypeCode.Char <= 4 || sourceTypeCode == TypeCode.UInt32;
        }

        private static bool ImplicitConvertToDouble(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            bool ImplicitConvertToDouble;
            switch (sourceTypeCode)
            {
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                    EmitConvert(ilg, OpCodes.Conv_R8);
                    break;
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    EmitConvert(ilg, OpCodes.Conv_R_Un);
                    EmitConvert(ilg, OpCodes.Conv_R8);
                    break;
                case TypeCode.Double:
                    break;
                default:
                    ImplicitConvertToDouble = false;
                    return ImplicitConvertToDouble;
            }
            ImplicitConvertToDouble = true;
            return ImplicitConvertToDouble;
        }

        private static bool ImplicitConvertToSingle(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            bool ImplicitConvertToSingle;
            switch (sourceTypeCode)
            {
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    EmitConvert(ilg, OpCodes.Conv_R4);
                    break;
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    EmitConvert(ilg, OpCodes.Conv_R_Un);
                    EmitConvert(ilg, OpCodes.Conv_R4);
                    break;
                case TypeCode.Single:
                    break;
                default:
                    ImplicitConvertToSingle = false;
                    return ImplicitConvertToSingle;
            }
            ImplicitConvertToSingle = true;
            return ImplicitConvertToSingle;
        }

        private static bool ImplicitConvertToInt64(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            bool ImplicitConvertToInt64;
            switch (sourceTypeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    EmitConvert(ilg, OpCodes.Conv_U8);
                    break;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                    EmitConvert(ilg, OpCodes.Conv_I8);
                    break;
                case TypeCode.Int64:
                    break;
                default:
                    ImplicitConvertToInt64 = false;
                    return ImplicitConvertToInt64;
            }
            ImplicitConvertToInt64 = true;
            return ImplicitConvertToInt64;
        }

        private static bool ImplicitConvertToUInt64(TypeCode sourceTypeCode, FleeILGenerator ilg)
        {
            switch (sourceTypeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    EmitConvert(ilg, OpCodes.Conv_U8);
                    goto IL_49;
                case TypeCode.UInt64:
                    goto IL_49;
            }
            bool ImplicitConvertToUInt64 = false;
            return ImplicitConvertToUInt64;
            IL_49:
            ImplicitConvertToUInt64 = true;
            return ImplicitConvertToUInt64;
        }

        private static void EmitConvert(FleeILGenerator ilg, OpCode convertOpcode)
        {
            bool flag = ilg != null;
            if (flag)
            {
                ilg.Emit(convertOpcode);
            }
        }

        public static Type GetBinaryResultType(Type t1, Type t2)
        {
            int index = GetTypeIndex(t1);
            int index2 = GetTypeIndex(t2);
            bool flag = index == -1 | index2 == -1;
            Type GetBinaryResultType;
            if (flag)
            {
                GetBinaryResultType = null;
            }
            else
            {
                GetBinaryResultType = OurBinaryResultTable[index, index2];
            }
            return GetBinaryResultType;
        }

        public static int GetImplicitConvertScore(Type sourceType, Type destType)
        {
            bool flag = sourceType == destType;
            int GetImplicitConvertScore;
            if (flag)
            {
                GetImplicitConvertScore = 0;
            }
            else
            {
                bool flag2 = sourceType == typeof(Null);
                if (flag2)
                {
                    GetImplicitConvertScore = GetInverseDistanceToObject(destType);
                }
                else
                {
                    bool flag3 = Utility.GetSimpleOverloadedOperator("Implicit", sourceType, destType) != null;
                    if (flag3)
                    {
                        GetImplicitConvertScore = 1;
                    }
                    else
                    {
                        bool isValueType = sourceType.IsValueType;
                        if (isValueType)
                        {
                            bool isValueType2 = destType.IsValueType;
                            if (isValueType2)
                            {
                                int sourceScore = GetValueTypeImplicitConvertScore(sourceType);
                                int destScore = GetValueTypeImplicitConvertScore(destType);
                                GetImplicitConvertScore = destScore - sourceScore;
                            }
                            else
                            {
                                GetImplicitConvertScore = GetReferenceTypeImplicitConvertScore(sourceType, destType);
                            }
                        }
                        else
                        {
                            bool isValueType3 = destType.IsValueType;
                            if (isValueType3)
                            {
                                Debug.Fail("No implicit conversion from reference type to value type");
                            }
                            else
                            {
                                GetImplicitConvertScore = GetReferenceTypeImplicitConvertScore(sourceType, destType);
                            }
                        }
                    }
                }
            }
            return GetImplicitConvertScore;
        }

        private static int GetValueTypeImplicitConvertScore(Type t)
        {
            int GetValueTypeImplicitConvertScore;
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                    GetValueTypeImplicitConvertScore = 12;
                    break;
                case TypeCode.Char:
                    GetValueTypeImplicitConvertScore = 3;
                    break;
                case TypeCode.SByte:
                    GetValueTypeImplicitConvertScore = 2;
                    break;
                case TypeCode.Byte:
                    GetValueTypeImplicitConvertScore = 1;
                    break;
                case TypeCode.Int16:
                    GetValueTypeImplicitConvertScore = 4;
                    break;
                case TypeCode.UInt16:
                    GetValueTypeImplicitConvertScore = 5;
                    break;
                case TypeCode.Int32:
                    GetValueTypeImplicitConvertScore = 6;
                    break;
                case TypeCode.UInt32:
                    GetValueTypeImplicitConvertScore = 7;
                    break;
                case TypeCode.Int64:
                    GetValueTypeImplicitConvertScore = 8;
                    break;
                case TypeCode.UInt64:
                    GetValueTypeImplicitConvertScore = 9;
                    break;
                case TypeCode.Single:
                    GetValueTypeImplicitConvertScore = 10;
                    break;
                case TypeCode.Double:
                    GetValueTypeImplicitConvertScore = 11;
                    break;
                case TypeCode.Decimal:
                    GetValueTypeImplicitConvertScore = 11;
                    break;
                case TypeCode.DateTime:
                    GetValueTypeImplicitConvertScore = 13;
                    break;
                default:
                    Debug.Assert(false, "unknown value type");
                    GetValueTypeImplicitConvertScore = -1;
                    break;
            }
            return GetValueTypeImplicitConvertScore;
        }

        private static int GetReferenceTypeImplicitConvertScore(Type sourceType, Type destType)
        {
            bool isInterface = destType.IsInterface;
            int GetReferenceTypeImplicitConvertScore;
            if (isInterface)
            {
                GetReferenceTypeImplicitConvertScore = 100;
            }
            else
            {
                GetReferenceTypeImplicitConvertScore = GetInheritanceDistance(sourceType, destType);
            }
            return GetReferenceTypeImplicitConvertScore;
        }

        private static int GetInheritanceDistance(Type sourceType, Type destType)
        {
            int count = 0;
            for (Type current = sourceType; current != destType; current = current.BaseType)
            {
                count++;
            }
            return count * 1000;
        }

        private static int GetInverseDistanceToObject(Type t)
        {
            int score = 1000;
            for (Type current = t.BaseType; current != null; current = current.BaseType)
            {
                score -= 100;
            }
            return score;
        }
    }
}