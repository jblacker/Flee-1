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
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class ImportBase : IEnumerable<ImportBase>, IEquatable<ImportBase>
    {
        internal ImportBase()
        {
        }

        public bool Equals(ImportBase other)
        {
            return this.EqualsInternal(other);
        }

        public virtual IEnumerator<ImportBase> GetEnumerator()
        {
            var coll = new List<ImportBase>();
            return coll.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal virtual void SetContext(ExpressionContext context)
        {
            this.Context = context;
            this.Validate();
        }

        internal abstract void Validate();

        protected abstract void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest);

        protected abstract void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest);

        internal ImportBase Clone()
        {
            return (ImportBase) this.MemberwiseClone();
        }

        protected static void AddImportMembers(ImportBase import, string memberName, MemberTypes memberType,
            ICollection<MemberInfo> dest)
        {
            import.AddMembers(memberName, memberType, dest);
        }

        protected static void AddImportMembers(ImportBase import, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            import.AddMembers(memberType, dest);
        }

        protected static void AddMemberRange(ICollection<MemberInfo> members, ICollection<MemberInfo> dest)
        {
            using (var enumerator = members.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var mi = enumerator.Current;
                    dest.Add(mi);
                }
            }
        }

        protected bool AlwaysMemberFilter(MemberInfo member, object criteria)
        {
            return true;
        }

        internal abstract bool IsMatch(string name);

        internal abstract Type FindType(string typename);

        internal virtual ImportBase FindImport(string name)
        {
            return null;
        }

        internal MemberInfo[] FindMembers(string memberName, MemberTypes memberType)
        {
            var found = new List<MemberInfo>();
            this.AddMembers(memberName, memberType, found);
            return found.ToArray();
        }

        public MemberInfo[] GetMembers(MemberTypes memberType)
        {
            var found = new List<MemberInfo>();
            this.AddMembers(memberType, found);
            return found.ToArray();
        }

        protected abstract bool EqualsInternal(ImportBase import);

        protected ExpressionContext Context { get; private set; }

        public virtual bool IsContainer => false;

        public abstract string Name { get; }
    }
}