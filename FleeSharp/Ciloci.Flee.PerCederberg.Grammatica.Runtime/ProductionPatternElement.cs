using System;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class ProductionPatternElement
	{
		private bool token;

		private int m_id;

		private int min;

		private int max;

		private LookAheadSet m_lookAhead;

		public int Id
		{
			get
			{
				return this.m_id;
			}
		}

		public int MinCount
		{
			get
			{
				return this.min;
			}
		}

		public int MaxCount
		{
			get
			{
				return this.max;
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

		public ProductionPatternElement(bool isToken, int id, int min, int max)
		{
			this.token = isToken;
			this.m_id = id;
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
			return this.IsToken() && token != null && token.Id == this.m_id;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is ProductionPatternElement;
			bool Equals;
			if (flag)
			{
				ProductionPatternElement elem = (ProductionPatternElement)obj;
				Equals = (this.token == elem.token && this.m_id == elem.Id && this.min == elem.min && this.max == elem.max);
			}
			else
			{
				Equals = false;
			}
			return Equals;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(this.m_id);
			bool flag = this.token;
			if (flag)
			{
				buffer.Append("(Token)");
			}
			else
			{
				buffer.Append("(Production)");
			}
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
