using System;
using System.Collections;
using System.Collections.Generic;

namespace Ciloci.Flee
{
	internal class ArgumentList
	{
		private IList<ExpressionElement> MyElements;

		public ExpressionElement this[int index]
		{
			get
			{
				return this.MyElements[index];
			}
		}

		public int Count
		{
			get
			{
				return this.MyElements.Count;
			}
		}

		public ArgumentList(ICollection elements)
		{
			ExpressionElement[] arr = new ExpressionElement[elements.Count - 1 + 1];
			elements.CopyTo(arr, 0);
			this.MyElements = arr;
		}

		private string[] GetArgumentTypeNames()
		{
			List<string> i = new List<string>();
			try
			{
				IEnumerator<ExpressionElement> enumerator = this.MyElements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ExpressionElement e = enumerator.Current;
					i.Add(e.ResultType.Name);
				}
			}
			finally
			{
				IEnumerator<ExpressionElement> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return i.ToArray();
		}

		public Type[] GetArgumentTypes()
		{
			List<Type> i = new List<Type>();
			try
			{
				IEnumerator<ExpressionElement> enumerator = this.MyElements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ExpressionElement e = enumerator.Current;
					i.Add(e.ResultType);
				}
			}
			finally
			{
				IEnumerator<ExpressionElement> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return i.ToArray();
		}

		public override string ToString()
		{
			string[] typeNames = this.GetArgumentTypeNames();
			return Utility.FormatList(typeNames);
		}

		public ExpressionElement[] ToArray()
		{
			ExpressionElement[] arr = new ExpressionElement[this.MyElements.Count - 1 + 1];
			this.MyElements.CopyTo(arr, 0);
			return arr;
		}
	}
}
