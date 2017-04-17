namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Text;

    internal class Token : Node
	{
		private readonly TokenPattern tokenPattern;

		private readonly string image;

		private readonly int startLine;

		private readonly int startColumn;

		private readonly int endLine;

	    private Token previous;

		private Token next;

		public override int Id => this.tokenPattern.Id;

	    public override string Name => this.tokenPattern.Name;

	    public override int StartLine => this.startLine;

	    public override int StartColumn => this.startColumn;

	    public override int EndLine => this.endLine;

	    public override int EndColumn { get; }

	    public string Image => this.image;

	    internal TokenPattern Pattern => this.tokenPattern;

	    public Token Previous
		{
			get
			{
				return this.previous;
			}
			set
			{
				var flag = this.previous != null;
				if (flag)
				{
					this.previous.Next = null;
				}
				this.previous = value;
				var flag2 = this.previous != null;
				if (flag2)
				{
					this.previous.Next = this;
				}
			}
		}

		public Token Next
		{
			get
			{
				return this.next;
			}
			set
			{
				var flag = this.next != null;
				if (flag)
				{
					this.next.Previous = null;
				}
				this.next = value;
				var flag2 = this.next != null;
				if (flag2)
				{
					this.next.Previous = this;
				}
			}
		}

		public Token(TokenPattern pattern, string image, int line, int col)
		{
			this.tokenPattern = pattern;
			this.image = image;
			this.startLine = line;
			this.startColumn = col;
			this.endLine = line;
			this.EndColumn = col + image.Length - 1;
			var pos = 0;
			while (image.IndexOf('\n', pos) >= 0)
			{
				pos = image.IndexOf('\n', pos) + 1;
				this.endLine++;
				this.EndColumn = image.Length - pos;
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
			var buffer = new StringBuilder();
			var newline = this.image.IndexOf('\n');
			buffer.Append(this.tokenPattern.Name);
			buffer.Append("(");
			buffer.Append(this.tokenPattern.Id);
			buffer.Append("): \"");
			var flag = newline >= 0;
			if (flag)
			{
				var flag2 = newline > 0 && this.image[newline - 1] == '\r';
				if (flag2)
				{
					newline--;
				}
				buffer.Append(this.image.Substring(0, newline));
				buffer.Append("(...)");
			}
			else
			{
				buffer.Append(this.image);
			}
			buffer.Append("\", line: ");
			buffer.Append(this.startLine);
			buffer.Append(", col: ");
			buffer.Append(this.startColumn);
			return buffer.ToString();
		}

		public string ToShortString()
		{
			var buffer = new StringBuilder();
			var newline = this.image.IndexOf('\n');
			buffer.Append('"');
			var flag = newline >= 0;
			if (flag)
			{
				var flag2 = newline > 0 && this.image[newline - 1] == '\r';
				if (flag2)
				{
					newline--;
				}
				buffer.Append(this.image.Substring(0, newline));
				buffer.Append("(...)");
			}
			else
			{
				buffer.Append(this.image);
			}
			buffer.Append('"');
			var flag3 = this.tokenPattern.Type == TokenPattern.PatternType.Regexp;
			if (flag3)
			{
				buffer.Append(" <");
				buffer.Append(this.tokenPattern.Name);
				buffer.Append(">");
			}
			return buffer.ToString();
		}
	}
}
