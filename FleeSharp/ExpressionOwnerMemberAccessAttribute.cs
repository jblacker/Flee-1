using System;

namespace Flee
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class ExpressionOwnerMemberAccessAttribute : Attribute
    {
        internal bool AllowAccess { get; set; }

        public ExpressionOwnerMemberAccessAttribute(bool allowAccess)
        {
            this.AllowAccess = allowAccess;
        }
    }
}