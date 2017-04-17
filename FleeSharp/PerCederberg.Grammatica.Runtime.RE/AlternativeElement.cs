namespace Flee.PerCederberg.Grammatica.Runtime.RE
{
    using System.IO;

    internal class AlternativeElement : Element
	{
		private readonly Element elem1;

		private readonly Element elem2;

		public AlternativeElement(Element first, Element second)
		{
			this.elem1 = first;
			this.elem2 = second;
		}

		public override object Clone()
		{
			return new AlternativeElement(this.elem1, this.elem2);
		}

		public override int Match(Matcher m, LookAheadReader input, int start, int skip)
		{
			int length = 0;
			int skip2 = 0;
			int skip3 = 0;
			while (length >= 0 && skip2 + skip3 <= skip)
			{
				int length2 = this.elem1.Match(m, input, start, skip2);
				int length3 = this.elem2.Match(m, input, start, skip3);
				bool flag = length2 >= length3;
				if (flag)
				{
					length = length2;
					skip2++;
				}
				else
				{
					length = length3;
					skip3++;
				}
			}
			return length;
		}

		public override void PrintTo(TextWriter output, string indent)
		{
			output.WriteLine(indent + "Alternative 1");
			this.elem1.PrintTo(output, indent + " ");
			output.WriteLine(indent + "Alternative 2");
			this.elem2.PrintTo(output, indent + " ");
		}
	}
}
