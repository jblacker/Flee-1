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
    using System.Collections;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Production : Node
    {
        private readonly ArrayList children;

        public Production(ProductionPattern pattern)
        {
            this.Pattern = pattern;
            this.children = new ArrayList();
        }

        public void AddChild(Node child)
        {
            var flag = child != null;
            if (flag)
            {
                child.SetParent(this);
                this.children.Add(child);
            }
        }

        public ProductionPattern GetPattern()
        {
            return this.Pattern;
        }

        internal override bool IsHidden()
        {
            return this.Pattern.Synthetic;
        }

        public override string ToString()
        {
            return this.Pattern.Name + "(" + Conversions.ToString(this.Pattern.Id) + ")";
        }

        public override int Count => this.children.Count;

        public override int Id => this.Pattern.Id;

        public override Node this[int index]
        {
            get
            {
                var flag = index < 0 || index >= this.children.Count;
                Node item;
                if (flag)
                {
                    item = null;
                }
                else
                {
                    item = (Node) this.children[index];
                }
                return item;
            }
        }

        public override string Name => this.Pattern.Name;

        public ProductionPattern Pattern { get; }
    }
}