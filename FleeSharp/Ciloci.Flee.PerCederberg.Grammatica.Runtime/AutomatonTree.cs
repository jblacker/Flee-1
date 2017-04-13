using System;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
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
			Automaton Find;
			if (flag)
			{
				Find = this.state;
			}
			else
			{
				bool flag2 = this.value > c;
				if (flag2)
				{
					Find = this.left.Find(c, false);
				}
				else
				{
					Find = this.right.Find(c, false);
				}
			}
			return Find;
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
