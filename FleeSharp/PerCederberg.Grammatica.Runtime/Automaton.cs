namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualBasic;

    internal class Automaton
	{
		private object value;

		private readonly AutomatonTree tree;

		public Automaton()
		{
			this.tree = new AutomatonTree();
		}

		public void AddMatch(string str, bool caseInsensitive, object value)
		{
			var flag = str.Length == 0;
			if (flag)
			{
				this.value = RuntimeHelpers.GetObjectValue(value);
			}
			else
			{
				var state = this.tree.Find(str[0], caseInsensitive);
				var flag2 = state == null;
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
			var c = input.Peek(pos);
			var flag = this.tree != null && c >= 0;
			if (flag)
			{
				var state = this.tree.Find(Convert.ToChar(c), caseInsensitive);
				var flag2 = state != null;
				if (flag2)
				{
					result = RuntimeHelpers.GetObjectValue(state.MatchFrom(input, pos + 1, caseInsensitive));
				}
			}
			return Interaction.IIf(result == null, RuntimeHelpers.GetObjectValue(this.value), RuntimeHelpers.GetObjectValue(result));
		}
	}
}
