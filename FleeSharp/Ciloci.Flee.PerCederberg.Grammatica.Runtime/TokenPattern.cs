using System;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class TokenPattern
	{
		public enum PatternType
		{
			STRING,
			REGEXP
		}

		private int m_id;

		private string m_name;

		private TokenPattern.PatternType m_type;

		private string m_pattern;

		private bool m_error;

		private string m_errorMessage;

		private bool m_ignore;

		private string m_ignoreMessage;

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

		public TokenPattern.PatternType Type
		{
			get
			{
				return this.m_type;
			}
		}

		public string Pattern
		{
			get
			{
				return this.m_pattern;
			}
			set
			{
				this.m_pattern = value;
			}
		}

		public bool Error
		{
			get
			{
				return this.m_error;
			}
			set
			{
				this.m_error = value;
				bool flag = this.m_error && this.m_errorMessage == null;
				if (flag)
				{
					this.m_errorMessage = "unrecognized token found";
				}
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
			set
			{
				this.m_error = true;
				this.m_errorMessage = value;
			}
		}

		public bool Ignore
		{
			get
			{
				return this.m_ignore;
			}
			set
			{
				this.m_ignore = value;
			}
		}

		public string IgnoreMessage
		{
			get
			{
				return this.m_ignoreMessage;
			}
			set
			{
				this.m_ignore = true;
				this.m_ignoreMessage = value;
			}
		}

		protected TokenPattern()
		{
		}

		public TokenPattern(int id, string name, TokenPattern.PatternType type, string pattern)
		{
			this.SetData(id, name, type, pattern);
		}

		protected void SetData(int id, string name, TokenPattern.PatternType type, string pattern)
		{
			this.m_id = id;
			this.m_name = name;
			this.m_type = type;
			this.m_pattern = pattern;
		}

		public int GetId()
		{
			return this.m_id;
		}

		public string GetName()
		{
			return this.m_name;
		}

		public TokenPattern.PatternType GetPatternType()
		{
			return this.m_type;
		}

		public string GetPattern()
		{
			return this.m_pattern;
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
			StringBuilder buffer = new StringBuilder();
			buffer.Append(this.m_name);
			buffer.Append(" (");
			buffer.Append(this.m_id);
			buffer.Append("): ");
			TokenPattern.PatternType type = this.m_type;
			if (type != TokenPattern.PatternType.STRING)
			{
				if (type == TokenPattern.PatternType.REGEXP)
				{
					buffer.Append("<<");
					buffer.Append(this.m_pattern);
					buffer.Append(">>");
				}
			}
			else
			{
				buffer.Append("\"");
				buffer.Append(this.m_pattern);
				buffer.Append("\"");
			}
			bool error = this.m_error;
			if (error)
			{
				buffer.Append(" ERROR: \"");
				buffer.Append(this.m_errorMessage);
				buffer.Append("\"");
			}
			bool ignore = this.m_ignore;
			if (ignore)
			{
				buffer.Append(" IGNORE");
				bool flag = this.m_ignoreMessage != null;
				if (flag)
				{
					buffer.Append(": \"");
					buffer.Append(this.m_ignoreMessage);
					buffer.Append("\"");
				}
			}
			return buffer.ToString();
		}

		public string ToShortString()
		{
			StringBuilder buffer = new StringBuilder();
			int newline = this.m_pattern.IndexOf('\n');
			bool flag = this.m_type == TokenPattern.PatternType.STRING;
			if (flag)
			{
				buffer.Append("\"");
				bool flag2 = newline >= 0;
				if (flag2)
				{
					bool flag3 = newline > 0 && this.m_pattern[newline - 1] == '\r';
					if (flag3)
					{
						newline--;
					}
					buffer.Append(this.m_pattern.Substring(0, newline));
					buffer.Append("(...)");
				}
				else
				{
					buffer.Append(this.m_pattern);
				}
				buffer.Append("\"");
			}
			else
			{
				buffer.Append("<");
				buffer.Append(this.m_name);
				buffer.Append(">");
			}
			return buffer.ToString();
		}
	}
}
