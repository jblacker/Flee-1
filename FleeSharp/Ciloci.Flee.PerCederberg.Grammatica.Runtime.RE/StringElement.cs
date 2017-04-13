using System;
using System.IO;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class StringElement : Element
	{
		private string value;

		public StringElement(char c) : this(c.ToString())
		{
		}

		public StringElement(string str)
		{
			this.value = str;
		}

		public string GetString()
		{
			return this.value;
		}

		public override object Clone()
		{
			return this;
		}

		public override int Match(Matcher m, LookAheadReader input, int start, int skip)
		{
			bool flag = skip != 0;
			int Match;
			if (flag)
			{
				Match = -1;
			}
			else
			{
				int num = this.value.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					int c = input.Peek(start + i);
					bool flag2 = c < 0;
					if (flag2)
					{
						m.SetReadEndOfString();
						Match = -1;
						return Match;
					}
					bool flag3 = m.IsCaseInsensitive();
					if (flag3)
					{
						c = Convert.ToInt32(char.ToLower(Convert.ToChar(c)));
					}
					bool flag4 = c != Convert.ToInt32(this.value[i]);
					if (flag4)
					{
						Match = -1;
						return Match;
					}
				}
				Match = this.value.Length;
			}
			return Match;
		}

		public override void PrintTo(TextWriter output, string indent)
		{
			output.WriteLine(indent + "'" + this.value + "'");
		}
	}
}
