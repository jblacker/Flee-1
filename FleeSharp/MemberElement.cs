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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    internal abstract class MemberElement : ExpressionElement
    {
        public const BindingFlags BindFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        protected ExpressionContext myContext;

        protected ImportBase myImport;
        protected string myName;

        protected MemberElement myNext;

        protected ExpressionOptions myOptions;

        protected MemberElement myPrevious;

        protected IServiceProvider myServices;

        public void Link(MemberElement nextElement)
        {
            this.myNext = nextElement;
            var flag = nextElement != null;
            if (flag)
            {
                nextElement.myPrevious = this;
            }
        }

        public void Resolve(IServiceProvider services)
        {
            this.myServices = services;
            this.myOptions = (ExpressionOptions) services.GetService(typeof(ExpressionOptions));
            this.myContext = (ExpressionContext) services.GetService(typeof(ExpressionContext));
            this.ResolveInternal();
            this.Validate();
        }

        public void SetImport(ImportBase import)
        {
            this.myImport = import;
        }

        protected abstract void ResolveInternal();

        protected virtual void Validate()
        {
            var flag = this.myPrevious == null;
            if (!flag)
            {
                var flag2 = this.IsStatic && !this.SupportsStatic;
                if (flag2)
                {
                    this.ThrowCompileException("StaticMemberCannotBeAccessedWithInstanceReference", CompileExceptionReason.TypeMismatch,
                        this.myName);
                }
                else
                {
                    var flag3 = !this.IsStatic && !this.SupportsInstance;
                    if (flag3)
                    {
                        this.ThrowCompileException("ReferenceToNonSharedMemberRequiresObjectReference",
                            CompileExceptionReason.TypeMismatch, this.myName);
                    }
                }
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            var flag = this.myPrevious != null;
            if (flag)
            {
                this.myPrevious.Emit(ilg, services);
            }
        }

        protected static void EmitLoadVariables(FleeIlGenerator ilg)
        {
            ilg.Emit(OpCodes.Ldarg_2);
        }

        protected void EmitMethodCall(MethodInfo mi, FleeIlGenerator ilg)
        {
            EmitMethodCall(this.ResultType, this.NextRequiresAddress, mi, ilg);
        }

        protected static void EmitMethodCall(Type resultType, bool nextRequiresAddress, MethodInfo mi, FleeIlGenerator ilg)
        {
            var flag = mi.ReflectedType != null && !mi.ReflectedType.IsValueType;
            if (flag)
            {
                EmitReferenceTypeMethodCall(mi, ilg);
            }
            else
            {
                EmitValueTypeMethodCall(mi, ilg);
            }
            var flag2 = resultType.IsValueType & nextRequiresAddress;
            if (flag2)
            {
                EmitValueTypeLoadAddress(ilg, resultType);
            }
        }

        protected static bool IsGetTypeMethod(MethodInfo mi)
        {
            var miGetType = typeof(object).GetMethod("gettype", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            return mi.MethodHandle.Equals(miGetType.MethodHandle);
        }

        private static void EmitValueTypeMethodCall(MethodInfo mi, FleeIlGenerator ilg)
        {
            var isStatic = mi.IsStatic;
            if (isStatic)
            {
                ilg.Emit(OpCodes.Call, mi);
            }
            else
            {
                var flag = mi.DeclaringType != mi.ReflectedType;
                if (flag)
                {
                    var flag2 = IsGetTypeMethod(mi);
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

        private static void EmitReferenceTypeMethodCall(MethodInfo mi, FleeIlGenerator ilg)
        {
            var isStatic = mi.IsStatic;
            ilg.Emit(isStatic ? OpCodes.Call : OpCodes.Callvirt, mi);
        }

        protected static void EmitValueTypeLoadAddress(FleeIlGenerator ilg, Type targetType)
        {
            var index = ilg.GetTempLocalIndex(targetType);
            Utility.EmitStoreLocal(ilg, index);
            ilg.Emit(OpCodes.Ldloca_S, (byte) index);
        }

        protected void EmitLoadOwner(FleeIlGenerator ilg)
        {
            ilg.Emit(OpCodes.Ldarg_0);
            var ownerType = this.myOptions.OwnerType;
            var flag = !ownerType.IsValueType;
            if (!flag)
            {
                ilg.Emit(OpCodes.Unbox, ownerType);
                ilg.Emit(OpCodes.Ldobj, ownerType);
                var requiresAddress = this.RequiresAddress;
                if (requiresAddress)
                {
                    EmitValueTypeLoadAddress(ilg, ownerType);
                }
            }
        }

        private static bool IsMemberPublic(MemberInfo member)
        {
            var fi = member as FieldInfo;
            var flag = fi != null;
            var isMemberPublic = false;
            if (flag)
            {
                isMemberPublic = fi.IsPublic;
            }
            else
            {
                var pi = member as PropertyInfo;
                var flag2 = pi != null;
                if (flag2)
                {
                    var pmi = pi.GetGetMethod(true);
                    isMemberPublic = pmi.IsPublic;
                }
                else
                {
                    var mi = member as MethodInfo;
                    var flag3 = mi != null;
                    if (flag3)
                    {
                        isMemberPublic = mi.IsPublic;
                    }
                    else
                    {
                        Debug.Assert(false, "unknown member type");
                    }
                }
            }
            return isMemberPublic;
        }

        protected MemberInfo[] GetAccessibleMembers(MemberInfo[] members)
        {
            var accessible = new List<MemberInfo>();
            checked
            {
                for (var i = 0; i < members.Length; i++)
                {
                    var mi = members[i];
                    var flag = this.IsMemberAccessible(mi);
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
            var flag = IsMemberPublic(member);
            bool accessAllowed;
            if (flag)
            {
                accessAllowed = (options.OwnerMemberAccess & BindingFlags.Public) > BindingFlags.Default;
            }
            else
            {
                accessAllowed = (options.OwnerMemberAccess & BindingFlags.NonPublic) > BindingFlags.Default;
            }
            var attr =
                (ExpressionOwnerMemberAccessAttribute)
                Attribute.GetCustomAttribute(member, typeof(ExpressionOwnerMemberAccessAttribute));
            var flag2 = attr == null;
            var isOwnerMemberAccessible = flag2 ? accessAllowed : attr.AllowAccess;
            return isOwnerMemberAccessible;
        }

        public bool IsMemberAccessible(MemberInfo member)
        {
            var flag = this.myOptions.IsOwnerType(member.ReflectedType);
            var isMemberAccessible = flag ? IsOwnerMemberAccessible(member, this.myOptions) : IsMemberPublic(member);
            return isMemberAccessible;
        }

        protected MemberInfo[] GetMembers(MemberTypes targets)
        {
            var flag = this.myPrevious == null;
            MemberInfo[] getMembers;
            if (flag)
            {
                var flag2 = this.myImport == null;
                getMembers = flag2
                    ? this.GetDefaultNamespaceMembers(this.myName, targets) : this.myImport.FindMembers(this.myName, targets);
            }
            else
            {
                getMembers = this.myPrevious.TargetType.FindMembers(targets,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    this.myOptions.MemberFilter, this.myName);
            }
            return getMembers;
        }

        protected MemberInfo[] GetDefaultNamespaceMembers(string name, MemberTypes memberType)
        {
            var members = this.myContext.Imports.FindOwnerMembers(name, memberType);
            members = this.GetAccessibleMembers(members);
            var flag = members.Length > 0;
            var getDefaultNamespaceMembers = flag ? members : this.myContext.Imports.RootImport.FindMembers(name, memberType);
            return getDefaultNamespaceMembers;
        }

        protected static bool IsElementPublic(MemberElement e)
        {
            return e.IsPublic;
        }

        protected abstract bool IsPublic { get; }

        public abstract bool IsStatic { get; }

        public string MemberName => this.myName;

        protected bool NextRequiresAddress
        {
            get
            {
                var flag = this.myNext == null;
                return !flag && this.myNext.RequiresAddress;
            }
        }

        protected virtual bool RequiresAddress => false;

        protected virtual bool SupportsInstance => true;

        protected virtual bool SupportsStatic => false;

        public Type TargetType => this.ResultType;
    }
}