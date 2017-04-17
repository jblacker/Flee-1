namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Exceptions;

    internal class ProductionPattern
	{
	    private readonly ArrayList alternatives;

		private int defaultAlt;

	    public int Id { get; set; }

	    public string Name { get; set; }

        public bool Synthetic { get; set; }

        internal LookAheadSet LookAhead { get; set; }

        internal ProductionPatternAlternative DefaultAlternative
		{
			get
			{
				var flag = this.defaultAlt >= 0;
				ProductionPatternAlternative defaultAlternatives;
				if (flag)
				{
					var obj = RuntimeHelpers.GetObjectValue(this.alternatives[this.defaultAlt]);
					defaultAlternatives = (ProductionPatternAlternative)obj;
				}
				else
				{
					defaultAlternatives = null;
				}
				return defaultAlternatives;
			}
			set
			{
				this.defaultAlt = 0;
				var num = this.alternatives.Count - 1;
				for (var i = 0; i <= num; i++)
				{
					var flag = Equals(this.alternatives[i], value);
					if (flag)
					{
						this.defaultAlt = i;
					}
				}
			}
		}

		public int Count => this.alternatives.Count;

	    public ProductionPatternAlternative this[int index] => (ProductionPatternAlternative)this.alternatives[index];

	    public ProductionPattern(int id, string name)
		{
			this.Id = id;
			this.Name = name;
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
			Synthetic = synthetic;
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
			var num = this.alternatives.Count - 1;
		    for (var i = 0; i <= num; i++)
			{
				var alt = (ProductionPatternAlternative)this.alternatives[i];
				var flag = alt.IsLeftRecursive();
				if (flag)
				{
				    return true;
				}
			}
		    return false;
		}

		public bool IsRightRecursive()
		{
			var num = this.alternatives.Count - 1;
		    for (var i = 0; i <= num; i++)
			{
				var alt = (ProductionPatternAlternative)this.alternatives[i];
				var flag = alt.IsRightRecursive();
				if (flag)
				{
				    return true;
				}
			}
		    return false;
		}

		public bool IsMatchingEmpty()
		{
			var num = this.alternatives.Count - 1;
		    for (var i = 0; i <= num; i++)
			{
				var alt = (ProductionPatternAlternative)this.alternatives[i];
				var flag = alt.IsMatchingEmpty();
				if (flag)
				{
				    return true;
				}
			}
		    return false;
		}

		public void AddAlternative(ProductionPatternAlternative alt)
		{
			var flag = this.alternatives.Contains(alt);
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, this.Name, "two identical alternatives exist");
			}
			alt.SetPattern(this);
			this.alternatives.Add(alt);
		}

		public override string ToString()
		{
			var buffer = new StringBuilder();
			var indent = new StringBuilder();
			buffer.Append(this.Name);
			buffer.Append("(");
			buffer.Append(this.Id);
			buffer.Append(") ");
			var num = buffer.Length - 1;
			for (var i = 0; i <= num; i++)
			{
				indent.Append(" ");
			}
			var num2 = this.alternatives.Count - 1;
			for (var i = 0; i <= num2; i++)
			{
				var flag = i == 0;
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
