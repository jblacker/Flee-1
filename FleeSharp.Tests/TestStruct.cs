namespace FleeSharp.Tests
{
    using System;

    internal struct TestStruct : IComparable
    {
        private int MyA;

        public TestStruct(int a)
        {
            this = default(TestStruct);
            this.MyA = a;
        }

        public int DoStuff()
        {
            return 100;
        }

        public int CompareTo(object obj)
        {
            int CompareTo = 0;
            return CompareTo;
        }
    }
}