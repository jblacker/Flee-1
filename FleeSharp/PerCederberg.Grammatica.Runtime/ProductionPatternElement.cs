namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Text;

    internal class ProductionPatternElement
	{
		private readonly bool token;

		private readonly int id;

		private readonly int min;

		private readonly int max;

	    public int Id => this.id;

	    public int MinCount => this.min;

	    public int MaxCount => this.max;

	    internal LookAheadSet LookAhead { get; set; }

	    public ProductionPatternElement(bool isToken, int id, int min, int max)
		{
			this.token = isToken;
			this.id = id;
			bool flag = min < 0;
			if (flag)
			{
				min = 0;
			}
			this.min = min;
			bool flag2 = max <= 0;
			if (flag2)
			{
				max = 2147483647;
			}
			else
			{
				bool flag3 = max < min;
				if (flag3)
				{
					max = min;
				}
			}
			this.max = max;
		}

		public int GetId()
		{
			return this.Id;
		}

		public int GetMinCount()
		{
			return this.MinCount;
		}

		public int GetMaxCount()
		{
			return this.MaxCount;
		}

		public bool IsToken()
		{
			return this.token;
		}

		public bool IsProduction()
		{
			return !this.token;
		}

		public bool IsMatch(Token token)
		{
			return this.IsToken() && token != null && token.Id == this.id;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is ProductionPatternElement;
			bool equals;
			if (flag)
			{
				var elem = (ProductionPatternElement)obj;
				equals = (this.token == elem.token && this.id == elem.Id && this.min == elem.min && this.max == elem.max);
			}
			else
			{
				equals = false;
			}
			return equals;
		}

		public override string ToString()
		{
			var buffer = new StringBuilder();
			buffer.Append(this.id);
			bool flag = this.token;
		    buffer.Append(flag ? "(Token)" : "(Production)");
		    bool flag2 = this.min != 1 || this.max != 1;
			if (flag2)
			{
				buffer.Append("{");
				buffer.Append(this.min);
				buffer.Append(",");
				buffer.Append(this.max);
				buffer.Append("}");
			}
			return buffer.ToString();
		}
	}
}
