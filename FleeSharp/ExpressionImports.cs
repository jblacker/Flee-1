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

namespace FleeSharp
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    //using Microsoft.VisualBasic.CompilerServices;

    public sealed class ExpressionImports
    {
        private static readonly Dictionary<string, Type> ourBuiltinTypeMap = CreateBuiltinTypeMap();

        private ExpressionContext myContext;

        private TypeImport myOwnerImport;

        internal ExpressionImports()
        {
            this.RootImport = new NamespaceImport("true");
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
            this.RootImport.SetContext(context);
        }

        internal ExpressionImports Clone()
        {
            return new ExpressionImports
            {
                RootImport = (NamespaceImport) this.RootImport.Clone(),
                myOwnerImport = this.myOwnerImport
            };
        }

        internal void ImportOwner(Type ownerType)
        {
            this.myOwnerImport = new TypeImport(ownerType,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, false);
            this.myOwnerImport.SetContext(this.myContext);
        }

        internal bool HasNamespace(string ns)
        {
            var import = this.RootImport.FindImport(ns) as NamespaceImport;
            return import != null;
        }

        internal NamespaceImport GetImport(string ns)
        {
            var flag = ns.Length == 0;
            NamespaceImport getImport;
            if (flag)
            {
                getImport = this.RootImport;
            }
            else
            {
                var import = this.RootImport.FindImport(ns) as NamespaceImport;
                var flag2 = import == null;
                if (flag2)
                {
                    import = new NamespaceImport(ns);
                    this.RootImport.Add(import);
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
            ImportBase currentImport = this.RootImport;
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
                var findType = flag2 ? null : currentImport.FindType(typeName);
                return findType;
            }
        }

        internal static Type GetBuiltinType(string name)
        {
            Type t;
            var flag = ourBuiltinTypeMap.TryGetValue(name, out t);
            var getBuiltinType = flag ? t : null;
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
                var msg = Utility.GetGeneralErrorMessage("CouldNotFindPublicStaticMethodOnType", methodName, t.Name);
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
                var msg = Utility.GetGeneralErrorMessage("OnlyPublicStaticMethodsCanBeImported");
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

        public NamespaceImport RootImport { get; private set; }
    }
}