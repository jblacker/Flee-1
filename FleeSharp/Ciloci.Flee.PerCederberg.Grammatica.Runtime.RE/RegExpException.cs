using System;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class RegExpException : Exception
	{
		public enum ErrorType
		{
			UNEXPECTED_CHARACTER,
			UNTERMINATED_PATTERN,
			UNSUPPORTED_SPECIAL_CHARACTER,
			UNSUPPORTED_ESCAPE_CHARACTER,
			INVALID_REPEAT_COUNT
		}

		private RegExpException.ErrorType type;

		private int position;

		private string pattern;

		public override string Message
		{
			get
			{
				return this.GetMessage();
			}
		}

		public RegExpException(RegExpException.ErrorType type, int pos, string pattern)
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
			case RegExpException.ErrorType.UNEXPECTED_CHARACTER:
				buffer.Append("unexpected character");
				break;
			case RegExpException.ErrorType.UNTERMINATED_PATTERN:
				buffer.Append("unterminated pattern");
				break;
			case RegExpException.ErrorType.UNSUPPORTED_SPECIAL_CHARACTER:
				buffer.Append("unsupported character");
				break;
			case RegExpException.ErrorType.UNSUPPORTED_ESCAPE_CHARACTER:
				buffer.Append("unsupported escape character");
				break;
			case RegExpException.ErrorType.INVALID_REPEAT_COUNT:
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
