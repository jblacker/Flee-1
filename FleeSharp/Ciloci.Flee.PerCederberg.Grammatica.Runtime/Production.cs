using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class Production : Node
	{
		private ProductionPattern m_pattern;

		private ArrayList children;

		public override int Id
		{
			get
			{
				return this.m_pattern.Id;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_pattern.Name;
			}
		}

		public override int Count
		{
			get
			{
				return this.children.Count;
			}
		}

		public override Node this[int index]
		{
			get
			{
				bool flag = index < 0 || index >= this.children.Count;
				Node Item;
				if (flag)
				{
					Item = null;
				}
				else
				{
					Item = (Node)this.children[index];
				}
				return Item;
			}
		}

		public ProductionPattern Pattern
		{
			get
			{
				return this.m_pattern;
			}
		}

		public Production(ProductionPattern pattern)
		{
			this.m_pattern = pattern;
			this.children = new ArrayList();
		}

		public void AddChild(Node child)
		{
			bool flag = child != null;
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
			return this.m_pattern.Synthetic;
		}

		public override string ToString()
		{
			return this.m_pattern.Name + "(" + Conversions.ToString(this.m_pattern.Id) + ")";
		}
	}
}
