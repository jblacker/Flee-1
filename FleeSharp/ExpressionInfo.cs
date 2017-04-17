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
    using System.Collections.Generic;

    public sealed class ExpressionInfo
    {
        private readonly IDictionary<string, object> myData;

        internal ExpressionInfo()
        {
            this.myData = new Dictionary<string, object>
                {{"ReferencedVariables", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)}};
        }

        internal void AddReferencedVariable(string name)
        {
            var dict = (IDictionary<string, string>) this.myData["ReferencedVariables"];
            dict[name] = name;
        }

        public string[] GetReferencedVariables()
        {
            var dict = (IDictionary<string, string>) this.myData["ReferencedVariables"];
            var arr = new string[dict.Count - 1 + 1];
            dict.Keys.CopyTo(arr, 0);
            return arr;
        }
    }
}