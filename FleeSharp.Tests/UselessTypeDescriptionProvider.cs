namespace FleeSharp.Tests
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal sealed class UselessTypeDescriptionProvider : TypeDescriptionProvider
    {
        internal UselessTypeDescriptionProvider(TypeDescriptionProvider parent) : base(parent)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new UselessCustomTypeDescriptor(base.GetTypeDescriptor(objectType, RuntimeHelpers.GetObjectValue(instance)));
        }
    }
}