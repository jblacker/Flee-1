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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    internal class ParseException : Exception
    {
        public enum ErrorType
        {
            Internal = 0,
            Io,
            UnexpectedEof,
            UnexpectedChar,
            UnexpectedToken,
            InvalidToken,
            Analysis
        }

        private readonly List<string> details;

        public ParseException(ErrorType type, string info, int line, int column)
            : this(type, info, null, line, column)
        {
        }

        public ParseException(ErrorType type, string info, IEnumerable<string> details, int line, int column)
        {
            this.Type = type;
            this.Info = info;
            this.details = details.ToList();
            this.Line = line;
            this.Column = column;
        }

        private ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Type = (ErrorType) info.GetInt32("Type");
            this.Info = info.GetString("Info");
            this.details = (List<string>) info.GetValue("Details", typeof(List<string>));
            this.Line = info.GetInt32("Line");
            this.Column = info.GetInt32("Column");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type", (int) this.Type);
            info.AddValue("Info", this.Info);
            info.AddValue("Details", this.details);
            info.AddValue("Line", this.Line);
            info.AddValue("Column", this.Column);
        }

        public ErrorType GetErrorType()
        {
            return this.Type;
        }

        public string GetInfo()
        {
            return this.Info;
        }

        public ICollection<string> GetDetails()
        {
            return this.Details;
        }

        public int GetLine()
        {
            return this.Line;
        }

        public int GetColumn()
        {
            return this.Column;
        }

        public string GetMessage()
        {
            return this.Message;
        }

        public string GetErrorMessage()
        {
            return this.ErrorMessage;
        }

        private string GetMessageDetails()
        {
            var buffer = new StringBuilder();
            var num = this.details.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = i > 0;
                if (flag)
                {
                    buffer.Append(", ");
                    var flag2 = i + 1 == this.details.Count;
                    if (flag2)
                    {
                        buffer.Append("or ");
                    }
                }
                buffer.Append(RuntimeHelpers.GetObjectValue(this.details[i]));
            }
            return buffer.ToString();
        }

        public int Column { get; }

        public ICollection<string> Details => new List<string>(this.details);

        public string ErrorMessage
        {
            get
            {
                var args = new List<string>();

                switch (this.Type)
                {
                    case ErrorType.Internal:
                    case ErrorType.Io:
                    case ErrorType.UnexpectedChar:
                    case ErrorType.InvalidToken:
                    case ErrorType.Analysis:
                        args.Add(this.Info);
                        break;
                    case ErrorType.UnexpectedToken:
                        args.Add(this.Info);
                        args.Add(this.GetMessageDetails());
                        break;
                }
                var msg = FleeResourceManager.Instance.GetCompileErrorString(this.Type.ToString());
                return string.Format(msg, args);
            }
        }

        public string Info { get; }

        public int Line { get; }

        public override string Message
        {
            get
            {
                var buffer = new StringBuilder();
                buffer.AppendLine(this.ErrorMessage);
                var flag = this.Line > 0 && this.Column > 0;
                if (flag)
                {
                    var msg = FleeResourceManager.Instance.GetCompileErrorString("LineColumn");
                    msg = string.Format(msg, this.Line, this.Column);
                    buffer.AppendLine(msg);
                }
                return buffer.ToString();
            }
        }

        public ErrorType Type { get; }
    }
}