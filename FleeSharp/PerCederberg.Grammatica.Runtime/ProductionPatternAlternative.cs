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
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class ProductionPatternAlternative
    {
        private readonly ArrayList elements;

        public ProductionPatternAlternative()
        {
            this.elements = new ArrayList();
        }

        public ProductionPattern GetPattern()
        {
            return this.Pattern;
        }

        public int GetElementCount()
        {
            return this.Count;
        }

        public ProductionPatternElement GetElement(int pos)
        {
            return this[pos];
        }

        public bool IsLeftRecursive()
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var elem = (ProductionPatternElement) this.elements[i];
                var flag = elem.Id == this.Pattern.Id;
                if (flag)
                {
                    return true;
                }
                var flag2 = elem.MinCount > 0;
                if (flag2)
                {
                    break;
                }
            }
            return false;
        }

        public bool IsRightRecursive()
        {
            var num = this.elements.Count - 1;
            for (var i = num; i >= 0; i += -1)
            {
                var elem = (ProductionPatternElement) this.elements[i];
                var flag = elem.Id == this.Pattern.Id;
                if (flag)
                {
                    return true;
                }
                var flag2 = elem.MinCount > 0;
                if (flag2)
                {
                    break;
                }
            }
            return false;
        }

        public bool IsMatchingEmpty()
        {
            return this.GetMinElementCount() == 0;
        }

        internal void SetPattern(ProductionPattern pattern)
        {
            this.Pattern = pattern;
        }

        public int GetMinElementCount()
        {
            var min = 0;
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var elem = (ProductionPatternElement) this.elements[i];
                min += elem.MinCount;
            }
            return min;
        }

        public int GetMaxElementCount()
        {
            var max = 0;
            var num = this.elements.Count - 1;
            int getMaxElementCount;
            for (var i = 0; i <= num; i++)
            {
                var elem = (ProductionPatternElement) this.elements[i];
                var flag = elem.MaxCount >= 2147483647;
                if (flag)
                {
                    getMaxElementCount = 2147483647;
                    return getMaxElementCount;
                }
                max += elem.MaxCount;
            }
            getMaxElementCount = max;
            return getMaxElementCount;
        }

        public void AddToken(int id, int min, int max)
        {
            this.AddElement(new ProductionPatternElement(true, id, min, max));
        }

        public void AddProduction(int id, int min, int max)
        {
            this.AddElement(new ProductionPatternElement(false, id, min, max));
        }

        public void AddElement(ProductionPatternElement elem)
        {
            this.elements.Add(elem);
        }

        public void AddElement(ProductionPatternElement elem, int min, int max)
        {
            var flag = elem.IsToken();
            if (flag)
            {
                this.AddToken(elem.Id, min, max);
            }
            else
            {
                this.AddProduction(elem.Id, min, max);
            }
        }

        public override bool Equals(object obj)
        {
            var flag = obj is ProductionPatternAlternative;
            return flag && this.Equals((ProductionPatternAlternative) obj);
        }

        public bool Equals(ProductionPatternAlternative alt)
        {
            var flag = this.elements.Count != alt.elements.Count;
            bool equals;
            if (flag)
            {
                equals = false;
            }
            else
            {
                var num = this.elements.Count - 1;
                for (var i = 0; i <= num; i++)
                {
                    var flag2 = !this.elements[i].Equals(RuntimeHelpers.GetObjectValue(alt.elements[i]));
                    if (flag2)
                    {
                        return false;
                    }
                }
                equals = true;
            }
            return equals;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = i > 0;
                if (flag)
                {
                    buffer.Append(" ");
                }
                buffer.Append(RuntimeHelpers.GetObjectValue(this.elements[i]));
            }
            return buffer.ToString();
        }

        public int Count => this.elements.Count;

        public ProductionPatternElement this[int index] => (ProductionPatternElement) this.elements[index];

        internal LookAheadSet LookAheadSet { get; set; }

        public ProductionPattern Pattern { get; private set; }
    }
}