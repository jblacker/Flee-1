// ReSharper disable RedundantCast
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

        private readonly int min;

        private readonly int max;

        private readonly RepeatType repeatType;

        private int matchStart;

        private BitArray matches;

        public RepeatElement(Element element, int min, int max, RepeatType repeatType)
        {
            this.element = element;
            this.min = min;
            bool flag = max <= 0;
            this.max = flag ? 2147483647 : max;
            this.repeatType = repeatType;
            this.matchStart = -1;
        }

        public override object Clone()
        {
            return new RepeatElement((Element)this.element.Clone(), this.min, this.max, this.repeatType);
        }

        public override int Match(Matcher m, LookAheadReader input, int start, int skip)
        {
            bool flag = skip == 0;
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
                        bool flag2 = skip == 0;
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
            bool flag = skip == 0;
            int matchGreedy;
            if (flag)
            {
                matchGreedy = this.MatchPossessive(m, input, start, 0);
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
            bool flag = this.matchStart != start;
            if (flag)
            {
                this.matchStart = start;
                this.matches = new BitArray(10);
                this.FindMatches(m, input, start, 0, 0, 0);
            }
            int num = this.matches.Count - 1;
            int matchReluctant;
            for (int i = 0; i <= num; i++)
            {
                bool flag2 = this.matches[i];
                if (flag2)
                {
                    bool flag3 = skip == 0;
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
            int length = 0;
            int subLength = 1;
            while (subLength > 0 && count < this.max)
            {
                subLength = this.element.Match(m, input, start + length, 0);
                bool flag = subLength >= 0;
                if (flag)
                {
                    count++;
                    length += subLength;
                }
            }
            bool flag2 = this.min <= count && count <= this.max;
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
                int subLength = this.element.Match(m, input, start, attempt);
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
            bool flag = this.repeatType == RepeatType.Reluctant;
            if (flag)
            {
                output.Write("?");
            }
            else
            {
                bool flag2 = this.repeatType == RepeatType.Possessive;
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
