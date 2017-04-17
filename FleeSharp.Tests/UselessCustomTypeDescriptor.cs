namespace FleeSharp.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;

    internal sealed class UselessCustomTypeDescriptor : CustomTypeDescriptor
    {
        internal UselessCustomTypeDescriptor(ICustomTypeDescriptor parent) : base(parent)
        {
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection originalProperties = base.GetProperties();
            List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();

            foreach (var pd in originalProperties)
            {
                newProperties.Add((PropertyDescriptor) pd);
            }
            //try
            //{
            //    IEnumerator enumerator = originalProperties.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        PropertyDescriptor pd = (PropertyDescriptor)enumerator.Current;
            //        newProperties.Add(pd);
            //    }
            //}
            //finally
            //{
            //    IEnumerator enumerator;
            //    if (enumerator is IDisposable)
            //    {
            //        (enumerator as IDisposable).Dispose();
            //    }
            //}
            newProperties.Add(new CustomPropertyDescriptor());
            return new PropertyDescriptorCollection(newProperties.ToArray(), true);
        }
    }
}