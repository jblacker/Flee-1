namespace FleeSharp.Tests
{
    public class NestedA
    {
        public class NestedPublicB
        {
            public static int DoStuff()
            {
                return 100;
            }
        }

        internal class NestedInternalB
        {
            public static int DoStuff()
            {
                return 100;
            }
        }
    }
}