using System;
using System.IO;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class CombineElement : Element
	{
		private Element elem1;

		private Element elem2;

		public CombineElement(Element first, Element second)
		{
			this.elem1 = first;
			this.elem2 = second;
		}

		public override object Clone()
		{
			return new CombineElement(this.elem1, this.elem2);
		}

		public override int Match(Matcher m, LookAheadReader input, int start, int skip)
		{
			int length = -1;
			int length2 = 0;
			int skip2 = 0;
			int skip3 = 0;
			int Match;
			while (skip >= 0)
			{
				length = this.elem1.Match(m, input, start, skip2);
				bool flag = length < 0;
				if (flag)
				{
					Match = -1;
					return Match;
				}
				length2 = this.elem2.Match(m, input, start + length, skip3);
				bool flag2 = length2 < 0;
				if (flag2)
				{
					skip2++;
					skip3 = 0;
				}
				else
				{
					skip3++;
					skip--;
				}
			}
			Match = length + length2;
			return Match;
		}

		public override void PrintTo(TextWriter output, string indent)
		{
			this.elem1.PrintTo(output, indent);
			this.elem2.PrintTo(output, indent);
		}
	}
}
