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
    using System.Runtime.CompilerServices;
    using System.Text;
    using Exceptions;

    internal class ProductionPattern
    {
        private readonly ArrayList alternatives;

        private int defaultAlt;

        public ProductionPattern(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.alternatives = new ArrayList();
            this.defaultAlt = -1;
        }

        public int GetId()
        {
            return this.Id;
        }

        public string GetName()
        {
            return this.Name;
        }

        public bool IsSyntetic()
        {
            return this.Synthetic;
        }

        public void SetSyntetic(bool synthetic)
        {
            this.Synthetic = synthetic;
        }

        public int GetAlternativeCount()
        {
            return this.Count;
        }

        public ProductionPatternAlternative GetAlternative(int pos)
        {
            return this[pos];
        }

        public bool IsLeftRecursive()
        {
            var num = this.alternatives.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var alt = (ProductionPatternAlternative) this.alternatives[i];
                var flag = alt.IsLeftRecursive();
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsRightRecursive()
        {
            var num = this.alternatives.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var alt = (ProductionPatternAlternative) this.alternatives[i];
                var flag = alt.IsRightRecursive();
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMatchingEmpty()
        {
            var num = this.alternatives.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var alt = (ProductionPatternAlternative) this.alternatives[i];
                var flag = alt.IsMatchingEmpty();
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddAlternative(ProductionPatternAlternative alt)
        {
            var flag = this.alternatives.Contains(alt);
            if (flag)
            {
                throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, this.Name,
                    "two identical alternatives exist");
            }
            alt.SetPattern(this);
            this.alternatives.Add(alt);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            var indent = new StringBuilder();
            buffer.Append(this.Name);
            buffer.Append("(");
            buffer.Append(this.Id);
            buffer.Append(") ");
            var num = buffer.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                indent.Append(" ");
            }
            var num2 = this.alternatives.Count - 1;
            for (var i = 0; i <= num2; i++)
            {
                var flag = i == 0;
                if (flag)
                {
                    buffer.Append("= ");
                }
                else
                {
                    buffer.Append("\n");
                    buffer.Append(indent);
                    buffer.Append("| ");
                }
                buffer.Append(RuntimeHelpers.GetObjectValue(this.alternatives[i]));
            }
            return buffer.ToString();
        }

        public int Count => this.alternatives.Count;

        internal ProductionPatternAlternative DefaultAlternative
        {
            get
            {
                var flag = this.defaultAlt >= 0;
                ProductionPatternAlternative defaultAlternatives;
                if (flag)
                {
                    var obj = RuntimeHelpers.GetObjectValue(this.alternatives[this.defaultAlt]);
                    defaultAlternatives = (ProductionPatternAlternative) obj;
                }
                else
                {
                    defaultAlternatives = null;
                }
                return defaultAlternatives;
            }
            set
            {
                this.defaultAlt = 0;
                var num = this.alternatives.Count - 1;
                for (var i = 0; i <= num; i++)
                {
                    var flag = Equals(this.alternatives[i], value);
                    if (flag)
                    {
                        this.defaultAlt = i;
                    }
                }
            }
        }

        public int Id { get; set; }

        public ProductionPatternAlternative this[int index] => (ProductionPatternAlternative) this.alternatives[index];

        internal LookAheadSet LookAhead { get; set; }

        public string Name { get; set; }

        public bool Synthetic { get; set; }
    }
}