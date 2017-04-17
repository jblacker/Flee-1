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
    public class BatchLoadCompileException : Exception
    {
        internal BatchLoadCompileException(string atomName, string expressionText, ExpressionCompileException innerException)
            : base(string.Format("Batch Load: The expression for atom '${0}' could not be compiled", atomName), innerException)
        {
            this.AtomName = atomName;
            this.ExpressionText = expressionText;
        }

        private BatchLoadCompileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.AtomName = info.GetString("AtomName");
            this.ExpressionText = info.GetString("ExpressionText");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AtomName", this.AtomName);
            info.AddValue("ExpressionText", this.ExpressionText);
        }

        public string AtomName { get; }

        public string ExpressionText { get; }
    }
}