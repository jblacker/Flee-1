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
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualBasic;

    internal class Automaton
    {
        private readonly AutomatonTree tree;
        private object value;

        public Automaton()
        {
            this.tree = new AutomatonTree();
        }

        public void AddMatch(string str, bool caseInsensitive, object value)
        {
            var flag = str.Length == 0;
            if (flag)
            {
                this.value = RuntimeHelpers.GetObjectValue(value);
            }
            else
            {
                var state = this.tree.Find(str[0], caseInsensitive);
                var flag2 = state == null;
                if (flag2)
                {
                    state = new Automaton();
                    state.AddMatch(str.Substring(1), caseInsensitive, RuntimeHelpers.GetObjectValue(value));
                    this.tree.Add(str[0], caseInsensitive, state);
                }
                else
                {
                    state.AddMatch(str.Substring(1), caseInsensitive, RuntimeHelpers.GetObjectValue(value));
                }
            }
        }

        public object MatchFrom(LookAheadReader input, int pos, bool caseInsensitive)
        {
            object result = null;
            var c = input.Peek(pos);
            var flag = this.tree != null && c >= 0;
            if (flag)
            {
                var state = this.tree.Find(Convert.ToChar(c), caseInsensitive);
                var flag2 = state != null;
                if (flag2)
                {
                    result = RuntimeHelpers.GetObjectValue(state.MatchFrom(input, pos + 1, caseInsensitive));
                }
            }
            return Interaction.IIf(result == null, RuntimeHelpers.GetObjectValue(this.value), RuntimeHelpers.GetObjectValue(result));
        }
    }
}