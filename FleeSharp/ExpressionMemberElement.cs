using System;

namespace Flee
{
    internal class ExpressionMemberElement : MemberElement
    {
        private readonly ExpressionElement myElement;

        protected override bool SupportsInstance => true;

        protected override bool IsPublic => true;

        public override bool IsStatic => false;

        public override Type ResultType => this.myElement.ResultType;

        public ExpressionMemberElement(ExpressionElement element)
        {
            this.myElement = element;
        }

        protected override void ResolveInternal()
        {
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            base.Emit(ilg, services);
            this.myElement.Emit(ilg, services);
            bool isValueType = this.myElement.ResultType.IsValueType;
            if (isValueType)
            {
                EmitValueTypeLoadAddress(ilg, this.ResultType);
            }
        }
    }
}