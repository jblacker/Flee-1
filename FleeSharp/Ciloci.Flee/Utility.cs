using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class Utility
	{
		private Utility()
		{
		}

		public static void AssertNotNull(object o, string paramName)
		{
			bool flag = o == null;
			if (flag)
			{
				throw new ArgumentNullException(paramName);
			}
		}

		public static void EmitStoreLocal(FleeILGenerator ilg, int index)
		{
			bool flag = index >= 0 & index <= 3;
			if (flag)
			{
				switch (index)
				{
				case 0:
					ilg.Emit(OpCodes.Stloc_0);
					break;
				case 1:
					ilg.Emit(OpCodes.Stloc_1);
					break;
				case 2:
					ilg.Emit(OpCodes.Stloc_2);
					break;
				case 3:
					ilg.Emit(OpCodes.Stloc_3);
					break;
				}
			}
			else
			{
				Debug.Assert(index < 256, "local index too large");
				ilg.Emit(OpCodes.Stloc_S, (byte)index);
			}
		}

		public static void EmitLoadLocal(FleeILGenerator ilg, int index)
		{
			Debug.Assert(index >= 0, "Invalid index");
			bool flag = index >= 0 & index <= 3;
			if (flag)
			{
				switch (index)
				{
				case 0:
					ilg.Emit(OpCodes.Ldloc_0);
					break;
				case 1:
					ilg.Emit(OpCodes.Ldloc_1);
					break;
				case 2:
					ilg.Emit(OpCodes.Ldloc_2);
					break;
				case 3:
					ilg.Emit(OpCodes.Ldloc_3);
					break;
				}
			}
			else
			{
				Debug.Assert(index < 256, "local index too large");
				ilg.Emit(OpCodes.Ldloc_S, (byte)index);
			}
		}

		public static void EmitLoadLocalAddress(FleeILGenerator ilg, int index)
		{
			Debug.Assert(index >= 0, "Invalid index");
			bool flag = index <= 255;
			if (flag)
			{
				ilg.Emit(OpCodes.Ldloca_S, (byte)index);
			}
			else
			{
				ilg.Emit(OpCodes.Ldloca, index);
			}
		}

		public static void EmitArrayLoad(FleeILGenerator ilg, Type elementType)
		{
			switch (Type.GetTypeCode(elementType))
			{
			case TypeCode.Object:
			case TypeCode.String:
				ilg.Emit(OpCodes.Ldelem_Ref);
				return;
			case TypeCode.Boolean:
			case TypeCode.SByte:
				ilg.Emit(OpCodes.Ldelem_I1);
				return;
			case TypeCode.Byte:
				ilg.Emit(OpCodes.Ldelem_U1);
				return;
			case TypeCode.Int16:
				ilg.Emit(OpCodes.Ldelem_I2);
				return;
			case TypeCode.UInt16:
				ilg.Emit(OpCodes.Ldelem_U2);
				return;
			case TypeCode.Int32:
				ilg.Emit(OpCodes.Ldelem_I4);
				return;
			case TypeCode.UInt32:
				ilg.Emit(OpCodes.Ldelem_U4);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				ilg.Emit(OpCodes.Ldelem_I8);
				return;
			case TypeCode.Single:
				ilg.Emit(OpCodes.Ldelem_R4);
				return;
			case TypeCode.Double:
				ilg.Emit(OpCodes.Ldelem_R8);
				return;
			}
			ilg.Emit(OpCodes.Ldelema, elementType);
			ilg.Emit(OpCodes.Ldobj, elementType);
		}

		public static void EmitArrayStore(FleeILGenerator ilg, Type elementType)
		{
			switch (Type.GetTypeCode(elementType))
			{
			case TypeCode.Object:
			case TypeCode.String:
				ilg.Emit(OpCodes.Stelem_Ref);
				return;
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				ilg.Emit(OpCodes.Stelem_I1);
				return;
			case TypeCode.Int16:
			case TypeCode.UInt16:
				ilg.Emit(OpCodes.Stelem_I2);
				return;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				ilg.Emit(OpCodes.Stelem_I4);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				ilg.Emit(OpCodes.Stelem_I8);
				return;
			case TypeCode.Single:
				ilg.Emit(OpCodes.Stelem_R4);
				return;
			case TypeCode.Double:
				ilg.Emit(OpCodes.Stelem_R8);
				return;
			}
			ilg.Emit(OpCodes.Stelem, elementType);
		}

		public static void SyncFleeILGeneratorLabels(FleeILGenerator source, FleeILGenerator target)
		{
			while (source.LabelCount != target.LabelCount)
			{
				target.DefineLabel();
			}
		}

		public static bool IsIntegralType(Type t)
		{
			TypeCode tc = Type.GetTypeCode(t);
			return tc - TypeCode.SByte <= 7;
		}

		public static Type GetBitwiseOpType(Type leftType, Type rightType)
		{
			bool flag = !Utility.IsIntegralType(leftType) || !Utility.IsIntegralType(rightType);
			Type GetBitwiseOpType;
			if (flag)
			{
				GetBitwiseOpType = null;
			}
			else
			{
				GetBitwiseOpType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
			}
			return GetBitwiseOpType;
		}

		public static MethodInfo GetSimpleOverloadedOperator(string name, Type sourceType, Type destType)
		{
			Hashtable data = new Hashtable();
			data.Add("Name", "op_" + name);
			data.Add("sourceType", sourceType);
			data.Add("destType", destType);
			MemberInfo[] members = sourceType.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Public, new MemberFilter(Utility.SimpleOverloadedOperatorFilter), data);
			bool flag = members.Length == 0;
			if (flag)
			{
				members = destType.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Public, new MemberFilter(Utility.SimpleOverloadedOperatorFilter), data);
			}
			Debug.Assert(members.Length < 2, "Multiple overloaded operators found");
			bool flag2 = members.Length == 0;
			MethodInfo GetSimpleOverloadedOperator;
			if (flag2)
			{
				GetSimpleOverloadedOperator = null;
			}
			else
			{
				GetSimpleOverloadedOperator = (MethodInfo)members[0];
			}
			return GetSimpleOverloadedOperator;
		}

		private static bool SimpleOverloadedOperatorFilter(MemberInfo member, object value)
		{
			IDictionary data = (IDictionary)value;
			MethodInfo method = (MethodInfo)member;
			bool nameMatch = method.IsSpecialName && method.Name.Equals((string)data["Name"], StringComparison.OrdinalIgnoreCase);
			bool flag = !nameMatch;
			bool SimpleOverloadedOperatorFilter;
			if (flag)
			{
				SimpleOverloadedOperatorFilter = false;
			}
			else
			{
				bool returnTypeMatch = method.ReturnType == (Type)data["destType"];
				bool flag2 = !returnTypeMatch;
				if (flag2)
				{
					SimpleOverloadedOperatorFilter = false;
				}
				else
				{
					ParameterInfo[] parameters = method.GetParameters();
					bool argumentMatch = parameters.Length > 0 && parameters[0].ParameterType == (Type)data["sourceType"];
					SimpleOverloadedOperatorFilter = argumentMatch;
				}
			}
			return SimpleOverloadedOperatorFilter;
		}

		public static MethodInfo GetOverloadedOperator(string name, Type sourceType, Binder binder, params Type[] argumentTypes)
		{
			name = "op_" + name;
			MethodInfo mi = sourceType.GetMethod(name, BindingFlags.Static | BindingFlags.Public, binder, CallingConventions.Any, argumentTypes, null);
			bool flag = mi == null || !mi.IsSpecialName;
			MethodInfo GetOverloadedOperator;
			if (flag)
			{
				GetOverloadedOperator = null;
			}
			else
			{
				GetOverloadedOperator = mi;
			}
			return GetOverloadedOperator;
		}

		public static int GetILGeneratorLength(ILGenerator ilg)
		{
			FieldInfo fi = typeof(ILGenerator).GetField("m_length", BindingFlags.Instance | BindingFlags.NonPublic);
			return (int)fi.GetValue(ilg);
		}

		public static bool IsLongBranch(int startPosition, int endPosition)
		{
			return endPosition - startPosition > 127;
		}

		public static string FormatList(string[] items)
		{
			string separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
			return string.Join(separator, items);
		}

		public static string GetGeneralErrorMessage(string key, params object[] args)
		{
			string msg = FleeResourceManager.Instance.GetGeneralErrorString(key);
			return string.Format(msg, args);
		}

		public static string GetCompileErrorMessage(string key, params object[] args)
		{
			string msg = FleeResourceManager.Instance.GetCompileErrorString(key);
			return string.Format(msg, args);
		}
	}
}
