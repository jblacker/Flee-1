using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class ProductionPattern
	{
		private int m_id;

		private string m_name;

		private bool m_synthetic;

		private ArrayList alternatives;

		private int defaultAlt;

		private LookAheadSet m_lookAhead;

		public int Id
		{
			get
			{
				return this.m_id;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public bool Synthetic
		{
			get
			{
				return this.m_synthetic;
			}
			set
			{
				this.m_synthetic = value;
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

		internal ProductionPatternAlternative DefaultAlternative
		{
			get
			{
				bool flag = this.defaultAlt >= 0;
				ProductionPatternAlternative DefaultAlternative;
				if (flag)
				{
					object obj = RuntimeHelpers.GetObjectValue(this.alternatives[this.defaultAlt]);
					DefaultAlternative = (ProductionPatternAlternative)obj;
				}
				else
				{
					DefaultAlternative = null;
				}
				return DefaultAlternative;
			}
			set
			{
				this.defaultAlt = 0;
				int num = this.alternatives.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					bool flag = this.alternatives[i] == value;
					if (flag)
					{
						this.defaultAlt = i;
					}
				}
			}
		}

		public int Count
		{
			get
			{
				return this.alternatives.Count;
			}
		}

		public ProductionPatternAlternative this[int index]
		{
			get
			{
				return (ProductionPatternAlternative)this.alternatives[index];
			}
		}

		public ProductionPattern(int id, string name)
		{
			this.m_id = id;
			this.m_name = name;
			this.alternatives = new ArrayList();
			this.defaultAlt = -1;
		}

		public int GetId()
		{
			return this.Id;
		}

		public string GetName()
		{
			return this.Name;
		}

		public bool IsSyntetic()
		{
			return this.Synthetic;
		}

		public void SetSyntetic(bool synthetic)
		{
			synthetic = synthetic;
		}

		public int GetAlternativeCount()
		{
			return this.Count;
		}

		public ProductionPatternAlternative GetAlternative(int pos)
		{
			return this[pos];
		}

		public bool IsLeftRecursive()
		{
			int num = this.alternatives.Count - 1;
			bool IsLeftRecursive;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternAlternative alt = (ProductionPatternAlternative)this.alternatives[i];
				bool flag = alt.IsLeftRecursive();
				if (flag)
				{
					IsLeftRecursive = true;
					return IsLeftRecursive;
				}
			}
			IsLeftRecursive = false;
			return IsLeftRecursive;
		}

		public bool IsRightRecursive()
		{
			int num = this.alternatives.Count - 1;
			bool IsRightRecursive;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternAlternative alt = (ProductionPatternAlternative)this.alternatives[i];
				bool flag = alt.IsRightRecursive();
				if (flag)
				{
					IsRightRecursive = true;
					return IsRightRecursive;
				}
			}
			IsRightRecursive = false;
			return IsRightRecursive;
		}

		public bool IsMatchingEmpty()
		{
			int num = this.alternatives.Count - 1;
			bool IsMatchingEmpty;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternAlternative alt = (ProductionPatternAlternative)this.alternatives[i];
				bool flag = alt.IsMatchingEmpty();
				if (flag)
				{
					IsMatchingEmpty = true;
					return IsMatchingEmpty;
				}
			}
			IsMatchingEmpty = false;
			return IsMatchingEmpty;
		}

		public void AddAlternative(ProductionPatternAlternative alt)
		{
			bool flag = this.alternatives.Contains(alt);
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, this.m_name, "two identical alternatives exist");
			}
			alt.SetPattern(this);
			this.alternatives.Add(alt);
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			StringBuilder indent = new StringBuilder();
			buffer.Append(this.m_name);
			buffer.Append("(");
			buffer.Append(this.m_id);
			buffer.Append(") ");
			int num = buffer.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				indent.Append(" ");
			}
			int num2 = this.alternatives.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				bool flag = i == 0;
				if (flag)
				{
					buffer.Append("= ");
				}
				else
				{
					buffer.Append("\n");
					buffer.Append(indent);
					buffer.Append("| ");
				}
				buffer.Append(RuntimeHelpers.GetObjectValue(this.alternatives[i]));
			}
			return buffer.ToString();
		}
	}
}
