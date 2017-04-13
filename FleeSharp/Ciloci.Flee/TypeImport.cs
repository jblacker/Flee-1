using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ciloci.Flee
{
	public sealed class TypeImport : ImportBase
	{
		private Type MyType;

		private BindingFlags MyBindFlags;

		private bool MyUseTypeNameAsNamespace;

		public override bool IsContainer
		{
			get
			{
				return this.MyUseTypeNameAsNamespace;
			}
		}

		public override string Name
		{
			get
			{
				return this.MyType.Name;
			}
		}

		public Type Target
		{
			get
			{
				return this.MyType;
			}
		}

		public TypeImport(Type importType) : this(importType, false)
		{
		}

		public TypeImport(Type importType, bool useTypeNameAsNamespace) : this(importType, BindingFlags.Static | BindingFlags.Public, useTypeNameAsNamespace)
		{
		}

		internal TypeImport(Type t, BindingFlags flags, bool useTypeNameAsNamespace)
		{
			Utility.AssertNotNull(t, "t");
			this.MyType = t;
			this.MyBindFlags = flags;
			this.MyUseTypeNameAsNamespace = useTypeNameAsNamespace;
		}

		internal override void Validate()
		{
			base.Context.AssertTypeIsAccessible(this.MyType);
		}

		protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
		{
			MemberInfo[] members = this.MyType.FindMembers(memberType, this.MyBindFlags, base.Context.Options.MemberFilter, memberName);
			ImportBase.AddMemberRange(members, dest);
		}

		protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
		{
			bool flag = !this.MyUseTypeNameAsNamespace;
			if (flag)
			{
				MemberInfo[] members = this.MyType.FindMembers(memberType, this.MyBindFlags, new MemberFilter(base.AlwaysMemberFilter), null);
				ImportBase.AddMemberRange(members, dest);
			}
		}

		internal override bool IsMatch(string name)
		{
			bool myUseTypeNameAsNamespace = this.MyUseTypeNameAsNamespace;
			return myUseTypeNameAsNamespace && string.Equals(this.MyType.Name, name, base.Context.Options.MemberStringComparison);
		}

		internal override Type FindType(string typeName)
		{
			bool flag = string.Equals(typeName, this.MyType.Name, base.Context.Options.MemberStringComparison);
			Type FindType;
			if (flag)
			{
				FindType = this.MyType;
			}
			else
			{
				FindType = null;
			}
			return FindType;
		}

		protected override bool EqualsInternal(ImportBase import)
		{
			TypeImport otherSameType = import as TypeImport;
			return otherSameType != null && this.MyType == otherSameType.MyType;
		}

		public override IEnumerator<ImportBase> GetEnumerator()
		{
			bool myUseTypeNameAsNamespace = this.MyUseTypeNameAsNamespace;
			IEnumerator<ImportBase> GetEnumerator;
			if (myUseTypeNameAsNamespace)
			{
				GetEnumerator = (IEnumerator<ImportBase>)new List<ImportBase>
				{
					new TypeImport(this.MyType, false)
				}.GetEnumerator();
			}
			else
			{
				GetEnumerator = base.GetEnumerator();
			}
			return GetEnumerator;
		}
	}
}
