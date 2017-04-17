namespace Flee.PerCederberg.Grammatica.Runtime
{
    internal class AutomatonTree
	{
		private char value;

		private Automaton state;

		private AutomatonTree left;

		private AutomatonTree right;

		public Automaton Find(char c, bool lowerCase)
		{
			if (lowerCase)
			{
				c = char.ToLower(c);
			}
			bool flag = this.value == '\0' || this.value == c;
			Automaton find;
			if (flag)
			{
				find = this.state;
			}
			else
			{
			    bool flag2 = this.value > c;
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
			bool flag = this.value == '\0';
			if (flag)
			{
				this.value = c;
				this.state = state;
				this.left = new AutomatonTree();
				this.right = new AutomatonTree();
			}
			else
			{
				bool flag2 = this.value > c;
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
