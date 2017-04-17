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


#pragma warning disable 659

namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;

    internal class LookAheadSet
    {
        private readonly ArrayList elements;

        private readonly int maxLength;

        public LookAheadSet(int maxLength)
        {
            this.elements = new ArrayList();
            this.maxLength = maxLength;
        }

        public LookAheadSet(int maxLength, LookAheadSet set)
            : this(maxLength)
        {
            this.AddAll(set);
        }

        public int Size()
        {
            return this.elements.Count;
        }

        public int GetMinLength()
        {
            var min = -1;
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = min < 0 || seq.Length() < min;
                if (flag)
                {
                    min = seq.Length();
                }
            }
            return Conversions.ToInteger(Interaction.IIf(min < 0, 0, min));
        }

        public int GetMaxLength()
        {
            var max = 0;
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = seq.Length() > max;
                if (flag)
                {
                    max = seq.Length();
                }
            }
            return max;
        }

        public int[] GetInitialTokens()
        {
            var list = new ArrayList();
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var token = RuntimeHelpers.GetObjectValue(((Sequence) this.elements[i]).GetToken(0));
                var flag = token != null && !list.Contains(RuntimeHelpers.GetObjectValue(token));
                if (flag)
                {
                    list.Add(RuntimeHelpers.GetObjectValue(token));
                }
            }
            var result = new int[list.Count - 1 + 1];
            var num2 = list.Count - 1;
            for (var i = 0; i <= num2; i++)
            {
                result[i] = Conversions.ToInteger(list[i]);
            }
            return result;
        }

        public bool IsRepetitive()
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = seq.IsRepetitive();
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNext(Parser parser)
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = seq.IsNext(parser);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNext(Parser parser, int length)
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = seq.IsNext(parser, length);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOverlap(LookAheadSet set)
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = set.IsOverlap((Sequence) this.elements[i]);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOverlap(Sequence seq)
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var elem = (Sequence) this.elements[i];
                var flag = seq.StartsWith(elem) || elem.StartsWith(seq);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        private bool Contains(Sequence elem)
        {
            return this.FindSequence(elem) != null;
        }

        public bool Intersects(LookAheadSet set)
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = set.Contains((Sequence) this.elements[i]);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        private Sequence FindSequence(Sequence elem)
        {
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = this.elements[i].Equals(elem);
                if (flag)
                {
                    var findSequence = (Sequence) this.elements[i];
                    return findSequence;
                }
            }
            return null;
        }

        private void Add(Sequence seq)
        {
            var flag = seq.Length() > this.maxLength;
            if (flag)
            {
                seq = new Sequence(this.maxLength, seq);
            }
            var flag2 = !this.Contains(seq);
            if (flag2)
            {
                this.elements.Add(seq);
            }
        }

        public void Add(int token)
        {
            this.Add(new Sequence(false, token));
        }

        public void AddAll(LookAheadSet set)
        {
            var num = set.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                this.Add((Sequence) set.elements[i]);
            }
        }

        public void AddEmpty()
        {
            this.Add(new Sequence());
        }

        private void Remove(Sequence seq)
        {
            this.elements.Remove(seq);
        }

        public void RemoveAll(LookAheadSet set)
        {
            var num = set.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                this.Remove((Sequence) set.elements[i]);
            }
        }

        public LookAheadSet CreateNextSet(int token)
        {
            var result = new LookAheadSet(this.maxLength - 1);
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var value = RuntimeHelpers.GetObjectValue(seq.GetToken(0));
                var flag = value != null && token == Conversions.ToInteger(value);
                if (flag)
                {
                    result.Add(seq.Subsequence(1));
                }
            }
            return result;
        }

        public LookAheadSet CreateIntersection(LookAheadSet set)
        {
            var result = new LookAheadSet(this.maxLength);
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var seq2 = set.FindSequence(seq);
                var flag = seq2 != null && seq.IsRepetitive();
                if (flag)
                {
                    result.Add(seq2);
                }
                else
                {
                    var flag2 = seq2 != null;
                    if (flag2)
                    {
                        result.Add(seq);
                    }
                }
            }
            return result;
        }

        public LookAheadSet CreateCombination(LookAheadSet set)
        {
            var result = new LookAheadSet(this.maxLength);
            var flag = this.Size() <= 0;
            LookAheadSet createCombination;
            if (flag)
            {
                createCombination = set;
            }
            else
            {
                var flag2 = set.Size() <= 0;
                if (flag2)
                {
                    createCombination = this;
                }
                else
                {
                    var num = this.elements.Count - 1;
                    for (var i = 0; i <= num; i++)
                    {
                        var first = (Sequence) this.elements[i];
                        var flag3 = first.Length() >= this.maxLength;
                        if (flag3)
                        {
                            result.Add(first);
                        }
                        else
                        {
                            var flag4 = first.Length() <= 0;
                            if (flag4)
                            {
                                result.AddAll(set);
                            }
                            else
                            {
                                var num2 = set.elements.Count - 1;
                                for (var j = 0; j <= num2; j++)
                                {
                                    var second = (Sequence) set.elements[j];
                                    result.Add(first.Concat(this.maxLength, second));
                                }
                            }
                        }
                    }
                    createCombination = result;
                }
            }
            return createCombination;
        }

        public LookAheadSet CreateOverlaps(LookAheadSet set)
        {
            var result = new LookAheadSet(this.maxLength);
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = set.IsOverlap(seq);
                if (flag)
                {
                    result.Add(seq);
                }
            }
            return result;
        }

        public LookAheadSet CreateFilter(LookAheadSet set)
        {
            var result = new LookAheadSet(this.maxLength);
            var flag = this.Size() <= 0 || set.Size() <= 0;
            LookAheadSet createFilter;
            if (flag)
            {
                createFilter = this;
            }
            else
            {
                var num = this.elements.Count - 1;
                for (var i = 0; i <= num; i++)
                {
                    var first = (Sequence) this.elements[i];
                    var num2 = set.elements.Count - 1;
                    for (var j = 0; j <= num2; j++)
                    {
                        var second = (Sequence) set.elements[j];
                        var flag2 = first.StartsWith(second);
                        if (flag2)
                        {
                            result.Add(first.Subsequence(second.Length()));
                        }
                    }
                }
                createFilter = result;
            }
            return createFilter;
        }

        public LookAheadSet CreateRepetitive()
        {
            var result = new LookAheadSet(this.maxLength);
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                var flag = seq.IsRepetitive();
                result.Add(flag ? seq : new Sequence(true, seq));
            }
            return result;
        }

        public override string ToString()
        {
            return this.ToString(null);
        }

        public string ToString(Tokenizer tokenizer)
        {
            var buffer = new StringBuilder();
            buffer.Append("{");
            var num = this.elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var seq = (Sequence) this.elements[i];
                buffer.Append("\n ");
                buffer.Append(seq.ToString(tokenizer));
            }
            buffer.Append("\n}");
            return buffer.ToString();
        }
