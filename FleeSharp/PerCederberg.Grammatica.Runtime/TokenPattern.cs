namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Text;

    internal class TokenPattern
	{
		public enum PatternType
		{
			String,
			Regexp
		}

		private int id;

		private string name;

		private PatternType patternType;

		private string pattern;

		private bool hasError;

		private string errorMessage;

		private bool ignore;

		private string ignoreMessage;

		public int Id => this.id;

	    public string Name => this.name;

	    public PatternType Type => this.patternType;

	    public string Pattern
		{
			get
			{
				return this.pattern;
			}
			set
			{
				this.pattern = value;
			}
		}

		public bool Error
		{
			get
			{
				return this.hasError;
			}
			set
			{
				this.hasError = value;
				var flag = this.hasError && this.errorMessage == null;
				if (flag)
				{
					this.errorMessage = "unrecognized token found";
				}
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.hasError = true;
				this.errorMessage = value;
			}
		}

		public bool Ignore
		{
			get
			{
				return this.ignore;
			}
			set
			{
				this.ignore = value;
			}
		}

		public string IgnoreMessage
		{
			get
			{
				return this.ignoreMessage;
			}
			set
			{
				this.ignore = true;
				this.ignoreMessage = value;
			}
		}

		protected TokenPattern()
		{
		}

		public TokenPattern(int id, string name, PatternType type, string pattern)
		{
			this.SetData(id, name, type, pattern);
		}

		protected void SetData(int id, string name, PatternType type, string pattern)
		{
			this.id = id;
			this.name = name;
			this.patternType = type;
			this.pattern = pattern;
		}

		public int GetId()
		{
			return this.id;
		}

		public string GetName()
		{
			return this.name;
		}

		public PatternType GetPatternType()
		{
			return this.patternType;
		}

		public string GetPattern()
		{
			return this.pattern;
		}

		public bool IsError()
		{
			return this.Error;
		}

		public string GetErrorMessage()
		{
			return this.ErrorMessage;
		}

		public void SetError()
		{
			this.Error = true;
		}

		public void SetError(string message)
		{
			this.ErrorMessage = message;
		}

		public bool IsIgnore()
		{
			return this.Ignore;
		}

		public string GetIgnoreMessage()
		{
			return this.IgnoreMessage;
		}

		public void SetIgnore()
		{
			this.Ignore = true;
		}

		public void SetIgnore(string message)
		{
			this.IgnoreMessage = message;
		}

		public override string ToString()
		{
			var buffer = new StringBuilder();
			buffer.Append(this.name);
			buffer.Append(" (");
			buffer.Append(this.id);
			buffer.Append("): ");
			var type = this.patternType;
			if (type != PatternType.String)
			{
				if (type == PatternType.Regexp)
				{
					buffer.Append("<<");
					buffer.Append(this.pattern);
					buffer.Append(">>");
				}
			}
			else
			{
				buffer.Append("\"");
				buffer.Append(this.pattern);
				buffer.Append("\"");
			}
			var error = this.hasError;
			if (error)
			{
				buffer.Append(" ERROR: \"");
				buffer.Append(this.errorMessage);
				buffer.Append("\"");
			}
			var ignore = this.ignore;
			if (ignore)
			{
				buffer.Append(" IGNORE");
				var flag = this.ignoreMessage != null;
				if (flag)
				{
					buffer.Append(": \"");
					buffer.Append(this.ignoreMessage);
					buffer.Append("\"");
				}
			}
			return buffer.ToString();
		}

		public string ToShortString()
		{
			var buffer = new StringBuilder();
			var newline = this.pattern.IndexOf('\n');
			var flag = this.patternType == PatternType.String;
			if (flag)
			{
				buffer.Append("\"");
				var flag2 = newline >= 0;
				if (flag2)
				{
					var flag3 = newline > 0 && this.pattern[newline - 1] == '\r';
					if (flag3)
					{
						newline--;
					}
					buffer.Append(this.pattern.Substring(0, newline));
					buffer.Append("(...)");
				}
				else
				{
					buffer.Append(this.pattern);
				}
				buffer.Append("\"");
			}
			else
			{
				buffer.Append("<");
				buffer.Append(this.name);
				buffer.Append(">");
			}
			return buffer.ToString();
		}
	}
}
