using System;

namespace Flee
{
    internal class ExpressionMemberElement : MemberElement
    {
        private ExpressionElement MyElement;

        protected override bool SupportsInstance
        {
            get
            {
                return true;
            }
        }

        protected override bool IsPublic
        {
            get
            {
                return true;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return false;
            }
        }

        public override Type ResultType
        {
            get
            {
                return this.MyElement.ResultType;
            }
        }

        public ExpressionMemberElement(ExpressionElement element)
        {
            this.MyElement = element;
        }

        protected override void ResolveInternal()
        {
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            base.Emit(ilg, services);
            this.MyElement.Emit(ilg, services);
            bool isValueType = this.MyElement.ResultType.IsValueType;
            if (isValueType)
            {
                EmitValueTypeLoadAddress(ilg, this.ResultType);
            }
        }
    }
}