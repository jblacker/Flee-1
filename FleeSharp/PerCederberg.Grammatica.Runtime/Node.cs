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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal abstract class Node
    {
        protected List<object> valuesArrayList;

        protected Node()
        {
            this.valuesArrayList = new List<object>();
        }

        internal virtual bool IsHidden()
        {
            return false;
        }

        public virtual int GetId()
        {
            return this.Id;
        }

        public virtual string GetName()
        {
            return this.Name;
        }

        public virtual int GetStartLine()
        {
            return this.StartLine;
        }

        public virtual int GetStartColumn()
        {
            return this.StartColumn;
        }

        public virtual int GetEndLine()
        {
            return this.EndLine;
        }

        public virtual int GetEndColumn()
        {
            return this.EndColumn;
        }

        public Node GetParent()
        {
            return this.Parent;
        }

        internal void SetParent(Node parent)
        {
            this.Parent = parent;
        }

        public virtual int GetChildCount()
        {
            return this.Count;
        }

        public int GetDescendantCount()
        {
            var count = 0;
            var num = count - 1;
            for (var i = 0; i <= num; i++)
            {
                count += 1 + this[i].GetDescendantCount();
            }
            return count;
        }

        public virtual Node GetChildAt(int index)
        {
            return this[index];
        }

        public int GetValueCount()
        {
            var flag = this.valuesArrayList == null;
            int getValueCount;
            if (flag)
            {
                getValueCount = 0;
            }
            else
            {
                getValueCount = this.valuesArrayList.Count;
            }
            return getValueCount;
        }

        public object GetValue(int pos)
        {
            return this.valuesArrayList[pos];
        }

        public ICollection GetAllValues()
        {
            return this.valuesArrayList;
        }

        public void AddValue(object value)
        {
            var flag = value != null;
            if (flag)
            {
                this.valuesArrayList.Add(RuntimeHelpers.GetObjectValue(value));
            }
        }

        public void AddValues(IEnumerable<object> values)
        {
            var flag = values != null;
            if (flag)
            {
                foreach (var o in values)
                {
                    this.valuesArrayList.Add(o);
                }
                //this.valuesArrayList.AddRange(values);
            }
        }

        public void RemoveAllValues()
        {
            this.valuesArrayList = null;
        }

        public void PrintTo(TextWriter output)
        {
            this.PrintTo(output, "");
            output.Flush();
        }

        private void PrintTo(TextWriter output, string indent)
        {
            output.WriteLine(indent + this);
            indent += " ";
            var num = this.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                this[i].PrintTo(output, indent);
            }
        }

        public virtual int Count => 0;

        public virtual int EndColumn
        {
            get
            {
                var num = this.Count - 1;
                int endColumn;
                for (var i = num; i >= 0; i += -1)
                {
                    var col = this[i].EndColumn;
                    var flag = col >= 0;
                    if (flag)
                    {
                        endColumn = col;
                        return endColumn;
                    }
                }
                endColumn = -1;
                return endColumn;
            }
        }

        public virtual int EndLine
        {
            get
            {
                var num = this.Count - 1;
                int endLine;
                for (var i = num; i >= 0; i += -1)
                {
                    var line = this[i].EndLine;
                    var flag = line >= 0;
                    if (flag)
                    {
                        endLine = line;
                        return endLine;
                    }
                }
                endLine = -1;
                return endLine;
            }
        }

        public abstract int Id { get; }

        public virtual Node this[int index] => null;

        public abstract string Name { get; }

        public Node Parent { get; private set; }

        public virtual int StartColumn
        {
            get
            {
                var num = this.Count - 1;
                int startColumn;
                for (var i = 0; i <= num; i++)
                {
                    var col = this[i].StartColumn;
                    var flag = col >= 0;
                    if (flag)
                    {
                        startColumn = col;
                        return startColumn;
                    }
                }
                startColumn = -1;
                return startColumn;
            }
        }

        public virtual int StartLine
        {
            get
            {
                var num = this.Count - 1;
                int startLIne;
                for (var i = 0; i <= num; i++)
                {
                    var line = this[i].StartLine;
                    var flag = line >= 0;
                    if (flag)
                    {
                        startLIne = line;
                        return startLIne;
                    }
                }
                startLIne = -1;
                return startLIne;
            }
        }

        public IList<object> ValuesArrayList
        {
            get
            {
                var flag = this.valuesArrayList == null;
                if (flag)
                {
                    this.valuesArrayList = new List<object>();
                }
                return this.valuesArrayList;
            }
            //set
            //{
            //	this.valuesArrayList = value;
            //}
        }
    }


    internal class NodeValue
    {
        private object t;

        public void SetValue<T>(T t)
        {
            this.t = t;
        }

        public T GetValue<T>()
        {
            return (T) this.t;
        }
    }
}