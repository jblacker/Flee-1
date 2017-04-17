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
    using System.Linq;
    using System.Reflection;
    using Extensions;

    public sealed class NamespaceImport : ImportBase, ICollection<ImportBase>
    {
        private readonly List<ImportBase> myImports;
        private readonly string myNamespace;

        public NamespaceImport(string importNamespace)
        {
            Utility.AssertNotNull(importNamespace, "importNamespace");
            var flag = importNamespace.Length == 0;
            if (flag)
            {
                var msg = Utility.GetGeneralErrorMessage("InvalidNamespaceName");
                throw new ArgumentException(msg);
            }
            this.myNamespace = importNamespace;
            this.myImports = new List<ImportBase>();
        }

        public void Add(ImportBase item)
        {
            Utility.AssertNotNull(item, "item");
            var flag = this.Context != null;
            if (flag)
            {
                item.SetContext(this.Context);
            }
            this.myImports.Add(item);
        }

        public void Clear()
        {
            this.myImports.Clear();
        }

        public bool Contains(ImportBase item)
        {
            return this.myImports.Contains(item);
        }

        public void CopyTo(ImportBase[] array, int arrayIndex)
        {
            this.myImports.CopyTo(array, arrayIndex);
        }

        public int Count => this.myImports.Count;

        public override IEnumerator<ImportBase> GetEnumerator()
        {
            return this.myImports.GetEnumerator();
        }

        bool ICollection<ImportBase>.IsReadOnly => false;

        public bool Remove(ImportBase item)
        {
            return this.myImports.Remove(item);
        }

        internal override void SetContext(ExpressionContext context)
        {
            base.SetContext(context);
            this.myImports.ForEach(c => c.SetContext(context));
        }

        internal override void Validate()
        {
        }

        protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            this.NonContainerImports.Each(c => AddImportMembers(c, memberName, memberType, dest));

            //try
            //{
            //    IEnumerator<ImportBase> enumerator = this.NonContainerImports.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        ImportBase import = enumerator.Current;
            //        AddImportMembers(import, memberName, memberType, dest);
            //    }
            //}
            //finally
            //{
            //    IEnumerator<ImportBase> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
        }

        protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
        {
        }

        internal override Type FindType(string typeName)
        {
            foreach (var import in this.NonContainerImports)
            {
                var t = import.FindType(typeName);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
            //try
            //{
            //    IEnumerator<ImportBase> enumerator = this.NonContainerImports.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        ImportBase import = enumerator.Current;
            //        Type t = import.FindType(typeName);
            //        bool flag = t != null;
            //        if (flag)
            //        {
            //            FindType = t;
            //            return FindType;
            //        }
            //    }
            //}
            //finally
            //{
            //    IEnumerator<ImportBase> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
            //FindType = null;
            //return null;
        }

        internal override ImportBase FindImport(string name)
        {
            foreach (var import in this.NonContainerImports)
            {
                if (import.IsMatch(name))
                {
                    return import;
                }
            }
            return null;
            //ImportBase FindImport;
            //try
            //{
            //    var enumerator = this.myImports.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var import = enumerator.Current;
            //        var flag = import.IsMatch(name);
            //        if (flag)
            //        {
            //            FindImport = import;
            //            return FindImport;
            //        }
            //    }
            //}
            //finally
            //{
            //    List<ImportBase>.Enumerator enumerator;
            //    ((IDisposable)enumerator).Dispose();
            //}
            //FindImport = null;
            //return FindImport;
        }

        internal override bool IsMatch(string name)
        {
            return string.Equals(this.myNamespace, name, this.Context.Options.MemberStringComparison);
        }

        protected override bool EqualsInternal(ImportBase import)
        {
            var otherSameType = import as NamespaceImport;
            return otherSameType != null &&
                this.myNamespace.Equals(otherSameType.myNamespace, this.Context.Options.MemberStringComparison);
        }

        public override bool IsContainer => true;

        public override string Name => this.myNamespace;

        private ICollection<ImportBase> NonContainerImports
        {
            get
            {
                return this.myImports.Where(x => !this.IsContainer).ToList();

                //var found = new List<ImportBase>();
                //try
                //{
                //    List<ImportBase>.Enumerator enumerator = this.myImports.GetEnumerator();
                //    while (enumerator.MoveNext())
                //    {
                //        ImportBase import = enumerator.Current;
                //        bool flag = !import.IsContainer;
                //        if (flag)
                //        {
                //            found.Add(import);
                //        }
                //    }
                //}
                //finally
                //{
                //    List<ImportBase>.Enumerator enumerator;
                //    ((IDisposable)enumerator).Dispose();
                //}
                //return found;
            }
        }
    }
}