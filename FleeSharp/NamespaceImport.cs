using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flee
{
    public sealed class NamespaceImport : ImportBase, ICollection<ImportBase>
    {
        private string MyNamespace;

        private List<ImportBase> MyImports;

        private ICollection<ImportBase> NonContainerImports
        {
            get
            {
                List<ImportBase> found = new List<ImportBase>();
                try
                {
                    List<ImportBase>.Enumerator enumerator = this.MyImports.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ImportBase import = enumerator.Current;
                        bool flag = !import.IsContainer;
                        if (flag)
                        {
                            found.Add(import);
                        }
                    }
                }
                finally
                {
                    List<ImportBase>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                return found;
            }
        }

        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public override string Name
        {
            get
            {
                return this.MyNamespace;
            }
        }

        public int Count
        {
            get
            {
                return this.MyImports.Count;
            }
        }

        bool ICollection<ImportBase>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public NamespaceImport(string importNamespace)
        {
            Utility.AssertNotNull(importNamespace, "importNamespace");
            bool flag = importNamespace.Length == 0;
            if (flag)
            {
                string msg = Utility.GetGeneralErrorMessage("InvalidNamespaceName", new object[0]);
                throw new ArgumentException(msg);
            }
            this.MyNamespace = importNamespace;
            this.MyImports = new List<ImportBase>();
        }

        internal override void SetContext(ExpressionContext context)
        {
            base.SetContext(context);
            try
            {
                List<ImportBase>.Enumerator enumerator = this.MyImports.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ImportBase import = enumerator.Current;
                    import.SetContext(context);
                }
            }
            finally
            {
                List<ImportBase>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
        }

        internal override void Validate()
        {
        }

        protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
        {
            try
            {
                IEnumerator<ImportBase> enumerator = this.NonContainerImports.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ImportBase import = enumerator.Current;
                    AddImportMembers(import, memberName, memberType, dest);
                }
            }
            finally
            {
                IEnumerator<ImportBase> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
        }

        protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
        {
        }

        internal override Type FindType(string typeName)
        {
            Type FindType;
            try
            {
                IEnumerator<ImportBase> enumerator = this.NonContainerImports.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ImportBase import = enumerator.Current;
                    Type t = import.FindType(typeName);
                    bool flag = t != null;
                    if (flag)
                    {
                        FindType = t;
                        return FindType;
                    }
                }
            }
            finally
            {
                IEnumerator<ImportBase> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            FindType = null;
            return FindType;
        }

        internal override ImportBase FindImport(string name)
        {
            ImportBase FindImport;
            try
            {
                List<ImportBase>.Enumerator enumerator = this.MyImports.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ImportBase import = enumerator.Current;
                    bool flag = import.IsMatch(name);
                    if (flag)
                    {
                        FindImport = import;
                        return FindImport;
                    }
                }
            }
            finally
            {
                List<ImportBase>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            FindImport = null;
            return FindImport;
        }

        internal override bool IsMatch(string name)
        {
            return string.Equals(this.MyNamespace, name, this.Context.Options.MemberStringComparison);
        }

        protected override bool EqualsInternal(ImportBase import)
        {
            NamespaceImport otherSameType = import as NamespaceImport;
            return otherSameType != null && this.MyNamespace.Equals(otherSameType.MyNamespace, this.Context.Options.MemberStringComparison);
        }

        public void Add(ImportBase item)
        {
            Utility.AssertNotNull(item, "item");
            bool flag = this.Context != null;
            if (flag)
            {
                item.SetContext(this.Context);
            }
            this.MyImports.Add(item);
        }

        public void Clear()
        {
            this.MyImports.Clear();
        }

        public bool Contains(ImportBase item)
        {
            return this.MyImports.Contains(item);
        }

        public void CopyTo(ImportBase[] array, int arrayIndex)
        {
            this.MyImports.CopyTo(array, arrayIndex);
        }

        public bool Remove(ImportBase item)
        {
            return this.MyImports.Remove(item);
        }

        public override IEnumerator<ImportBase> GetEnumerator()
        {
            return (IEnumerator<ImportBase>)this.MyImports.GetEnumerator();
        }
    }
}