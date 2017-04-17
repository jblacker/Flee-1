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

namespace FleeSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    internal class ArgumentList
    {
        private readonly IList<ExpressionElement> myElements;

        public ArgumentList(ICollection elements)
        {
            var arr = new ExpressionElement[elements.Count - 1 + 1];
            elements.CopyTo(arr, 0);
            this.myElements = arr;
        }

        private string[] GetArgumentTypeNames()
        {
            return this.myElements.Map(i => i.ResultType.Name).ToArray();
        }

        public Type[] GetArgumentTypes()
        {
            return this.myElements.Map(i => i.ResultType).ToArray();
        }

        public override string ToString()
        {
            var typeNames = this.GetArgumentTypeNames();
            return Utility.FormatList(typeNames);
        }

        public ExpressionElement[] ToArray()
        {
            var arr = new ExpressionElement[this.myElements.Count - 1 + 1];
            this.myElements.CopyTo(arr, 0);
            return arr;
        }

        public int Count => this.myElements.Count;

        public ExpressionElement this[int index] => this.myElements[index];
    }
}