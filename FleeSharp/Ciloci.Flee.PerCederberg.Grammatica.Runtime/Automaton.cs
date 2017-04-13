using Microsoft.VisualBasic;
using System;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class Automaton
	{
		private object value;

		private AutomatonTree tree;

		public Automaton()
		{
			this.tree = new AutomatonTree();
		}

		public void AddMatch(string str, bool caseInsensitive, object value)
		{
			bool flag = str.Length == 0;
			if (flag)
			{
				this.value = RuntimeHelpers.GetObjectValue(value);
			}
			else
			{
				Automaton state = this.tree.Find(str[0], caseInsensitive);
				bool flag2 = state == null;
				if (flag2)
				{
					state = new Automaton();
					state.AddMatch(str.Substring(1), caseInsensitive, RuntimeHelpers.GetObjectValue(value));
					this.tree.Add(str[0], caseInsensitive, state);
				}
				else
				{
					state.AddMatch(str.Substring(1), caseInsensitive, RuntimeHelpers.GetObjectValue(value));
				}
			}
		}

		public object MatchFrom(LookAheadReader input, int pos, bool caseInsensitive)
		{
			object result = null;
			int c = input.Peek(pos);
			bool flag = this.tree != null && c >= 0;
			if (flag)
			{
				Automaton state = this.tree.Find(Convert.ToChar(c), caseInsensitive);
				bool flag2 = state != null;
				if (flag2)
				{
					result = RuntimeHelpers.GetObjectValue(state.MatchFrom(input, pos + 1, caseInsensitive));
				}
			}
			return Interaction.IIf(result == null, RuntimeHelpers.GetObjectValue(this.value), RuntimeHelpers.GetObjectValue(result));
		}
	}
}
