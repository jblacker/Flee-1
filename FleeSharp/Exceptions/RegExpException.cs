namespace Flee.Exceptions
{
    using System;
    using System.Text;

    internal class RegExpException : Exception
	{
		public enum ErrorType
		{
			UnexpectedCharacter = 0,
			UnterminatedPattern,
			UnsupportedSpecialCharacter,
			UnsupportedEscapeCharacter,
			InvalidRepeatCount
		}

		private readonly ErrorType type;

		private readonly int position;

		private readonly string pattern;

		public override string Message => this.GetMessage();

	    public RegExpException(ErrorType type, int pos, string pattern)
		{
			this.type = type;
			this.position = pos;
			this.pattern = pattern;
		}

		public string GetMessage()
		{
			StringBuilder buffer = new StringBuilder();
			switch (this.type)
			{
			case ErrorType.UnexpectedCharacter:
				buffer.Append("unexpected character");
				break;
			case ErrorType.UnterminatedPattern:
				buffer.Append("unterminated pattern");
				break;
			case ErrorType.UnsupportedSpecialCharacter:
				buffer.Append("unsupported character");
				break;
			case ErrorType.UnsupportedEscapeCharacter:
				buffer.Append("unsupported escape character");
				break;
			case ErrorType.InvalidRepeatCount:
				buffer.Append("invalid repeat count");
				break;
			default:
				buffer.Append("internal error");
				break;
			}
			buffer.Append(": ");
			bool flag = this.position < this.pattern.Length;
			if (flag)
			{
				buffer.Append('\'');
				buffer.Append(this.pattern.Substring(this.position));
				buffer.Append('\'');
			}
			else
			{
				buffer.Append("<end of pattern>");
			}
			buffer.Append(" at position ");
			buffer.Append(this.position);
			return buffer.ToString();
		}
	}
}
