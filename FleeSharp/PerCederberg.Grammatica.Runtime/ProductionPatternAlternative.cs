namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class ProductionPatternAlternative
    {
		private ProductionPattern productionPattern;

		private readonly ArrayList elements;

        public ProductionPattern Pattern => this.productionPattern;

        internal LookAheadSet LookAheadSet { get; set; }

        public int Count => this.elements.Count;

        public ProductionPatternElement this[int index] => (ProductionPatternElement)this.elements[index];

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
		    for (int i = 0; i <= num; i++)
			{
				var elem = (ProductionPatternElement)this.elements[i];
				bool flag = elem.Id == this.productionPattern.Id;
				if (flag)
				{
				    return true;
				}
				bool flag2 = elem.MinCount > 0;
				if (flag2)
				{
					break;
				}
			}
		    return false;
		}

		public bool IsRightRecursive()
		{
			int num = this.elements.Count - 1;
		    for (int i = num; i >= 0; i += -1)
			{
				var elem = (ProductionPatternElement)this.elements[i];
				bool flag = elem.Id == this.productionPattern.Id;
				if (flag)
				{
				    return true;
				}
				bool flag2 = elem.MinCount > 0;
				if (flag2)
				{
					break;
				}
			}
		    return false;
		}

		public bool IsMatchingEmpty()
		{
			return this.GetMinElementCount() == 0;
		}

		internal void SetPattern(ProductionPattern pattern)
		{
			this.productionPattern = pattern;
		}

		public int GetMinElementCount()
		{
			int min = 0;
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				var elem = (ProductionPatternElement)this.elements[i];
				min += elem.MinCount;
			}
			return min;
		}

		public int GetMaxElementCount()
		{
			int max = 0;
			int num = this.elements.Count - 1;
			int getMaxElementCount;
			for (int i = 0; i <= num; i++)
			{
				var elem = (ProductionPatternElement)this.elements[i];
				bool flag = elem.MaxCount >= 2147483647;
				if (flag)
				{
					getMaxElementCount = 2147483647;
					return getMaxElementCount;
				}
				max += elem.MaxCount;
			}
			getMaxElementCount = max;
			return getMaxElementCount;
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
			bool equals;
			if (flag)
			{
				equals = false;
			}
			else
			{
				int num = this.elements.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					bool flag2 = !this.elements[i].Equals(RuntimeHelpers.GetObjectValue(alt.elements[i]));
					if (flag2)
					{
					    return false;
					}
				}
				equals = true;
			}
			return equals;
		}

		public override string ToString()
		{
			var buffer = new StringBuilder();
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
