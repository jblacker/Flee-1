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
    using System.Text;

    internal class ProductionPatternElement
    {
        private readonly bool token;

        public ProductionPatternElement(bool isToken, int id, int min, int max)
        {
            this.token = isToken;
            this.Id = id;
            var flag = min < 0;
            if (flag)
            {
                min = 0;
            }
            this.MinCount = min;
            var flag2 = max <= 0;
            if (flag2)
            {
                max = 2147483647;
            }
            else
            {
                var flag3 = max < min;
                if (flag3)
                {
                    max = min;
                }
            }
            this.MaxCount = max;
        }

        public int GetId()
        {
            return this.Id;
        }

        public int GetMinCount()
        {
            return this.MinCount;
        }

        public int GetMaxCount()
        {
            return this.MaxCount;
        }

        public bool IsToken()
        {
            return this.token;
        }

        public bool IsProduction()
        {
            return !this.token;
        }

        public bool IsMatch(Token token)
        {
            return this.IsToken() && token != null && token.Id == this.Id;
        }

        public override bool Equals(object obj)
        {
            var flag = obj is ProductionPatternElement;
            bool equals;
            if (flag)
            {
                var elem = (ProductionPatternElement) obj;
                equals = this.token == elem.token && this.Id == elem.Id && this.MinCount == elem.MinCount &&
                    this.MaxCount == elem.MaxCount;
            }
            else
            {
                equals = false;
            }
            return equals;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(this.Id);
            var flag = this.token;
            buffer.Append(flag ? "(Token)" : "(Production)");
            var flag2 = this.MinCount != 1 || this.MaxCount != 1;
            if (flag2)
            {
                buffer.Append("{");
                buffer.Append(this.MinCount);
                buffer.Append(",");
                buffer.Append(this.MaxCount);
                buffer.Append("}");
            }
            return buffer.ToString();
        }

        public int Id { get; }

        internal LookAheadSet LookAhead { get; set; }

        public int MaxCount { get; }

        public int MinCount { get; }
    }
}