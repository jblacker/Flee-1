namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Production : Node
	{
		private readonly ProductionPattern productionPattern;

		private readonly ArrayList children;

		public override int Id => this.productionPattern.Id;

	    public override string Name => this.productionPattern.Name;

	    public override int Count => this.children.Count;

	    public override Node this[int index]
		{
			get
			{
				bool flag = index < 0 || index >= this.children.Count;
				Node item;
				if (flag)
				{
					item = null;
				}
				else
				{
					item = (Node)this.children[index];
				}
				return item;
			}
		}

		public ProductionPattern Pattern => this.productionPattern;

	    public Production(ProductionPattern pattern)
		{
			this.productionPattern = pattern;
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
			return this.productionPattern.Synthetic;
		}

		public override string ToString()
		{
			return this.productionPattern.Name + "(" + Conversions.ToString(this.productionPattern.Id) + ")";
		}
	}
}
