using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.IO;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class RepeatElement : Element
	{
		public enum RepeatType
		{
			GREEDY = 1,
			RELUCTANT,
			POSSESSIVE
		}

		private Element elem;

		private int min;

		private int max;

		private RepeatElement.RepeatType type;

		private int matchStart;

		private BitArray matches;

		public RepeatElement(Element elem, int min, int max, RepeatElement.RepeatType type)
		{
			this.elem = elem;
			this.min = min;
			bool flag = max <= 0;
			if (flag)
			{
				this.max = 2147483647;
			}
			else
			{
				this.max = max;
			}
			this.type = type;
			this.matchStart = -1;
		}

		public override object Clone()
		{
			return new RepeatElement((Element)this.elem.Clone(), this.min, this.max, this.type);
		}

		public override int Match(Matcher m, LookAheadReader input, int start, int skip)
		{
			bool flag = skip == 0;
			if (flag)
			{
				this.matchStart = -1;
				this.matches = null;
			}
			int Match;
			switch (this.type)
			{
			case RepeatElement.RepeatType.GREEDY:
				Match = this.MatchGreedy(m, input, start, skip);
				return Match;
			case RepeatElement.RepeatType.RELUCTANT:
				Match = this.MatchReluctant(m, input, start, skip);
				return Match;
			case RepeatElement.RepeatType.POSSESSIVE:
			{
				bool flag2 = skip == 0;
				if (flag2)
				{
					Match = this.MatchPossessive(m, input, start, 0);
					return Match;
				}
				break;
			}
			}
			Match = -1;
			return Match;
		}

		private int MatchGreedy(Matcher m, LookAheadReader input, int start, int skip)
		{
			bool flag = skip == 0;
			int MatchGreedy;
			if (flag)
			{
				MatchGreedy = this.MatchPossessive(m, input, start, 0);
			}
			else
			{
				bool flag2 = this.matchStart != start;
				if (flag2)
				{
					this.matchStart = start;
					this.matches = new BitArray(10);
					this.FindMatches(m, input, start, 0, 0, 0);
				}
				int num = this.matches.Count - 1;
				for (int i = num; i >= 0; i += -1)
				{
					bool flag3 = this.matches[i];
					if (flag3)
					{
						bool flag4 = skip == 0;
						if (flag4)
						{
							MatchGreedy = i;
							return MatchGreedy;
						}
						skip--;
					}
				}
				MatchGreedy = -1;
			}
			return MatchGreedy;
		}

		private int MatchReluctant(Matcher m, LookAheadReader input, int start, int skip)
		{
			bool flag = this.matchStart != start;
			if (flag)
			{
				this.matchStart = start;
				this.matches = new BitArray(10);
				this.FindMatches(m, input, start, 0, 0, 0);
			}
			int num = this.matches.Count - 1;
			int MatchReluctant;
			for (int i = 0; i <= num; i++)
			{
				bool flag2 = this.matches[i];
				if (flag2)
				{
					bool flag3 = skip == 0;
					if (flag3)
					{
						MatchReluctant = i;
						return MatchReluctant;
					}
					skip--;
				}
			}
			MatchReluctant = -1;
			return MatchReluctant;
		}

		private int MatchPossessive(Matcher m, LookAheadReader input, int start, int count)
		{
			int length = 0;
			int subLength = 1;
			while (subLength > 0 && count < this.max)
			{
				subLength = this.elem.Match(m, input, start + length, 0);
				bool flag = subLength >= 0;
				if (flag)
				{
					count++;
					length += subLength;
				}
			}
			bool flag2 = this.min <= count && count <= this.max;
			int MatchPossessive;
			if (flag2)
			{
				MatchPossessive = length;
			}
			else
			{
				MatchPossessive = -1;
			}
			return MatchPossessive;
		}

		private void FindMatches(Matcher m, LookAheadReader input, int start, int length, int count, int attempt)
		{
			bool flag = count > this.max;
			if (!flag)
			{
				bool flag2 = this.min <= count && attempt == 0;
				if (flag2)
				{
					bool flag3 = this.matches.Length <= length;
					if (flag3)
					{
						this.matches.Length = length + 10;
					}
					this.matches[length] = true;
				}
				int subLength = this.elem.Match(m, input, start, attempt);
				bool flag4 = subLength < 0;
				if (!flag4)
				{
					bool flag5 = subLength == 0;
					if (flag5)
					{
						bool flag6 = this.min == count + 1;
						if (flag6)
						{
							bool flag7 = this.matches.Length <= length;
							if (flag7)
							{
								this.matches.Length = length + 10;
							}
							this.matches[length] = true;
						}
					}
					else
					{
						this.FindMatches(m, input, start, length, count, attempt + 1);
						this.FindMatches(m, input, start + subLength, length + subLength, count + 1, 0);
					}
				}
			}
		}

		public override void PrintTo(TextWriter output, string indent)
		{
			output.Write(Conversions.ToDouble(indent + "Repeat (") + (double)this.min + Conversions.ToDouble(",") + (double)this.max + Conversions.ToDouble(")"));
			bool flag = this.type == RepeatElement.RepeatType.RELUCTANT;
			if (flag)
			{
				output.Write("?");
			}
			else
			{
				bool flag2 = this.type == RepeatElement.RepeatType.POSSESSIVE;
				if (flag2)
				{
					output.Write("+");
				}
			}
			output.WriteLine();
			this.elem.PrintTo(output, indent + " ");
		}
	}
}
