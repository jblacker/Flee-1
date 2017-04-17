namespace FleeSharp.Tests
{
    internal class AccessTestExpressionOwner
    {
        private int PrivateField1;

        [ExpressionOwnerMemberAccess(true)]
        private int PrivateField2;

        [ExpressionOwnerMemberAccess(false)]
        private int PrivateField3;

        public int PublicField1;
    }
}