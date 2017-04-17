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
    using System.Collections;
    using System.IO;
    using Microsoft.VisualBasic.CompilerServices;

    internal class RepeatElement : Element
    {
        public enum RepeatType
        {
            Greedy = 1,
            Reluctant,
            Possessive
        }

        private readonly Element element;

        private readonly int max;

        private readonly int min;

        private readonly RepeatType repeatType;

        private BitArray matches;

        private int matchStart;

        public RepeatElement(Element element, int min, int max, RepeatType repeatType)
        {
            this.element = element;
            this.min = min;
            var flag = max <= 0;
            this.max = flag ? 2147483647 : max;
            this.repeatType = repeatType;
            this.matchStart = -1;
        }

        public override object Clone()
        {
            return new RepeatElement((Element) this.element.Clone(), this.min, this.max, this.repeatType);
        }

        public override int Match(Matcher m, LookAheadReader input, int start, int skip)
        {
            var flag = skip == 0;
            if (flag)
            {
                this.matchStart = -1;
                this.matches = null;
            }
            int match;
            switch (this.repeatType)
            {
                case RepeatType.Greedy:
                    match = this.MatchGreedy(m, input, start, skip);
                    return match;
                case RepeatType.Reluctant:
                    match = this.MatchReluctant(m, input, start, skip);
                    return match;
                case RepeatType.Possessive:
                {
                    var flag2 = skip == 0;
                    if (flag2)
                    {
                        match = this.MatchPossessive(m, input, start, 0);
                        return match;
                    }
                    break;
                }
            }
            match = -1;
            return match;
        }

        private int MatchGreedy(Matcher m, LookAheadReader input, int start, int skip)
        {
            var flag = skip == 0;
            int matchGreedy;
            if (flag)
            {
                matchGreedy = this.MatchPossessive(m, input, start, 0);
            }
            else
            {
                var flag2 = this.matchStart != start;
                if (flag2)
                {
                    this.matchStart = start;
                    this.matches = new BitArray(10);
                    this.FindMatches(m, input, start, 0, 0, 0);
                }
                var num = this.matches.Count - 1;
                for (var i = num; i >= 0; i += -1)
                {
                    var flag3 = this.matches[i];
                    if (flag3)
                    {
                        var flag4 = skip == 0;
                        if (flag4)
                        {
                            matchGreedy = i;
                            return matchGreedy;
                        }
                        skip--;
                    }
                }
                matchGreedy = -1;
            }
            return matchGreedy;
        }

        private int MatchReluctant(Matcher m, LookAheadReader input, int start, int skip)
        {
            var flag = this.matchStart != start;
            if (flag)
            {
                this.matchStart = start;
                this.matches = new BitArray(10);
                this.FindMatches(m, input, start, 0, 0, 0);
            }
            var num = this.matches.Count - 1;
            int matchReluctant;
            for (var i = 0; i <= num; i++)
            {
                var flag2 = this.matches[i];
                if (flag2)
                {
                    var flag3 = skip == 0;
                    if (flag3)
                    {
                        matchReluctant = i;
                        return matchReluctant;
                    }
                    skip--;
                }
            }
            matchReluctant = -1;
            return matchReluctant;
        }

        private int MatchPossessive(Matcher m, LookAheadReader input, int start, int count)
        {
            var length = 0;
            var subLength = 1;
            while (subLength > 0 && count < this.max)
            {
                subLength = this.element.Match(m, input, start + length, 0);
                var flag = subLength >= 0;
                if (flag)
                {
                    count++;
                    length += subLength;
                }
            }
            var flag2 = this.min <= count && count <= this.max;
            int matchPossessive;
            if (flag2)
            {
                matchPossessive = length;
            }
            else
            {
                matchPossessive = -1;
            }
            return matchPossessive;
        }

        private void FindMatches(Matcher m, LookAheadReader input, int start, int length, int count, int attempt)
        {
            var flag = count > this.max;
            if (!flag)
            {
                var flag2 = this.min <= count && attempt == 0;
                if (flag2)
                {
                    var flag3 = this.matches.Length <= length;
                    if (flag3)
                    {
                        this.matches.Length = length + 10;
                    }
                    this.matches[length] = true;
                }
                var subLength = this.element.Match(m, input, start, attempt);
                var flag4 = subLength < 0;
                if (!flag4)
                {
                    var flag5 = subLength == 0;
                    if (flag5)
                    {
                        var flag6 = this.min == count + 1;
                        if (flag6)
                        {
                            var flag7 = this.matches.Length <= length;
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
            output.Write(Conversions.ToDouble(indent + "Repeat (") + this.min + Conversions.ToDouble(",") + this.max +
                Conversions.ToDouble(")"));
            var flag = this.repeatType == RepeatType.Reluctant;
            if (flag)
            {
                output.Write("?");
            }
            else
            {
                var flag2 = this.repeatType == RepeatType.Possessive;
                if (flag2)
                {
                    output.Write("+");
                }
            }
            output.WriteLine();
            this.element.PrintTo(output, indent + " ");
        }
    }
}