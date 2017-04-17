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
    using System.Diagnostics;
    using System.Reflection.Emit;

    internal class ImplicitConverter
    {
        private static readonly Type[,] ourBinaryResultTable;

        private static readonly Type[] ourBinaryTypes;

        static ImplicitConverter()
        {
            Type[] types =
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
            ourBinaryTypes = types;
            var table = new Type[types.Length - 1 + 1, types.Length - 1 + 1];
            ourBinaryResultTable = table;
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
            var num = typeList.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                var t = typeList[i];
                table[i, i] = t;
            }
        }

        private static void AddEntry(Type t1, Type t2, Type result)
        {
            var index = GetTypeIndex(t1);
            var index2 = GetTypeIndex(t2);
            ourBinaryResultTable[index, index2] = result;
            ourBinaryResultTable[index2, index] = result;
        }

        private static int GetTypeIndex(Type t)
        {
            return Array.IndexOf(ourBinaryTypes, t);
        }

        public static bool EmitImplicitConvert(Type sourceType, Type destType, FleeIlGenerator ilg)
        {
            var flag = sourceType == destType;
            bool emitImplicitConvert;
            if (flag)
            {
                emitImplicitConvert = true;
            }
            else
            {
                var flag2 = EmitOverloadedImplicitConvert(sourceType, destType, ilg);
                if (flag2)
                {
                    emitImplicitConvert = true;
                }
                else
                {
                    var flag3 = ImplicitConvertToReferenceType(sourceType, destType, ilg);
                    emitImplicitConvert = flag3 || ImplicitConvertToValueType(sourceType, destType, ilg);
                }
            }
            return emitImplicitConvert;
        }

        private static bool EmitOverloadedImplicitConvert(Type sourceType, Type destType, FleeIlGenerator ilg)
        {
            var mi = Utility.GetSimpleOverloadedOperator("Implicit", sourceType, destType);
            var flag = mi == null;
            bool emitOverloadedImplicitConvert;
            if (flag)
            {
                emitOverloadedImplicitConvert = false;
            }
            else
            {
                var flag2 = ilg != null;
                if (flag2)
                {
                    ilg.Emit(OpCodes.Call, mi);
                }
                emitOverloadedImplicitConvert = true;
            }
            return emitOverloadedImplicitConvert;
        }

        private static bool ImplicitConvertToReferenceType(Type sourceType, Type destType, FleeIlGenerator ilg)
        {
            var isValueType = destType.IsValueType;
            bool implicitConvertToReferenceType;
            if (isValueType)
            {
                implicitConvertToReferenceType = false;
            }
            else
            {
                var flag = sourceType == typeof(Null);
                if (flag)
                {
                    implicitConvertToReferenceType = true;
                }
                else
                {
                    var flag2 = !destType.IsAssignableFrom(sourceType);
                    if (flag2)
                    {
                        implicitConvertToReferenceType = false;
                    }
                    else
                    {
                        var isValueType2 = sourceType.IsValueType;
                        if (isValueType2)
                        {
                            var flag3 = ilg != null;
                            if (flag3)
                            {
                                ilg.Emit(OpCodes.Box, sourceType);
                            }
                        }
                        implicitConvertToReferenceType = true;
                    }
                }
            }
            return implicitConvertToReferenceType;
        }

        private static bool ImplicitConvertToValueType(Type sourceType, Type destType, FleeIlGenerator ilg)
        {
            var flag = !sourceType.IsValueType & !destType.IsValueType;
            bool implicitConvertToValueType;
            if (flag)
            {
                implicitConvertToValueType = false;
            }
            else
            {
                var flag2 = sourceType.IsEnum | destType.IsEnum;
                implicitConvertToValueType = !flag2 && EmitImplicitNumericConvert(sourceType, destType, ilg);
            }
            return implicitConvertToValueType;
        }

        public static bool EmitImplicitNumericConvert(Type sourceType, Type destType, FleeIlGenerator ilg)
        {
            var sourceTypeCode = Type.GetTypeCode(sourceType);
            bool emitImplicitNumericConvert;
            switch (Type.GetTypeCode(destType))
            {
                case TypeCode.Int16:
                    emitImplicitNumericConvert = ImplicitConvertToInt16(sourceTypeCode, ilg);
                    break;
                case TypeCode.UInt16:
                    emitImplicitNumericConvert = ImplicitConvertToUInt16(sourceTypeCode, ilg);
                    break;
                case TypeCode.Int32:
                    emitImplicitNumericConvert = ImplicitConvertToInt32(sourceTypeCode, ilg);
                    break;
                case TypeCode.UInt32:
                    emitImplicitNumericConvert = ImplicitConvertToUInt32(sourceTypeCode, ilg);
                    break;
                case TypeCode.Int64:
                    emitImplicitNumericConvert = ImplicitConvertToInt64(sourceTypeCode, ilg);
                    break;
                case TypeCode.UInt64:
                    emitImplicitNumericConvert = ImplicitConvertToUInt64(sourceTypeCode, ilg);
                    break;
                case TypeCode.Single:
                    emitImplicitNumericConvert = ImplicitConvertToSingle(sourceTypeCode, ilg);
                    break;
                case TypeCode.Double:
                    emitImplicitNumericConvert = ImplicitConvertToDouble(sourceTypeCode, ilg);
                    break;
                default:
                    emitImplicitNumericConvert = false;
                    break;
            }
            return emitImplicitNumericConvert;
        }

        private static bool ImplicitConvertToInt16(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            return sourceTypeCode - TypeCode.SByte <= 2;
        }

        private static bool ImplicitConvertToUInt16(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            bool implicitConvertToUInt16;
            switch (sourceTypeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                    implicitConvertToUInt16 = true;
                    return implicitConvertToUInt16;
            }
            implicitConvertToUInt16 = false;
            return implicitConvertToUInt16;
        }

        private static bool ImplicitConvertToInt32(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            return sourceTypeCode - TypeCode.Char <= 5;
        }

        private static bool ImplicitConvertToUInt32(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            return sourceTypeCode - TypeCode.Char <= 4 || sourceTypeCode == TypeCode.UInt32;
        }

        private static bool ImplicitConvertToDouble(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            bool implicitConvertToDouble;
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
                    implicitConvertToDouble = false;
                    return implicitConvertToDouble;
            }
            implicitConvertToDouble = true;
            return implicitConvertToDouble;
        }

        private static bool ImplicitConvertToSingle(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            bool implicitConvertToSingle;
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
                    implicitConvertToSingle = false;
                    return implicitConvertToSingle;
            }
            implicitConvertToSingle = true;
            return implicitConvertToSingle;
        }

        private static bool ImplicitConvertToInt64(TypeCode sourceTypeCode, FleeIlGenerator ilg)
        {
            bool implicitConvertToInt64;
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
                    implicitConvertToInt64 = false;
                    return implicitConvertToInt64;
            }
            implicitConvertToInt64 = true;
            return implicitConvertToInt64;
        }

        private static bool ImplicitConvertToUInt64(TypeCode sourceTypeCode, FleeIlGenerator ilg)
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
            var implicitConvertToUInt64 = false;
            return implicitConvertToUInt64;
            IL_49:
            implicitConvertToUInt64 = true;
            return implicitConvertToUInt64;
        }

        private static void EmitConvert(FleeIlGenerator ilg, OpCode convertOpcode)
        {
            var flag = ilg != null;
            if (flag)
            {
                ilg.Emit(convertOpcode);
            }
        }

        public static Type GetBinaryResultType(Type t1, Type t2)
        {
            var index = GetTypeIndex(t1);
            var index2 = GetTypeIndex(t2);
            var flag = (index == -1) | (index2 == -1);
            Type getBinaryResultType;
            if (flag)
            {
                getBinaryResultType = null;
            }
            else
            {
                getBinaryResultType = ourBinaryResultTable[index, index2];
            }
            return getBinaryResultType;
        }

        public static int GetImplicitConvertScore(Type sourceType, Type destType)
        {
            var flag = sourceType == destType;
            var getImplicitConvertScore = 0;
            if (flag)
            {
                getImplicitConvertScore = 0;
            }
            else
            {
                var flag2 = sourceType == typeof(Null);
                if (flag2)
                {
                    getImplicitConvertScore = GetInverseDistanceToObject(destType);
                }
                else
                {
                    var flag3 = Utility.GetSimpleOverloadedOperator("Implicit", sourceType, destType) != null;
                    if (flag3)
                    {
                        getImplicitConvertScore = 1;
                    }
                    else
                    {
                        var isValueType = sourceType.IsValueType;
                        if (isValueType)
                        {
                            var isValueType2 = destType.IsValueType;
                            if (isValueType2)
                            {
                                var sourceScore = GetValueTypeImplicitConvertScore(sourceType);
                                var destScore = GetValueTypeImplicitConvertScore(destType);
                                getImplicitConvertScore = destScore - sourceScore;
                            }
                            else
                            {
                                getImplicitConvertScore = GetReferenceTypeImplicitConvertScore(sourceType, destType);
                            }
                        }
                        else
                        {
                            var isValueType3 = destType.IsValueType;
                            if (isValueType3)
                            {
                                Debug.Fail("No implicit conversion from reference type to value type");
                            }
                            else
                            {
                                getImplicitConvertScore = GetReferenceTypeImplicitConvertScore(sourceType, destType);
                            }
                        }
                    }
                }
            }
            return getImplicitConvertScore;
        }

        private static int GetValueTypeImplicitConvertScore(Type t)
        {
            int getValueTypeImplicitConvertScore;
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                    getValueTypeImplicitConvertScore = 12;
                    break;
                case TypeCode.Char:
                    getValueTypeImplicitConvertScore = 3;
                    break;
                case TypeCode.SByte:
                    getValueTypeImplicitConvertScore = 2;
                    break;
                case TypeCode.Byte:
                    getValueTypeImplicitConvertScore = 1;
                    break;
                case TypeCode.Int16:
                    getValueTypeImplicitConvertScore = 4;
                    break;
                case TypeCode.UInt16:
                    getValueTypeImplicitConvertScore = 5;
                    break;
                case TypeCode.Int32:
                    getValueTypeImplicitConvertScore = 6;
                    break;
                case TypeCode.UInt32:
                    getValueTypeImplicitConvertScore = 7;
                    break;
                case TypeCode.Int64:
                    getValueTypeImplicitConvertScore = 8;
                    break;
                case TypeCode.UInt64:
                    getValueTypeImplicitConvertScore = 9;
                    break;
                case TypeCode.Single:
                    getValueTypeImplicitConvertScore = 10;
                    break;
                case TypeCode.Double:
                    getValueTypeImplicitConvertScore = 11;
                    break;
                case TypeCode.Decimal:
                    getValueTypeImplicitConvertScore = 11;
                    break;
                case TypeCode.DateTime:
                    getValueTypeImplicitConvertScore = 13;
                    break;
                default:
                    Debug.Assert(false, "unknown value type");
                    getValueTypeImplicitConvertScore = -1;
                    break;
            }
            return getValueTypeImplicitConvertScore;
        }

        private static int GetReferenceTypeImplicitConvertScore(Type sourceType, Type destType)
        {
            var isInterface = destType.IsInterface;
            int getReferenceTypeImplicitConvertScore;
            if (isInterface)
            {
                getReferenceTypeImplicitConvertScore = 100;
            }
            else
            {
                getReferenceTypeImplicitConvertScore = GetInheritanceDistance(sourceType, destType);
            }
            return getReferenceTypeImplicitConvertScore;
        }

        private static int GetInheritanceDistance(Type sourceType, Type destType)
        {
            var count = 0;
            for (var current = sourceType; current != destType; current = current.BaseType)
            {
                count++;
            }
            return count * 1000;
        }

        private static int GetInverseDistanceToObject(Type t)
        {
            var score = 1000;
            for (var current = t.BaseType; current != null; current = current.BaseType)
            {
                score -= 100;
            }
            return score;
        }
    }
}