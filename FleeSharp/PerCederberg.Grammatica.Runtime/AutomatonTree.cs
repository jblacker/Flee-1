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
    internal class AutomatonTree
    {
        private AutomatonTree left;

        private AutomatonTree right;

        private Automaton state;
        private char value;

        public Automaton Find(char c, bool lowerCase)
        {
            if (lowerCase)
            {
                c = char.ToLower(c);
            }
            var flag = this.value == '\0' || this.value == c;
            Automaton find;
            if (flag)
            {
                find = this.state;
            }
            else
            {
                var flag2 = this.value > c;
                find = flag2 ? this.left.Find(c, false) : this.right.Find(c, false);
            }
            return find;
        }

        public void Add(char c, bool lowerCase, Automaton state)
        {
            if (lowerCase)
            {
                c = char.ToLower(c);
            }
            var flag = this.value == '\0';
            if (flag)
            {
                this.value = c;
                this.state = state;
                this.left = new AutomatonTree();
                this.right = new AutomatonTree();
            }
            else
            {
                var flag2 = this.value > c;
                if (flag2)
                {
                    this.left.Add(c, false, state);
                }
                else
                {
                    this.right.Add(c, false, state);
                }
            }
        }
    }
}