using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal abstract class Node
	{
		private Node m_parent;

		private ArrayList m_values;

		public abstract int Id
		{
			get;
		}

		public abstract string Name
		{
			get;
		}

		public virtual int StartLine
		{
			get
			{
				int num = this.Count - 1;
				int StartLine;
				for (int i = 0; i <= num; i++)
				{
					int line = this[i].StartLine;
					bool flag = line >= 0;
					if (flag)
					{
						StartLine = line;
						return StartLine;
					}
				}
				StartLine = -1;
				return StartLine;
			}
		}

		public virtual int StartColumn
		{
			get
			{
				int num = this.Count - 1;
				int StartColumn;
				for (int i = 0; i <= num; i++)
				{
					int col = this[i].StartColumn;
					bool flag = col >= 0;
					if (flag)
					{
						StartColumn = col;
						return StartColumn;
					}
				}
				StartColumn = -1;
				return StartColumn;
			}
		}

		public virtual int EndLine
		{
			get
			{
				int num = this.Count - 1;
				int EndLine;
				for (int i = num; i >= 0; i += -1)
				{
					int line = this[i].EndLine;
					bool flag = line >= 0;
					if (flag)
					{
						EndLine = line;
						return EndLine;
					}
				}
				EndLine = -1;
				return EndLine;
			}
		}

		public virtual int EndColumn
		{
			get
			{
				int num = this.Count - 1;
				int EndColumn;
				for (int i = num; i >= 0; i += -1)
				{
					int col = this[i].EndColumn;
					bool flag = col >= 0;
					if (flag)
					{
						EndColumn = col;
						return EndColumn;
					}
				}
				EndColumn = -1;
				return EndColumn;
			}
		}

		public Node Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual Node this[int index]
		{
			get
			{
				return null;
			}
		}

		public ArrayList Values
		{
			get
			{
				bool flag = this.m_values == null;
				if (flag)
				{
					this.m_values = new ArrayList();
				}
				return this.m_values;
			}
			set
			{
				this.m_values = value;
			}
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
			this.m_parent = parent;
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
			bool flag = this.m_values == null;
			int GetValueCount;
			if (flag)
			{
				GetValueCount = 0;
			}
			else
			{
				GetValueCount = this.m_values.Count;
			}
			return GetValueCount;
		}

		public object GetValue(int pos)
		{
			return this.Values[pos];
		}

		public ArrayList GetAllValues()
		{
			return this.m_values;
		}

		public void AddValue(object value)
		{
			bool flag = value != null;
			if (flag)
			{
				this.Values.Add(RuntimeHelpers.GetObjectValue(value));
			}
		}

		public void AddValues(ArrayList values)
		{
			bool flag = values != null;
			if (flag)
			{
				this.Values.AddRange(values);
			}
		}

		public void RemoveAllValues()
		{
			this.m_values = null;
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
}
