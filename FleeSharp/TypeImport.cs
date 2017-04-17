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

    public sealed class TypeImport : ImportBase
    {
        private readonly BindingFlags myBindFlags;

        private readonly bool myUseTypeNameAsNamespace;

        public TypeImport(Type importType)
            : this(importType, false)
        {
        }

        public TypeImport(Type importType, bool useTypeNameAsNamespace)
            : this(importType, BindingFlags.Static | BindingFlags.Public, useTypeNameAsNamespace)
        {
        }

        internal TypeImport(Type t, BindingFlags flags, bool useTypeNameAsNamespace)
        {
            Utility.AssertNotNull(t, "t");
            this.Target = t;
            this.myBindFlags = flags;
            this.myUseTypeNameAsNamespace = useTypeNameAsNamespace;
        }

        internal override void Validate()
        {
            this.Context.AssertTypeIsAccessible(this.Target);
        }

        protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            var members = this.Target.FindMembers(memberType, this.myBindFlags, this.Context.Options.MemberFilter, memberName);
            AddMemberRange(members, dest);
        }

        protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            var flag = !this.myUseTypeNameAsNamespace;
            if (flag)
            {
                var members = this.Target.FindMembers(memberType, this.myBindFlags, this.AlwaysMemberFilter, null);
                AddMemberRange(members, dest);
            }
        }

        internal override bool IsMatch(string name)
        {
            var myUseTypeNameAsNamespace1 = this.myUseTypeNameAsNamespace;
            return myUseTypeNameAsNamespace1 && string.Equals(this.Target.Name, name, this.Context.Options.MemberStringComparison);
        }

        internal override Type FindType(string typeName)
        {
            var flag = string.Equals(typeName, this.Target.Name, this.Context.Options.MemberStringComparison);
            var findType = flag ? this.Target : null;
            return findType;
        }

        protected override bool EqualsInternal(ImportBase import)
        {
            var otherSameType = import as TypeImport;
            return otherSameType != null && this.Target == otherSameType.Target;
        }

        public override IEnumerator<ImportBase> GetEnumerator()
        {
            var myUseTypeNameAsNamespace1 = this.myUseTypeNameAsNamespace;
            IEnumerator<ImportBase> getEnumerator;
            if (myUseTypeNameAsNamespace1)
            {
                getEnumerator = new List<ImportBase>
                {
                    new TypeImport(this.Target, false)
                }.GetEnumerator();
            }
            else
            {
                getEnumerator = base.GetEnumerator();
            }
            return getEnumerator;
        }

        public override bool IsContainer => this.myUseTypeNameAsNamespace;

        public override string Name => this.Target.Name;

        public Type Target { get; }
    }
}