namespace Flee.Exceptions
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

        private readonly ErrorType errorType;

        private readonly string info;

        private readonly List<string> details;

        private readonly int line;

        private readonly int column;

        public ErrorType Type => this.errorType;

        public string Info => this.info;

        public ICollection<string> Details => new List<string>(this.details);

        public int Line => this.line;

        public int Column => this.column;

        public override string Message
        {
            get
            {
                var buffer = new StringBuilder();
                buffer.AppendLine(this.ErrorMessage);
                var flag = this.line > 0 && this.column > 0;
                if (flag)
                {
                    var msg = FleeResourceManager.Instance.GetCompileErrorString("LineColumn");
                    msg = string.Format(msg, this.line, this.column);
                    buffer.AppendLine(msg);
                }
                return buffer.ToString();
            }
        }

        public string ErrorMessage
        {
            get
            {
                var args = new List<string>();

                switch (this.errorType)
                {
                    case ErrorType.Internal:
                    case ErrorType.Io:
                    case ErrorType.UnexpectedChar:
                    case ErrorType.InvalidToken:
                    case ErrorType.Analysis:
                        args.Add(this.info);
                        break;
                    case ErrorType.UnexpectedToken:
                        args.Add(this.info);
                        args.Add(this.GetMessageDetails());
                        break;
                }
                var msg = FleeResourceManager.Instance.GetCompileErrorString(this.errorType.ToString());
                return string.Format(msg, args);
            }
        }

        public ParseException(ErrorType type, string info, int line, int column) : this(type, info, null, line, column)
        {
        }

        public ParseException(ErrorType type, string info, IEnumerable<string> details, int line, int column)
        {
            this.errorType = type;
            this.info = info;
            this.details = details.ToList();
            this.line = line;
            this.column = column;
        }

        private ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.errorType = (ErrorType)info.GetInt32("Type");
            this.info = info.GetString("Info");
            this.details = (List<string>)info.GetValue("Details", typeof(List<string>));
            this.line = info.GetInt32("Line");
            this.column = info.GetInt32("Column");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type", (int)this.errorType);
            info.AddValue("Info", this.info);
            info.AddValue("Details", this.details);
            info.AddValue("Line", this.line);
            info.AddValue("Column", this.column);
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
            return this.column;
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
    }
}
