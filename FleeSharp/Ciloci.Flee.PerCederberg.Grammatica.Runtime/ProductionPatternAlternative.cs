using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class ProductionPatternAlternative
	{
		private ProductionPattern m_pattern;

		private ArrayList elements;

		private LookAheadSet m_lookAhead;

		public ProductionPattern Pattern
		{
			get
			{
				return this.m_pattern;
			}
		}

		internal LookAheadSet LookAhead
		{
			get
			{
				return this.m_lookAhead;
			}
			set
			{
				this.m_lookAhead = value;
			}
		}

		public int Count
		{
			get
			{
				return this.elements.Count;
			}
		}

		public ProductionPatternElement this[int index]
		{
			get
			{
				return (ProductionPatternElement)this.elements[index];
			}
		}

		public ProductionPatternAlternative()
		{
			this.elements = new ArrayList();
		}

		public ProductionPattern GetPattern()
		{
			return this.Pattern;
		}

		public int GetElementCount()
		{
			return this.Count;
		}

		public ProductionPatternElement GetElement(int pos)
		{
			return this[pos];
		}

		public bool IsLeftRecursive()
		{
			int num = this.elements.Count - 1;
			bool IsLeftRecursive;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternElement elem = (ProductionPatternElement)this.elements[i];
				bool flag = elem.Id == this.m_pattern.Id;
				if (flag)
				{
					IsLeftRecursive = true;
					return IsLeftRecursive;
				}
				bool flag2 = elem.MinCount > 0;
				if (flag2)
				{
					break;
				}
			}
			IsLeftRecursive = false;
			return IsLeftRecursive;
		}

		public bool IsRightRecursive()
		{
			int num = this.elements.Count - 1;
			bool IsRightRecursive;
			for (int i = num; i >= 0; i += -1)
			{
				ProductionPatternElement elem = (ProductionPatternElement)this.elements[i];
				bool flag = elem.Id == this.m_pattern.Id;
				if (flag)
				{
					IsRightRecursive = true;
					return IsRightRecursive;
				}
				bool flag2 = elem.MinCount > 0;
				if (flag2)
				{
					break;
				}
			}
			IsRightRecursive = false;
			return IsRightRecursive;
		}

		public bool IsMatchingEmpty()
		{
			return this.GetMinElementCount() == 0;
		}

		internal void SetPattern(ProductionPattern pattern)
		{
			this.m_pattern = pattern;
		}

		public int GetMinElementCount()
		{
			int min = 0;
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternElement elem = (ProductionPatternElement)this.elements[i];
				min += elem.MinCount;
			}
			return min;
		}

		public int GetMaxElementCount()
		{
			int max = 0;
			int num = this.elements.Count - 1;
			int GetMaxElementCount;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternElement elem = (ProductionPatternElement)this.elements[i];
				bool flag = elem.MaxCount >= 2147483647;
				if (flag)
				{
					GetMaxElementCount = 2147483647;
					return GetMaxElementCount;
				}
				max += elem.MaxCount;
			}
			GetMaxElementCount = max;
			return GetMaxElementCount;
		}

		public void AddToken(int id, int min, int max)
		{
			this.AddElement(new ProductionPatternElement(true, id, min, max));
		}

		public void AddProduction(int id, int min, int max)
		{
			this.AddElement(new ProductionPatternElement(false, id, min, max));
		}

		public void AddElement(ProductionPatternElement elem)
		{
			this.elements.Add(elem);
		}

		public void AddElement(ProductionPatternElement elem, int min, int max)
		{
			bool flag = elem.IsToken();
			if (flag)
			{
				this.AddToken(elem.Id, min, max);
			}
			else
			{
				this.AddProduction(elem.Id, min, max);
			}
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is ProductionPatternAlternative;
			return flag && this.Equals((ProductionPatternAlternative)obj);
		}

		public bool Equals(ProductionPatternAlternative alt)
		{
			bool flag = this.elements.Count != alt.elements.Count;
			bool Equals;
			if (flag)
			{
				Equals = false;
			}
			else
			{
				int num = this.elements.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					bool flag2 = !this.elements[i].Equals(RuntimeHelpers.GetObjectValue(alt.elements[i]));
					if (flag2)
					{
						Equals = false;
						return Equals;
					}
				}
				Equals = true;
			}
			return Equals;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					buffer.Append(" ");
				}
				buffer.Append(RuntimeHelpers.GetObjectValue(this.elements[i]));
			}
			return buffer.ToString();
		}
	}
}