#pragma warning disable 660,661
        private class Sequence : IEquatable<Sequence>
#pragma warning restore 660,661
        {
            private readonly ArrayList tokens;
            private bool repeat;

            public Sequence()
            {
                this.tokens = new ArrayList(0);
            }

            public Sequence(bool repeat, int token)
            {
                this.repeat = repeat;
                this.tokens = new ArrayList(1) {token};
            }

            public Sequence(int length, Sequence seq)
            {
                this.repeat = seq.repeat;
                this.tokens = new ArrayList(length);
                var flag = seq.Length() < length;
                if (flag)
                {
                    length = seq.Length();
                }
                var num = length - 1;
                for (var i = 0; i <= num; i++)
                {
                    this.tokens.Add(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
                }
            }

            public Sequence(bool repeat, Sequence seq)
            {
                this.repeat = repeat;
                this.tokens = seq.tokens;
            }

            public bool Equals(Sequence seq)
            {
                if (seq == null)
                {
                    return false;
                }
                if (this.tokens.Count != seq.tokens.Count)
                {
                    return false;
                }
                var num = this.tokens.Count - 1;
                for (var i = 0; i <= num; i++)
                {
                    var flag2 = !this.tokens[i].Equals(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
                    if (flag2)
                    {
                        return false;
                    }
                }
                return true;
            }

            public int Length()
            {
                return this.tokens.Count;
            }

            public object GetToken(int pos)
            {
                var flag = pos >= 0 && pos < this.tokens.Count;
                var getToken = flag ? this.tokens[pos] : null;
                return getToken;
            }

            public override bool Equals(object obj)
            {
                var a = obj as Sequence;
                if (a != null)
                {
                    return this.Equals(a);
                }
                return false;
            }

            /// <summary>Returns a value that indicates whether the values of two <see cref="T:Flee.PerCederberg.Grammatica.Runtime.LookAheadSet.Sequence" /> objects are equal.</summary>
            /// <param name="left">The first value to compare.</param>
            /// <param name="right">The second value to compare.</param>
            /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
            public static bool operator ==(Sequence left, Sequence right)
            {
                return Equals(left, right);
            }

            /// <summary>Returns a value that indicates whether two <see cref="T:Flee.PerCederberg.Grammatica.Runtime.LookAheadSet.Sequence" /> objects have different values.</summary>
            /// <param name="left">The first value to compare.</param>
            /// <param name="right">The second value to compare.</param>
            /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
            public static bool operator !=(Sequence left, Sequence right)
            {
                return !Equals(left, right);
            }

            public bool StartsWith(Sequence seq)
            {
                var flag = this.Length() < seq.Length();
                bool startsWith;
                if (flag)
                {
                    startsWith = false;
                }
                else
                {
                    var num = seq.tokens.Count - 1;
                    for (var i = 0; i <= num; i++)
                    {
                        var flag2 = !this.tokens[i].Equals(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
                        if (flag2)
                        {
                            return false;
                        }
                    }
                    startsWith = true;
                }
                return startsWith;
            }

            public bool IsRepetitive()
            {
                return this.repeat;
            }

            public bool IsNext(Parser parser)
            {
                var num = this.tokens.Count - 1;
                for (var i = 0; i <= num; i++)
                {
                    var id = Conversions.ToInteger(this.tokens[i]);
                    var token = parser.PeekToken(i);
                    var flag = token == null || token.Id != id;
                    if (flag)
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool IsNext(Parser parser, int length)
            {
                var flag = length > this.tokens.Count;
                if (flag)
                {
                    length = this.tokens.Count;
                }
                var num = length - 1;
                for (var i = 0; i <= num; i++)
                {
                    var id = Conversions.ToInteger(this.tokens[i]);
                    var token = parser.PeekToken(i);
                    var flag2 = token == null || token.Id != id;
                    if (flag2)
                    {
                        return false;
                    }
                }
                return true;
            }

            public override string ToString()
            {
                return this.ToString(null);
            }

            public string ToString(Tokenizer tokenizer)
            {
                var buffer = new StringBuilder();
                var flag = tokenizer == null;
                if (flag)
                {
                    buffer.Append(this.tokens);
                }
                else
                {
                    buffer.Append("[");
                    var num = this.tokens.Count - 1;
                    for (var i = 0; i <= num; i++)
                    {
                        var id = Conversions.ToInteger(this.tokens[i]);
                        var str = tokenizer.GetPatternDescription(id);
                        var flag2 = i > 0;
                        if (flag2)
                        {
                            buffer.Append(" ");
                        }
                        buffer.Append(str);
                    }
                    buffer.Append("]");
                }
                var flag3 = this.repeat;
                if (flag3)
                {
                    buffer.Append(" *");
                }
                return buffer.ToString();
            }

            public Sequence Concat(int length, Sequence seq)
            {
                var res = new Sequence(length, this);
                var flag = seq.repeat;
                if (flag)
                {
                    res.repeat = true;
                }
                length -= this.Length();
                var flag2 = length > seq.Length();
                if (flag2)
                {
                    res.tokens.AddRange(seq.tokens);
                }
                else
                {
                    var num = length - 1;
                    for (var i = 0; i <= num; i++)
                    {
                        res.tokens.Add(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
                    }
                }
                return res;
            }

            public Sequence Subsequence(int start)
            {
                var res = new Sequence(this.Length(), this);
                while (start > 0 && res.tokens.Count > 0)
                {
                    res.tokens.RemoveAt(0);
                    start--;
                }
                return res;
            }
        }
    }
}