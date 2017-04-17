using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flee
{
    public sealed class TypeImport : ImportBase
    {
        private readonly Type myType;

        private readonly BindingFlags myBindFlags;

        private readonly bool myUseTypeNameAsNamespace;

        public override bool IsContainer => this.myUseTypeNameAsNamespace;

        public override string Name => this.myType.Name;

        public Type Target => this.myType;

        public TypeImport(Type importType) : this(importType, false)
        {
        }

        public TypeImport(Type importType, bool useTypeNameAsNamespace) : this(importType, BindingFlags.Static | BindingFlags.Public, useTypeNameAsNamespace)
        {
        }

        internal TypeImport(Type t, BindingFlags flags, bool useTypeNameAsNamespace)
        {
            Utility.AssertNotNull(t, "t");
            this.myType = t;
            this.myBindFlags = flags;
            this.myUseTypeNameAsNamespace = useTypeNameAsNamespace;
        }

        internal override void Validate()
        {
            this.Context.AssertTypeIsAccessible(this.myType);
        }

        protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            var members = this.myType.FindMembers(memberType, this.myBindFlags, this.Context.Options.MemberFilter, memberName);
            AddMemberRange(members, dest);
        }

        protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            bool flag = !this.myUseTypeNameAsNamespace;
            if (flag)
            {
                var members = this.myType.FindMembers(memberType, this.myBindFlags, this.AlwaysMemberFilter, null);
                AddMemberRange(members, dest);
            }
        }

        internal override bool IsMatch(string name)
        {
            bool myUseTypeNameAsNamespace1 = this.myUseTypeNameAsNamespace;
            return myUseTypeNameAsNamespace1 && string.Equals(this.myType.Name, name, this.Context.Options.MemberStringComparison);
        }

        internal override Type FindType(string typeName)
        {
            bool flag = string.Equals(typeName, this.myType.Name, this.Context.Options.MemberStringComparison);
            var findType = flag ? this.myType : null;
            return findType;
        }

        protected override bool EqualsInternal(ImportBase import)
        {
            var otherSameType = import as TypeImport;
            return otherSameType != null && this.myType == otherSameType.myType;
        }

        public override IEnumerator<ImportBase> GetEnumerator()
        {
            bool myUseTypeNameAsNamespace1 = this.myUseTypeNameAsNamespace;
            IEnumerator<ImportBase> getEnumerator;
            if (myUseTypeNameAsNamespace1)
            {
                getEnumerator = new List<ImportBase>
                {
                    new TypeImport(this.myType, false)
                }.GetEnumerator();
            }
            else
            {
                getEnumerator = base.GetEnumerator();
            }
            return getEnumerator;
        }
    }
}