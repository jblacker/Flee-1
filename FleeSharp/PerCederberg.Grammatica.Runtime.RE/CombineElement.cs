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

namespace Flee.PerCederberg.Grammatica.Runtime.RE
{
    using System.IO;

    internal class CombineElement : Element
    {
        private readonly Element elem1;

        private readonly Element elem2;

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
            var length = -1;
            var length2 = 0;
            var skip2 = 0;
            var skip3 = 0;
            int match;
            while (skip >= 0)
            {
                length = this.elem1.Match(m, input, start, skip2);
                var flag = length < 0;
                if (flag)
                {
                    match = -1;
                    return match;
                }
                length2 = this.elem2.Match(m, input, start + length, skip3);
                var flag2 = length2 < 0;
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
            match = length + length2;
            return match;
        }

        public override void PrintTo(TextWriter output, string indent)
        {
            this.elem1.PrintTo(output, indent);
            this.elem2.PrintTo(output, indent);
        }
    }
}