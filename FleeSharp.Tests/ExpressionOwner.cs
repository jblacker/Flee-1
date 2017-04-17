namespace FleeSharp.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Text;

    public class ExpressionOwner
    {
        private readonly double DoubleA;

        private float SingleA;

        private readonly int Int32A;

        private string StringA;

        private bool BoolA;

        private Type TypeA;

        private byte ByteA;

        private byte ByteB;

        private sbyte SByteA;

        private short Int16A;

        private ushort UInt16A;

        private int[] IntArr;

        private string[] StringArr;

        private double[] DoubleArr;

        private bool[] BoolArr;

        private char[] CharArr;

        private DateTime[] DateTimeArr;

        private readonly IList List;

        private readonly StringDictionary StringDict;

        private Guid GuidA;

        private readonly DateTime DateTimeA;

        private ICloneable ICloneableA;

        private ICollection ICollectionA;

        private Version VersionA;

        private TestStruct StructA;

        private IComparable IComparableA;

        private object ObjectIntA;

        private object ObjectStringA;

        private ValueType ValueTypeStructA;

        private Exception ExceptionA;

        private Exception ExceptionNull;

        private IComparable IComparableString;

        private IComparable IComparableNull;

        private ICloneable ICloneableArray;

        private Delegate DelegateANull;

        private Array ArrayA;

        private AppDomainInitializer DelegateA;

        private ASCIIEncoding[] AsciiEncodingArr;

        private Encoding EncodingA;

        private Keyboard KeyboardA;

        private decimal DecimalA;

        private decimal DecimalB;

        private object NullField;

        private object InstanceA;

        private readonly ArrayList InstanceB;

        private readonly Hashtable Dict;

        private readonly Dictionary<string, int> GenericDict;

        private DataRow Row;

        public double DoubleAProp
        {
            get
            {
                return this.DoubleA;
            }
        }

        private int Int32AProp
        {
            get
            {
                return this.Int32A;
            }
        }

        internal static string SharedPropA
        {
            get
            {
                return "sharedprop";
            }
        }

        public ExpressionOwner()
        {
            this.IntArr = new int[]
            {
                100,
                200,
                300
            };
            this.StringArr = new string[]
            {
                "a",
                "b",
                "c"
            };
            this.DoubleArr = new double[]
            {
                1.1,
                2.2,
                3.3
            };
            this.BoolArr = new bool[]
            {
                true,
                default(bool),
                true
            };
            this.CharArr = new char[]
            {
                '.'
            };
            this.DateTimeArr = new DateTime[]
            {
                new DateTime(2007, 7, 1)
            };
            this.AsciiEncodingArr = new ASCIIEncoding[0];
            this.InstanceB = new ArrayList();
            this.InstanceA = this.InstanceB;
            this.NullField = null;
            this.DecimalA = new decimal(100L);
            this.DecimalB = 0.25m;
            this.KeyboardA = default(Keyboard);
            this.KeyboardA.StructA = new Mouse("mouse", 123);
            this.KeyboardA.ClassA = new Monitor();
            this.EncodingA = Encoding.ASCII;
            this.DelegateA = new AppDomainInitializer(this.DoAction);
            this.ICloneableArray = new string[0];
            this.ArrayA = new string[0];
            this.DelegateANull = null;
            this.IComparableNull = null;
            this.IComparableString = "string";
            this.ExceptionA = new ArgumentException();
            this.ExceptionNull = null;
            this.ValueTypeStructA = default(TestStruct);
            this.ObjectStringA = "string";
            this.ObjectIntA = 100;
            this.IComparableA = (IComparable)100.25;
            this.StructA = default(TestStruct);
            this.VersionA = new Version(1, 1, 1, 1);
            this.ICloneableA = "abc";
            this.GuidA = Guid.NewGuid();
            this.List = new ArrayList();
            this.List.Add("a");
            this.List.Add(100);
            this.StringDict = new StringDictionary();
            this.StringDict.Add("key", "value");
            this.DoubleA = 100.25;
            this.SingleA = 100.25f;
            this.Int32A = 100000;
            this.StringA = "string";
            this.BoolA = true;
            this.TypeA = typeof(string);
            this.ByteA = 50;
            this.ByteB = 2;
            this.SByteA = -10;
            this.Int16A = -10;
            this.UInt16A = 100;
            this.DateTimeA = new DateTime(2007, 7, 1);
            this.GenericDict = new Dictionary<string, int>();
            this.GenericDict.Add("a", 100);
            this.GenericDict.Add("b", 100);
            this.Dict = new Hashtable();
            this.Dict.Add(100, null);
            this.Dict.Add("abc", null);
            this.Row = new DataTable
            {
                Columns =
                {
                    {
                        "ColumnA",
                        typeof(int)
                    }
                },
                Rows =
                {
                    new object[]
                    {
                        100
                    }
                }
            }.Rows[0];
        }

        private void DoAction(string[] args)
        {
        }

        public void DoStuff()
        {
        }

        public int DoubleIt(int i)
        {
            return checked(i * 2);
        }

        public string FuncString()
        {
            return "abc";
        }

        public static int SharedFuncInt()
        {
            return 100;
        }

        private string PrivateFuncString()
        {
            return "abc";
        }

        public static int PrivateSharedFuncInt()
        {
            return 100;
        }

        public DateTime GetDateTime()
        {
            return this.DateTimeA;
        }

        public int ThrowException()
        {
            throw new InvalidOperationException("Should not be thrown!");
        }

        public ArrayList Func1(ArrayList al)
        {
            return al;
        }

        public string ReturnNullString()
        {
            return null;
        }

        public int Sum(int i)
        {
            return 1;
        }

        public int Sum(int i1, int i2)
        {
            return 2;
        }

        public int Sum(int i1, double i2)
        {
            return 3;
        }

        public int Sum(params int[] args)
        {
            return 4;
        }

        public int Sum2(int i1, double i2)
        {
            return 3;
        }

        public int Sum2(params int[] args)
        {
            return 4;
        }

        public int Sum4(params int[] args)
        {
            int sum = 0;
            checked
            {
                for (int j = 0; j < args.Length; j++)
                {
                    int i = args[j];
                    sum += i;
                }
                return sum;
            }
        }

        public int ParamArray1(string a, params object[] args)
        {
            return 1;
        }

        public int ParamArray2(params DateTime[] args)
        {
            return 1;
        }

        public int ParamArray3(params DateTime[] args)
        {
            return 1;
        }

        public int ParamArray3()
        {
            return 2;
        }

        public int ParamArray4(params int[] args)
        {
            return 1;
        }

        public int ParamArray4(params object[] args)
        {
            return 2;
        }
    }
}