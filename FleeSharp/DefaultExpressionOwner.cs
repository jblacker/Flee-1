namespace Flee
{
    internal class DefaultExpressionOwner
    {
        private static readonly DefaultExpressionOwner ourInstance = new DefaultExpressionOwner();

        public static object Instance => ourInstance;

        private DefaultExpressionOwner()
        {
        }
    }
}