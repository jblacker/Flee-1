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

namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Text;

    internal class Token : Node
    {
        private readonly int endLine;

        private readonly int startColumn;

        private readonly int startLine;

        private Token next;

        private Token previous;

        public Token(TokenPattern pattern, string image, int line, int col)
        {
            this.Pattern = pattern;
            this.Image = image;
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
            var newline = this.Image.IndexOf('\n');
            buffer.Append(this.Pattern.Name);
            buffer.Append("(");
            buffer.Append(this.Pattern.Id);
            buffer.Append("): \"");
            var flag = newline >= 0;
            if (flag)
            {
                var flag2 = newline > 0 && this.Image[newline - 1] == '\r';
                if (flag2)
                {
                    newline--;
                }
                buffer.Append(this.Image.Substring(0, newline));
                buffer.Append("(...)");
            }
            else
            {
                buffer.Append(this.Image);
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
            var newline = this.Image.IndexOf('\n');
            buffer.Append('"');
            var flag = newline >= 0;
            if (flag)
            {
                var flag2 = newline > 0 && this.Image[newline - 1] == '\r';
                if (flag2)
                {
                    newline--;
                }
                buffer.Append(this.Image.Substring(0, newline));
                buffer.Append("(...)");
            }
            else
            {
                buffer.Append(this.Image);
            }
            buffer.Append('"');
            var flag3 = this.Pattern.Type == TokenPattern.PatternType.Regexp;
            if (flag3)
            {
                buffer.Append(" <");
                buffer.Append(this.Pattern.Name);
                buffer.Append(">");
            }
            return buffer.ToString();
        }

        public override int EndColumn { get; }

        public override int EndLine => this.endLine;

        public override int Id => this.Pattern.Id;

        public string Image { get; }

        public override string Name => this.Pattern.Name;

        public Token Next
        {
            get { return this.next; }
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

        internal TokenPattern Pattern { get; }

        public Token Previous
        {
            get { return this.previous; }
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

        public override int StartColumn => this.startColumn;

        public override int StartLine => this.startLine;
    }
}