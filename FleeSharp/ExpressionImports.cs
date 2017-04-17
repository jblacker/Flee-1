using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    public sealed class ExpressionImports
    {
        private static readonly Dictionary<string, Type> ourBuiltinTypeMap = CreateBuiltinTypeMap();

        private NamespaceImport myRootImport;

        private TypeImport myOwnerImport;

        private ExpressionContext myContext;

        public NamespaceImport RootImport => this.myRootImport;

        internal ExpressionImports()
        {
            this.myRootImport = new NamespaceImport(Conversions.ToString(true));
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
            this.myContext = context;
            this.myRootImport.SetContext(context);
        }

        internal ExpressionImports Clone()
        {
            return new ExpressionImports
            {
                myRootImport = (NamespaceImport)this.myRootImport.Clone(),
                myOwnerImport = this.myOwnerImport
            };
        }

        internal void ImportOwner(Type ownerType)
        {
            this.myOwnerImport = new TypeImport(ownerType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false);
            this.myOwnerImport.SetContext(this.myContext);
        }

        internal bool HasNamespace(string ns)
        {
            var import = this.myRootImport.FindImport(ns) as NamespaceImport;
            return import != null;
        }

        internal NamespaceImport GetImport(string ns)
        {
            var flag = ns.Length == 0;
            NamespaceImport getImport;
            if (flag)
            {
                getImport = this.myRootImport;
            }
            else
            {
                var import = this.myRootImport.FindImport(ns) as NamespaceImport;
                var flag2 = import == null;
                if (flag2)
                {
                    import = new NamespaceImport(ns);
                    this.myRootImport.Add(import);
                }
                getImport = import;
            }
            return getImport;
        }

        internal MemberInfo[] FindOwnerMembers(string memberName, MemberTypes memberType)
        {
            return this.myOwnerImport.FindMembers(memberName, memberType);
        }

        internal Type FindType(string[] typeNameParts)
        {
            var namespaces = new string[typeNameParts.Length - 2 + 1];
            var typeName = typeNameParts[typeNameParts.Length - 1];
            Array.Copy(typeNameParts, namespaces, namespaces.Length);
            ImportBase currentImport = this.myRootImport;
            var array = namespaces;
            checked
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var ns = array[i];
                    currentImport = currentImport.FindImport(ns);
                    var flag = currentImport == null;
                    if (flag)
                    {
                        break;
                    }
                }
                var flag2 = currentImport == null;
                Type findType;
                if (flag2)
                {
                    findType = null;
                }
                else
                {
                    findType = currentImport.FindType(typeName);
                }
                return findType;
            }
        }

        internal static Type GetBuiltinType(string name)
        {
            Type t = null;
            var flag = ourBuiltinTypeMap.TryGetValue(name, out t);
            Type getBuiltinType;
            if (flag)
            {
                getBuiltinType = t;
            }
            else
            {
                getBuiltinType = null;
            }
            return getBuiltinType;
        }

        public void AddType(Type t, string ns)
        {
            Utility.AssertNotNull(t, "t");
            Utility.AssertNotNull(ns, "namespace");
            this.myContext.AssertTypeIsAccessible(t);
            var import = this.GetImport(ns);
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
            var mi = t.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
            var flag = mi == null;
            if (flag)
            {
                var msg = Utility.GetGeneralErrorMessage("CouldNotFindPublicStaticMethodOnType", new object[]
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
            this.myContext.AssertTypeIsAccessible(mi.ReflectedType);
            var flag = !mi.IsStatic | !mi.IsPublic;
            if (flag)
            {
                var msg = Utility.GetGeneralErrorMessage("OnlyPublicStaticMethodsCanBeImported", new object[0]);
                throw new ArgumentException(msg);
            }
            var import = this.GetImport(ns);
            import.Add(new MethodImport(mi));
        }

        public void ImportBuiltinTypes()
        {
            foreach (var kvp in ourBuiltinTypeMap)
            {
                this.AddType(kvp.Value, kvp.Key);
            }
        }
    }
}