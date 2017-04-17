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
    using System;
    using System.Collections;
    using System.Runtime.Serialization;
    using System.Text;
    using Exceptions;

    [Serializable]
    internal class ParserLogException : Exception
    {
        private readonly ArrayList errors;

        public ParserLogException()
        {
            this.errors = new ArrayList();
        }

        private ParserLogException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.errors = new ArrayList();
            this.errors = (ArrayList) info.GetValue("Errors", typeof(ArrayList));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Errors", this.errors, typeof(ArrayList));
        }

        public int GetErrorCount()
        {
            return this.Count;
        }

        public ParseException GetError(int index)
        {
            return this[index];
        }

        public void AddError(ParseException e)
        {
            this.errors.Add(e);
        }

        public string GetMessage()
        {
            return this.Message;
        }

        public int Count
        {
            get { return this.errors.Count; }
        }

        public ParseException this[int index]
        {
            get { return (ParseException) this.errors[index]; }
        }

        public override string Message
        {
            get
            {
                var buffer = new StringBuilder();
                var num = this.Count - 1;
                for (var i = 0; i <= num; i++)
                {
                    var flag = i > 0;
                    if (flag)
                    {
                        buffer.Append("\n");
                    }
                    buffer.Append(this[i].Message);
                }
                return buffer.ToString();
            }
        }
    }
}