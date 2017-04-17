namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Extensions;

    internal abstract class Node
	{
		private Node parentNode;

		protected List<object> valuesArrayList;

		public abstract int Id { get; }

		public abstract string Name { get; }

		public virtual int StartLine
		{
			get
			{
				int num = this.Count - 1;
				int startLIne;
				for (int i = 0; i <= num; i++)
				{
					int line = this[i].StartLine;
					bool flag = line >= 0;
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

		public virtual int StartColumn
		{
			get
			{
				int num = this.Count - 1;
				int startColumn;
				for (int i = 0; i <= num; i++)
				{
					int col = this[i].StartColumn;
					bool flag = col >= 0;
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

		public virtual int EndLine
		{
			get
			{
				int num = this.Count - 1;
				int endLine;
				for (int i = num; i >= 0; i += -1)
				{
					int line = this[i].EndLine;
					bool flag = line >= 0;
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

		public virtual int EndColumn
		{
			get
			{
				int num = this.Count - 1;
				int endColumn;
				for (int i = num; i >= 0; i += -1)
				{
					int col = this[i].EndColumn;
					bool flag = col >= 0;
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

		public Node Parent => this.parentNode;

	    public virtual int Count => 0;

	    public virtual Node this[int index] => null;

	    public IList<object> ValuesArrayList
		{
			get
			{
				bool flag = this.valuesArrayList == null;
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
			this.parentNode = parent;
		}

		public virtual int GetChildCount()
		{
			return this.Count;
		}

		public int GetDescendantCount()
		{
			int count = 0;
			int num = count - 1;
			for (int i = 0; i <= num; i++)
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
			bool flag = this.valuesArrayList == null;
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
			bool flag = value != null;
			if (flag)
			{
				this.valuesArrayList.Add(RuntimeHelpers.GetObjectValue(value));
			}
		}

		public void AddValues(IEnumerable<object> values)
		{
			bool flag = values != null;
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
			output.WriteLine(indent + this.ToString());
			indent += " ";
			int num = this.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				this[i].PrintTo(output, indent);
			}
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
