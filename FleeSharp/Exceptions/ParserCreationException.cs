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
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class ParserCreationException : Exception
    {
        public enum ErrorType
        {
            INTERNAL,
            INVALID_PARSER,
            INVALID_TOKEN,
            INVALID_PRODUCTION,
            INFINITE_LOOP,
            INHERENT_AMBIGUITY
        }

        private readonly ArrayList m_details;

        public ParserCreationException(ErrorType type, string info)
            : this(type, null, info)
        {
        }

        public ParserCreationException(ErrorType type, string name, string info)
            : this(type, name, info, null)
        {
        }

        public ParserCreationException(ErrorType type, string name, string info, ArrayList details)
        {
            this.Type = type;
            this.Name = name;
            this.Info = info;
            this.m_details = details;
        }

        public ErrorType GetErrorType()
        {
            return this.Type;
        }

        public string GetName()
        {
            return this.Name;
        }

        public string GetInfo()
        {
            return this.Info;
        }

        public string GetDetails()
        {
            return this.Details;
        }

        public string GetMessage()
        {
            return this.Message;
        }

        public string Details
        {
            get
            {
                var buffer = new StringBuilder();
                var flag = this.m_details == null;
                string Details;
                if (flag)
                {
                    Details = null;
                }
                else
                {
                    var num = this.m_details.Count - 1;
                    for (var i = 0; i <= num; i++)
                    {
                        var flag2 = i > 0;
                        if (flag2)
                        {
                            buffer.Append(", ");
                            var flag3 = i + 1 == this.m_details.Count;
                            if (flag3)
                            {
                                buffer.Append("and ");
                            }
                        }
                        buffer.Append(RuntimeHelpers.GetObjectValue(this.m_details[i]));
                    }
                    Details = buffer.ToString();
                }
                return Details;
            }
        }

        public string Info { get; }

        public override string Message
        {
            get
            {
                var buffer = new StringBuilder();
                switch (this.Type)
                {
                    case ErrorType.INVALID_PARSER:
                        buffer.Append("parser is invalid, as ");
                        buffer.Append(this.Info);
                        break;
                    case ErrorType.INVALID_TOKEN:
                        buffer.Append("token '");
                        buffer.Append(this.Name);
                        buffer.Append("' is invalid, as ");
                        buffer.Append(this.Info);
                        break;
                    case ErrorType.INVALID_PRODUCTION:
                        buffer.Append("production '");
                        buffer.Append(this.Name);
                        buffer.Append("' is invalid, as ");
                        buffer.Append(this.Info);
                        break;
                    case ErrorType.INFINITE_LOOP:
                        buffer.Append("infinite loop found in production pattern '");
                        buffer.Append(this.Name);
                        buffer.Append("'");
                        break;
                    case ErrorType.INHERENT_AMBIGUITY:
                    {
                        buffer.Append("inherent ambiguity in production '");
                        buffer.Append(this.Name);
                        buffer.Append("'");
                        var flag = this.Info != null;
                        if (flag)
                        {
                            buffer.Append(" ");
                            buffer.Append(this.Info);
                        }
                        var flag2 = this.m_details != null;
                        if (flag2)
                        {
                            buffer.Append(" starting with ");
                            var flag3 = this.m_details.Count > 1;
                            if (flag3)
                            {
                                buffer.Append("tokens ");
                            }
                            else
                            {
                                buffer.Append("token ");
                            }
                            buffer.Append(this.Details);
                        }
                        break;
                    }
                    default:
                        buffer.Append("internal error");
                        break;
                }
                return buffer.ToString();
            }
        }

        public string Name { get; }

        public ErrorType Type { get; }
    }
}