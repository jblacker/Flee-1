using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Flee
{
    public abstract class ImportBase : IEnumerable<ImportBase>, IEquatable<ImportBase>
    {
        private ExpressionContext myContext;

        protected ExpressionContext Context => this.myContext;

        public abstract string Name
        {
            get;
        }

        public virtual bool IsContainer => false;

        internal ImportBase()
        {
        }

        internal virtual void SetContext(ExpressionContext context)
        {
            this.myContext = context;
            this.Validate();
        }

        internal abstract void Validate();

        protected abstract void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest);

        protected abstract void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest);

        internal ImportBase Clone()
        {
            return (ImportBase)this.MemberwiseClone();
        }

        protected static void AddImportMembers(ImportBase import, string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
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

        public virtual IEnumerator<ImportBase> GetEnumerator()
        {
            var coll = new List<ImportBase>();
            return coll.GetEnumerator();
        }

        public bool Equals(ImportBase other)
        {
            return this.EqualsInternal(other);
        }

        protected abstract bool EqualsInternal(ImportBase import);

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}