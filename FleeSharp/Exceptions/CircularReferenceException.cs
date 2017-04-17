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
    using System.Runtime.Serialization;

    [Serializable]
    public class CircularReferenceException : Exception
    {
        private readonly string MyCircularReferenceSource;

        internal CircularReferenceException()
        {
        }

        internal CircularReferenceException(string circularReferenceSource)
        {
            this.MyCircularReferenceSource = circularReferenceSource;
        }

        private CircularReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string Message
        {
            get
            {
                var flag = this.MyCircularReferenceSource == null;
                string Message;
                if (flag)
                {
                    Message = "Circular reference detected in calculation engine";
                }
                else
                {
                    Message = string.Format("Circular reference detected in calculation engine at '{0}'",
                        this.MyCircularReferenceSource);
                }
                return Message;
            }
        }
    }
}