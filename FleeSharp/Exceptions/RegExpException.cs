// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp.Exceptions
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

        private readonly string pattern;

        private readonly int position;

        private readonly ErrorType type;

        public RegExpException(ErrorType type, int pos, string pattern)
        {
            this.type = type;
            this.position = pos;
            this.pattern = pattern;
        }

        public string GetMessage()
        {
            var buffer = new StringBuilder();
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
            var flag = this.position < this.pattern.Length;
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

        public override string Message => this.GetMessage();
    }
}