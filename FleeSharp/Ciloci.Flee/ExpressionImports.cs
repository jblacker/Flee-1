using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ciloci.Flee
{
	public sealed class ExpressionImports
	{
		private static Dictionary<string, Type> OurBuiltinTypeMap = ExpressionImports.CreateBuiltinTypeMap();

		private NamespaceImport MyRootImport;

		private TypeImport MyOwnerImport;

		private ExpressionContext MyContext;

		public NamespaceImport RootImport
		{
			get
			{
				return this.MyRootImport;
			}
		}

		internal ExpressionImports()
		{
			this.MyRootImport = new NamespaceImport(Conversions.ToString(true));
		}

		private static Dictionary<string, Type> CreateBuiltinTypeMap()
		{
			return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"boolean",
					typeof(bool)
				},
				{
					"byte",
					typeof(byte)
				},
				{
					"sbyte",
					typeof(sbyte)
				},
				{
					"short",
					typeof(short)
				},
				{
					"ushort",
					typeof(ushort)
				},
				{
					"int",
					typeof(int)
				},
				{
					"uint",
					typeof(uint)
				},
				{
					"long",
					typeof(long)
				},
				{
					"ulong",
					typeof(ulong)
				},
				{
					"single",
					typeof(float)
				},
				{
					"double",
					typeof(double)
				},
				{
					"decimal",
					typeof(decimal)
				},
				{
					"char",
					typeof(char)
				},
				{
					"object",
					typeof(object)
				},
				{
					"string",
					typeof(string)
				}
			};
		}

		internal void SetContext(ExpressionContext context)
		{
			this.MyContext = context;
			this.MyRootImport.SetContext(context);
		}

		internal ExpressionImports Clone()
		{
			return new ExpressionImports
			{
				MyRootImport = (NamespaceImport)this.MyRootImport.Clone(),
				MyOwnerImport = this.MyOwnerImport
			};
		}

		internal void ImportOwner(Type ownerType)
		{
			this.MyOwnerImport = new TypeImport(ownerType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false);
			this.MyOwnerImport.SetContext(this.MyContext);
		}

		internal bool HasNamespace(string ns)
		{
			NamespaceImport import = this.MyRootImport.FindImport(ns) as NamespaceImport;
			return import != null;
		}

		internal NamespaceImport GetImport(string ns)
		{
			bool flag = ns.Length == 0;
			NamespaceImport GetImport;
			if (flag)
			{
				GetImport = this.MyRootImport;
			}
			else
			{
				NamespaceImport import = this.MyRootImport.FindImport(ns) as NamespaceImport;
				bool flag2 = import == null;
				if (flag2)
				{
					import = new NamespaceImport(ns);
					this.MyRootImport.Add(import);
				}
				GetImport = import;
			}
			return GetImport;
		}

		internal MemberInfo[] FindOwnerMembers(string memberName, MemberTypes memberType)
		{
			return this.MyOwnerImport.FindMembers(memberName, memberType);
		}

		internal Type FindType(string[] typeNameParts)
		{
			string[] namespaces = new string[typeNameParts.Length - 2 + 1];
			string typeName = typeNameParts[typeNameParts.Length - 1];
			Array.Copy(typeNameParts, namespaces, namespaces.Length);
			ImportBase currentImport = this.MyRootImport;
			string[] array = namespaces;
			checked
			{
				for (int i = 0; i < array.Length; i++)
				{
					string ns = array[i];
					currentImport = currentImport.FindImport(ns);
					bool flag = currentImport == null;
					if (flag)
					{
						break;
					}
				}
				bool flag2 = currentImport == null;
				Type FindType;
				if (flag2)
				{
					FindType = null;
				}
				else
				{
					FindType = currentImport.FindType(typeName);
				}
				return FindType;
			}
		}

		internal static Type GetBuiltinType(string name)
		{
			Type t = null;
			bool flag = ExpressionImports.OurBuiltinTypeMap.TryGetValue(name, out t);
			Type GetBuiltinType;
			if (flag)
			{
				GetBuiltinType = t;
			}
			else
			{
				GetBuiltinType = null;
			}
			return GetBuiltinType;
		}

		public void AddType(Type t, string ns)
		{
			Utility.AssertNotNull(t, "t");
			Utility.AssertNotNull(ns, "namespace");
			this.MyContext.AssertTypeIsAccessible(t);
			NamespaceImport import = this.GetImport(ns);
			import.Add(new TypeImport(t, BindingFlags.Static | BindingFlags.Public, false));
		}

		public void AddType(Type t)
		{
			this.AddType(t, string.Empty);
		}

		public void AddMethod(string methodName, Type t, string ns)
		{
			Utility.AssertNotNull(methodName, "methodName");
			Utility.AssertNotNull(t, "t");
			Utility.AssertNotNull(ns, "namespace");
			MethodInfo mi = t.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
			bool flag = mi == null;
			if (flag)
			{
				string msg = Utility.GetGeneralErrorMessage("CouldNotFindPublicStaticMethodOnType", new object[]
				{
					methodName,
					t.Name
				});
				throw new ArgumentException(msg);
			}
			this.AddMethod(mi, ns);
		}

		public void AddMethod(MethodInfo mi, string ns)
		{
			Utility.AssertNotNull(mi, "mi");
			Utility.AssertNotNull(ns, "namespace");
			this.MyContext.AssertTypeIsAccessible(mi.ReflectedType);
			bool flag = !mi.IsStatic | !mi.IsPublic;
			if (flag)
			{
				string msg = Utility.GetGeneralErrorMessage("OnlyPublicStaticMethodsCanBeImported", new object[0]);
				throw new ArgumentException(msg);
			}
			NamespaceImport import = this.GetImport(ns);
			import.Add(new MethodImport(mi));
		}

		public void ImportBuiltinTypes()
		{
			try
			{
				Dictionary<string, Type>.Enumerator enumerator = ExpressionImports.OurBuiltinTypeMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, Type> pair = enumerator.Current;
					this.AddType(pair.Value, pair.Key);
				}
			}
			finally
			{
				Dictionary<string, Type>.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
		}
	}
}
