// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp.PerCederberg.Grammatica.Runtime.RE
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
            var length = 0;
            var skip2 = 0;
            var skip3 = 0;
            while (length >= 0 && skip2 + skip3 <= skip)
            {
                var length2 = this.elem1.Match(m, input, start, skip2);
                var length3 = this.elem2.Match(m, input, start, skip3);
                var flag = length2 >= length3;
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