using System;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class Token : Node
	{
		private TokenPattern m_pattern;

		private string m_image;

		private int m_startLine;

		private int m_startColumn;

		private int m_endLine;

		private int m_endColumn;

		private Token m_previous;

		private Token m_next;

		public override int Id
		{
			get
			{
				return this.m_pattern.Id;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_pattern.Name;
			}
		}

		public override int StartLine
		{
			get
			{
				return this.m_startLine;
			}
		}

		public override int StartColumn
		{
			get
			{
				return this.m_startColumn;
			}
		}

		public override int EndLine
		{
			get
			{
				return this.m_endLine;
			}
		}

		public override int EndColumn
		{
			get
			{
				return this.m_endColumn;
			}
		}

		public string Image
		{
			get
			{
				return this.m_image;
			}
		}

		internal TokenPattern Pattern
		{
			get
			{
				return this.m_pattern;
			}
		}

		public Token Previous
		{
			get
			{
				return this.m_previous;
			}
			set
			{
				bool flag = this.m_previous != null;
				if (flag)
				{
					this.m_previous.Next = null;
				}
				this.m_previous = value;
				bool flag2 = this.m_previous != null;
				if (flag2)
				{
					this.m_previous.Next = this;
				}
			}
		}

		public Token Next
		{
			get
			{
				return this.m_next;
			}
			set
			{
				bool flag = this.m_next != null;
				if (flag)
				{
					this.m_next.Previous = null;
				}
				this.m_next = value;
				bool flag2 = this.m_next != null;
				if (flag2)
				{
					this.m_next.Previous = this;
				}
			}
		}

		public Token(TokenPattern pattern, string image, int line, int col)
		{
			this.m_pattern = pattern;
			this.m_image = image;
			this.m_startLine = line;
			this.m_startColumn = col;
			this.m_endLine = line;
			this.m_endColumn = col + image.Length - 1;
			int pos = 0;
			while (image.IndexOf('\n', pos) >= 0)
			{
				pos = image.IndexOf('\n', pos) + 1;
				this.m_endLine++;
				this.m_endColumn = image.Length - pos;
			}
		}

		public string GetImage()
		{
			return this.Image;
		}

		public Token GetPreviousToken()
		{
			return this.Previous;
		}

		public Token GetNextToken()
		{
			return this.Next;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			int newline = this.m_image.IndexOf('\n');
			buffer.Append(this.m_pattern.Name);
			buffer.Append("(");
			buffer.Append(this.m_pattern.Id);
			buffer.Append("): \"");
			bool flag = newline >= 0;
			if (flag)
			{
				bool flag2 = newline > 0 && this.m_image[newline - 1] == '\r';
				if (flag2)
				{
					newline--;
				}
				buffer.Append(this.m_image.Substring(0, newline));
				buffer.Append("(...)");
			}
			else
			{
				buffer.Append(this.m_image);
			}
			buffer.Append("\", line: ");
			buffer.Append(this.m_startLine);
			buffer.Append(", col: ");
			buffer.Append(this.m_startColumn);
			return buffer.ToString();
		}

		public string ToShortString()
		{
			StringBuilder buffer = new StringBuilder();
			int newline = this.m_image.IndexOf('\n');
			buffer.Append('"');
			bool flag = newline >= 0;
			if (flag)
			{
				bool flag2 = newline > 0 && this.m_image[newline - 1] == '\r';
				if (flag2)
				{
					newline--;
				}
				buffer.Append(this.m_image.Substring(0, newline));
				buffer.Append("(...)");
			}
			else
			{
				buffer.Append(this.m_image);
			}
			buffer.Append('"');
			bool flag3 = this.m_pattern.Type == TokenPattern.PatternType.REGEXP;
			if (flag3)
			{
				buffer.Append(" <");
				buffer.Append(this.m_pattern.Name);
				buffer.Append(">");
			}
			return buffer.ToString();
		}
	}
}
