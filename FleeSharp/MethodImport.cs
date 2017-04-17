using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flee
{
    public sealed class MethodImport : ImportBase
    {
        private MethodInfo MyMethod;

        public override string Name
        {
            get
            {
                return this.MyMethod.Name;
            }
        }

        public MethodInfo Target
        {
            get
            {
                return this.MyMethod;
            }
        }

        public MethodImport(MethodInfo importMethod)
        {
            Utility.AssertNotNull(importMethod, "importMethod");
            this.MyMethod = importMethod;
        }

        internal override void Validate()
        {
            this.Context.AssertTypeIsAccessible(this.MyMethod.ReflectedType);
        }

        protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            bool flag = string.Equals(memberName, this.MyMethod.Name, this.Context.Options.MemberStringComparison) && (memberType & MemberTypes.Method) > (MemberTypes)0;
            if (flag)
            {
                dest.Add(this.MyMethod);
            }
        }

        protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            bool flag = (memberType & MemberTypes.Method) > (MemberTypes)0;
            if (flag)
            {
                dest.Add(this.MyMethod);
            }
        }

        internal override bool IsMatch(string name)
        {
            return string.Equals(this.MyMethod.Name, name, this.Context.Options.MemberStringComparison);
        }

        internal override Type FindType(string typeName)
        {
            return null;
        }

        protected override bool EqualsInternal(ImportBase import)
        {
            MethodImport otherSameType = import as MethodImport;
            return otherSameType != null && this.MyMethod.MethodHandle.Equals(otherSameType.MyMethod.MethodHandle);
        }
    }
}