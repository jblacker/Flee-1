using System;

namespace Ciloci.Flee
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class ExpressionOwnerMemberAccessAttribute : Attribute
	{
		private bool MyAllowAccess;

		internal bool AllowAccess
		{
			get
			{
				return this.MyAllowAccess;
			}
		}

		public ExpressionOwnerMemberAccessAttribute(bool allowAccess)
		{
			this.MyAllowAccess = allowAccess;
		}
	}
}
