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

    public sealed class MethodImport : ImportBase
    {
        public MethodImport(MethodInfo importMethod)
        {
            Utility.AssertNotNull(importMethod, "importMethod");
            this.Target = importMethod;
        }

        internal override void Validate()
        {
            this.Context.AssertTypeIsAccessible(this.Target.ReflectedType);
        }

        protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            var flag = string.Equals(memberName, this.Target.Name, this.Context.Options.MemberStringComparison) &&
                (memberType & MemberTypes.Method) > 0;
            if (flag)
            {
                dest.Add(this.Target);
            }
        }

        protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            var flag = (memberType & MemberTypes.Method) > 0;
            if (flag)
            {
                dest.Add(this.Target);
            }
        }

        internal override bool IsMatch(string name)
        {
            return string.Equals(this.Target.Name, name, this.Context.Options.MemberStringComparison);
        }

        internal override Type FindType(string typeName)
        {
            return null;
        }

        protected override bool EqualsInternal(ImportBase import)
        {
            var otherSameType = import as MethodImport;
            return otherSameType != null && this.Target.MethodHandle.Equals(otherSameType.Target.MethodHandle);
        }

        public override string Name
        {
            get { return this.Target.Name; }
        }

        public MethodInfo Target { get; }
    }
}