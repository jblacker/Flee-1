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

namespace FleeSharp.PerCederberg.Grammatica.Runtime
{
    using System.Text;

    internal class TokenPattern
    {
        public enum PatternType
        {
            String,
            Regexp
        }

        private string errorMessage;

        private bool hasError;

        private string ignoreMessage;

        protected TokenPattern()
        {
        }

        public TokenPattern(int id, string name, PatternType type, string pattern)
        {
            this.SetData(id, name, type, pattern);
        }

        protected void SetData(int id, string name, PatternType type, string pattern)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Pattern = pattern;
        }

        public int GetId()
        {
            return this.Id;
        }

        public string GetName()
        {
            return this.Name;
        }

        public PatternType GetPatternType()
        {
            return this.Type;
        }

        public string GetPattern()
        {
            return this.Pattern;
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
            buffer.Append(this.Name);
            buffer.Append(" (");
            buffer.Append(this.Id);
            buffer.Append("): ");
            var type = this.Type;
            if (type != PatternType.String)
            {
                if (type == PatternType.Regexp)
                {
                    buffer.Append("<<");
                    buffer.Append(this.Pattern);
                    buffer.Append(">>");
                }
            }
            else
            {
                buffer.Append("\"");
                buffer.Append(this.Pattern);
                buffer.Append("\"");
            }
            var error = this.hasError;
            if (error)
            {
                buffer.Append(" ERROR: \"");
                buffer.Append(this.errorMessage);
                buffer.Append("\"");
            }
            var ignore = this.Ignore;
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
            var newline = this.Pattern.IndexOf('\n');
            var flag = this.Type == PatternType.String;
            if (flag)
            {
                buffer.Append("\"");
                var flag2 = newline >= 0;
                if (flag2)
                {
                    var flag3 = newline > 0 && this.Pattern[newline - 1] == '\r';
                    if (flag3)
                    {
                        newline--;
                    }
                    buffer.Append(this.Pattern.Substring(0, newline));
                    buffer.Append("(...)");
                }
                else
                {
                    buffer.Append(this.Pattern);
                }
                buffer.Append("\"");
            }
            else
            {
                buffer.Append("<");
                buffer.Append(this.Name);
                buffer.Append(">");
            }
            return buffer.ToString();
        }

        public bool Error
        {
            get { return this.hasError; }
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
            get { return this.errorMessage; }
            set
            {
                this.hasError = true;
                this.errorMessage = value;
            }
        }

        public int Id { get; private set; }

        public bool Ignore { get; set; }

        public string IgnoreMessage
        {
            get { return this.ignoreMessage; }
            set
            {
                this.Ignore = true;
                this.ignoreMessage = value;
            }
        }

        public string Name { get; private set; }

        public string Pattern { get; set; }

        public PatternType Type { get; private set; }
    }
}