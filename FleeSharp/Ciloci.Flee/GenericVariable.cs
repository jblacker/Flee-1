using System;

namespace Ciloci.Flee
{
	internal class GenericVariable<T> : IVariable, IGenericVariable<T>
	{
		public T MyValue;

		public Type VariableType
		{
			get
			{
				return typeof(T);
			}
		}

		public object ValueAsObject
		{
			get
			{
				return this.MyValue;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.MyValue = default(T);
				}
				else
				{
					this.MyValue = (T)((object)value);
				}
			}
		}

		public IVariable Clone()
		{
			return new GenericVariable<T>
			{
				MyValue = this.MyValue
			};
		}

		public T GetValue()
		{
			return this.MyValue;
		}
	}
}
