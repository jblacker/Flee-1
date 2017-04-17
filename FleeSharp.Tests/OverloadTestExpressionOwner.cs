namespace FleeSharp.Tests
{
    using System;
    using System.IO;

    internal class OverloadTestExpressionOwner
    {
        public MemoryStream A;

        public object B;

        public int ValueType1(int arg)
        {
            return 1;
        }

        public int ValueType1(float arg)
        {
            return 2;
        }

        public int ValueType1(double arg)
        {
            return 3;
        }

        public int ValueType1(decimal arg)
        {
            return 4;
        }

        public int ValueType2(float arg)
        {
            return 1;
        }

        public int ValueType2(double arg)
        {
            return 2;
        }

        public int ValueType3(double arg)
        {
            return 1;
        }

        public int ValueType3(decimal arg)
        {
            return 2;
        }

        public int ReferenceType1(object arg)
        {
            return 1;
        }

        public int ReferenceType1(string arg)
        {
            return 2;
        }

        public int ReferenceType2(object arg)
        {
            return 1;
        }

        public int ReferenceType2(MemoryStream arg)
        {
            return 2;
        }

        public int ReferenceType3(object arg)
        {
            return 1;
        }

        public int ReferenceType3(IComparable arg)
        {
            return 2;
        }

        public int ReferenceType4(IFormattable arg)
        {
            return 1;
        }

        public int ReferenceType4(IComparable arg)
        {
            return 2;
        }

        public int Value_ReferenceType1(int arg)
        {
            return 1;
        }

        public int Value_ReferenceType1(object arg)
        {
            return 2;
        }

        public int Value_ReferenceType2(ValueType arg)
        {
            return 1;
        }

        public int Value_ReferenceType2(object arg)
        {
            return 2;
        }

        public int Value_ReferenceType3(IComparable arg)
        {
            return 1;
        }

        public int Value_ReferenceType3(object arg)
        {
            return 2;
        }

        public int Value_ReferenceType4(IComparable arg)
        {
            return 1;
        }

        public int Value_ReferenceType4(IFormattable arg)
        {
            return 2;
        }

        public int Access1(object arg)
        {
            return 1;
        }

        [ExpressionOwnerMemberAccess(false)]
        public int Access1(string arg)
        {
            return 2;
        }

        [ExpressionOwnerMemberAccess(false)]
        public int Access2(object arg)
        {
            return 1;
        }

        [ExpressionOwnerMemberAccess(false)]
        public int Access2(string arg)
        {
            return 2;
        }

        public int Multiple1(float arg1, double arg2)
        {
            return 1;
        }

        public int Multiple1(int arg1, double arg2)
        {
            return 2;
        }
    }
}