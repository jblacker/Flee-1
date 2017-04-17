using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Flee
{
    internal abstract class MemberElement : ExpressionElement
    {
        protected string MyName;

        protected MemberElement MyPrevious;

        protected MemberElement MyNext;

        protected IServiceProvider MyServices;

        protected ExpressionOptions MyOptions;

        protected ExpressionContext MyContext;

        protected ImportBase MyImport;

        public const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public abstract bool IsStatic
        {
            get;
        }

        protected abstract bool IsPublic
        {
            get;
        }

        public string MemberName
        {
            get
            {
                return this.MyName;
            }
        }

        protected bool NextRequiresAddress
        {
            get
            {
                bool flag = this.MyNext == null;
                return !flag && this.MyNext.RequiresAddress;
            }
        }

        protected virtual bool RequiresAddress
        {
            get
            {
                return false;
            }
        }

        protected virtual bool SupportsInstance
        {
            get
            {
                return true;
            }
        }

        protected virtual bool SupportsStatic
        {
            get
            {
                return false;
            }
        }

        public Type TargetType
        {
            get
            {
                return this.ResultType;
            }
        }

        public void Link(MemberElement nextElement)
        {
            this.MyNext = nextElement;
            bool flag = nextElement != null;
            if (flag)
            {
                nextElement.MyPrevious = this;
            }
        }

        public void Resolve(IServiceProvider services)
        {
            this.MyServices = services;
            this.MyOptions = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
            this.MyContext = (ExpressionContext)services.GetService(typeof(ExpressionContext));
            this.ResolveInternal();
            this.Validate();
        }

        public void SetImport(ImportBase import)
        {
            this.MyImport = import;
        }

        protected abstract void ResolveInternal();

        protected virtual void Validate()
        {
            bool flag = this.MyPrevious == null;
            if (!flag)
            {
                bool flag2 = this.IsStatic && !this.SupportsStatic;
                if (flag2)
                {
                    this.ThrowCompileException("StaticMemberCannotBeAccessedWithInstanceReference", CompileExceptionReason.TypeMismatch, new object[]
                    {
                        this.MyName
                    });
                }
                else
                {
                    bool flag3 = !this.IsStatic && !this.SupportsInstance;
                    if (flag3)
                    {
                        this.ThrowCompileException("ReferenceToNonSharedMemberRequiresObjectReference", CompileExceptionReason.TypeMismatch, new object[]
                        {
                            this.MyName
                        });
                    }
                }
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            bool flag = this.MyPrevious != null;
            if (flag)
            {
                this.MyPrevious.Emit(ilg, services);
            }
        }

        protected static void EmitLoadVariables(FleeILGenerator ilg)
        {
            ilg.Emit(OpCodes.Ldarg_2);
        }

        protected void EmitMethodCall(MethodInfo mi, FleeILGenerator ilg)
        {
            EmitMethodCall(this.ResultType, this.NextRequiresAddress, mi, ilg);
        }

        protected static void EmitMethodCall(Type resultType, bool nextRequiresAddress, MethodInfo mi, FleeILGenerator ilg)
        {
            bool flag = !mi.ReflectedType.IsValueType;
            if (flag)
            {
                EmitReferenceTypeMethodCall(mi, ilg);
            }
            else
            {
                EmitValueTypeMethodCall(mi, ilg);
            }
            bool flag2 = resultType.IsValueType & nextRequiresAddress;
            if (flag2)
            {
                EmitValueTypeLoadAddress(ilg, resultType);
            }
        }

        protected static bool IsGetTypeMethod(MethodInfo mi)
        {
            MethodInfo miGetType = typeof(object).GetMethod("gettype", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            return mi.MethodHandle.Equals(miGetType.MethodHandle);
        }

        private static void EmitValueTypeMethodCall(MethodInfo mi, FleeILGenerator ilg)
        {
            bool isStatic = mi.IsStatic;
            if (isStatic)
            {
                ilg.Emit(OpCodes.Call, mi);
            }
            else
            {
                bool flag = mi.DeclaringType != mi.ReflectedType;
                if (flag)
                {
                    bool flag2 = IsGetTypeMethod(mi);
                    if (flag2)
                    {
                        ilg.Emit(OpCodes.Box, mi.ReflectedType);
                        ilg.Emit(OpCodes.Call, mi);
                    }
                    else
                    {
                        ilg.Emit(OpCodes.Constrained, mi.ReflectedType);
                        ilg.Emit(OpCodes.Callvirt, mi);
                    }
                }
                else
                {
                    ilg.Emit(OpCodes.Call, mi);
                }
            }
        }

        private static void EmitReferenceTypeMethodCall(MethodInfo mi, FleeILGenerator ilg)
        {
            bool isStatic = mi.IsStatic;
            if (isStatic)
            {
                ilg.Emit(OpCodes.Call, mi);
            }
            else
            {
                ilg.Emit(OpCodes.Callvirt, mi);
            }
        }

        protected static void EmitValueTypeLoadAddress(FleeILGenerator ilg, Type targetType)
        {
            int index = ilg.GetTempLocalIndex(targetType);
            Utility.EmitStoreLocal(ilg, index);
            ilg.Emit(OpCodes.Ldloca_S, (byte)index);
        }

        protected void EmitLoadOwner(FleeILGenerator ilg)
        {
            ilg.Emit(OpCodes.Ldarg_0);
            Type ownerType = this.MyOptions.OwnerType;
            bool flag = !ownerType.IsValueType;
            if (!flag)
            {
                ilg.Emit(OpCodes.Unbox, ownerType);
                ilg.Emit(OpCodes.Ldobj, ownerType);
                bool requiresAddress = this.RequiresAddress;
                if (requiresAddress)
                {
                    EmitValueTypeLoadAddress(ilg, ownerType);
                }
            }
        }

        private static bool IsMemberPublic(MemberInfo member)
        {
            FieldInfo fi = member as FieldInfo;
            bool flag = fi != null;
            bool IsMemberPublic;
            if (flag)
            {
                IsMemberPublic = fi.IsPublic;
            }
            else
            {
                PropertyInfo pi = member as PropertyInfo;
                bool flag2 = pi != null;
                if (flag2)
                {
                    MethodInfo pmi = pi.GetGetMethod(true);
                    IsMemberPublic = pmi.IsPublic;
                }
                else
                {
                    MethodInfo mi = member as MethodInfo;
                    bool flag3 = mi != null;
                    if (flag3)
                    {
                        IsMemberPublic = mi.IsPublic;
                    }
                    else
                    {
                        Debug.Assert(false, "unknown member type");
                        IsMemberPublic = false;
                    }
                }
            }
            return IsMemberPublic;
        }

        protected MemberInfo[] GetAccessibleMembers(MemberInfo[] members)
        {
            List<MemberInfo> accessible = new List<MemberInfo>();
            checked
            {
                for (int i = 0; i < members.Length; i++)
                {
                    MemberInfo mi = members[i];
                    bool flag = this.IsMemberAccessible(mi);
                    if (flag)
                    {
                        accessible.Add(mi);
                    }
                }
                return accessible.ToArray();
            }
        }

        protected static bool IsOwnerMemberAccessible(MemberInfo member, ExpressionOptions options)
        {
            bool flag = IsMemberPublic(member);
            bool accessAllowed;
            if (flag)
            {
                accessAllowed = ((options.OwnerMemberAccess & BindingFlags.Public) > BindingFlags.Default);
            }
            else
            {
                accessAllowed = ((options.OwnerMemberAccess & BindingFlags.NonPublic) > BindingFlags.Default);
            }
            ExpressionOwnerMemberAccessAttribute attr = (ExpressionOwnerMemberAccessAttribute)Attribute.GetCustomAttribute(member, typeof(ExpressionOwnerMemberAccessAttribute));
            bool flag2 = attr == null;
            bool IsOwnerMemberAccessible;
            if (flag2)
            {
                IsOwnerMemberAccessible = accessAllowed;
            }
            else
            {
                IsOwnerMemberAccessible = attr.AllowAccess;
            }
            return IsOwnerMemberAccessible;
        }

        public bool IsMemberAccessible(MemberInfo member)
        {
            bool flag = this.MyOptions.IsOwnerType(member.ReflectedType);
            bool IsMemberAccessible;
            if (flag)
            {
                IsMemberAccessible = IsOwnerMemberAccessible(member, this.MyOptions);
            }
            else
            {
                IsMemberAccessible = IsMemberPublic(member);
            }
            return IsMemberAccessible;
        }

        protected MemberInfo[] GetMembers(MemberTypes targets)
        {
            bool flag = this.MyPrevious == null;
            MemberInfo[] GetMembers;
            if (flag)
            {
                bool flag2 = this.MyImport == null;
                if (flag2)
                {
                    GetMembers = this.GetDefaultNamespaceMembers(this.MyName, targets);
                }
                else
                {
                    GetMembers = this.MyImport.FindMembers(this.MyName, targets);
                }
            }
            else
            {
                GetMembers = this.MyPrevious.TargetType.FindMembers(targets, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, this.MyOptions.MemberFilter, this.MyName);
            }
            return GetMembers;
        }

        protected MemberInfo[] GetDefaultNamespaceMembers(string name, MemberTypes memberType)
        {
            MemberInfo[] members = this.MyContext.Imports.FindOwnerMembers(name, memberType);
            members = this.GetAccessibleMembers(members);
            bool flag = members.Length > 0;
            MemberInfo[] GetDefaultNamespaceMembers;
            if (flag)
            {
                GetDefaultNamespaceMembers = members;
            }
            else
            {
                GetDefaultNamespaceMembers = this.MyContext.Imports.RootImport.FindMembers(name, memberType);
            }
            return GetDefaultNamespaceMembers;
        }

        protected static bool IsElementPublic(MemberElement e)
        {
            return e.IsPublic;
        }
    }
}