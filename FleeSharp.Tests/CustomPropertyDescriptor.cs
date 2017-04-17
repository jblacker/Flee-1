namespace FleeSharp.Tests
{
    using System;
    using System.ComponentModel;

    internal class CustomPropertyDescriptor : PropertyDescriptor
    {
        public override Type ComponentType
        {
            get
            {
                return typeof(int);
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                bool IsReadOnly = false;
                return IsReadOnly;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(string);
            }
        }

        public CustomPropertyDescriptor() : base("Name", null)
        {
        }

        public override bool CanResetValue(object component)
        {
            bool CanResetValue = false;
            return CanResetValue;
        }

        public override object GetValue(object component)
        {
            return "prop!";
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            bool ShouldSerializeValue = false;
            return ShouldSerializeValue;
        }
    }
}