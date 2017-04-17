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
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;

    internal class CharacterSetElement : Element
    {
        public static CharacterSetElement DOT = new CharacterSetElement(false);

        public static CharacterSetElement DIGIT = new CharacterSetElement(false);

        public static CharacterSetElement NON_DIGIT = new CharacterSetElement(true);

        public static CharacterSetElement WHITESPACE = new CharacterSetElement(false);

        public static CharacterSetElement NON_WHITESPACE = new CharacterSetElement(true);

        public static CharacterSetElement WORD = new CharacterSetElement(false);

        public static CharacterSetElement NON_WORD = new CharacterSetElement(true);

        private readonly ArrayList contents;

        private readonly bool inverted;

        public CharacterSetElement(bool inverted)
        {
            this.contents = new ArrayList();
            this.inverted = inverted;
        }

        public void AddCharacter(char c)
        {
            this.contents.Add(c);
        }

        public void AddCharacters(string str)
        {
            var num = str.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                this.AddCharacter(str[i]);
            }
        }

        public void AddCharacters(StringElement elem)
        {
            this.AddCharacters(elem.GetString());
        }

        public void AddRange(char min, char max)
        {
            this.contents.Add(new Range(min, max));
        }

        public void AddCharacterSet(CharacterSetElement elem)
        {
            this.contents.Add(elem);
        }

        public override object Clone()
        {
            return this;
        }

        public override int Match(Matcher m, LookAheadReader input, int start, int skip)
        {
            var flag = skip != 0;
            int Match;
            if (flag)
            {
                Match = -1;
            }
            else
            {
                var c = input.Peek(start);
                var flag2 = c < 0;
                if (flag2)
                {
                    m.SetReadEndOfString();
                    Match = -1;
                }
                else
                {
                    var flag3 = m.IsCaseInsensitive();
                    if (flag3)
                    {
                        c = Convert.ToInt32(char.ToLower(Convert.ToChar(c)));
                    }
                    Match = Conversions.ToInteger(Interaction.IIf(this.InSet(Convert.ToChar(c)), 1, -1));
                }
            }
            return Match;
        }

        private bool InSet(char c)
        {
            var flag = this == DOT;
            bool InSet;
            if (flag)
            {
                InSet = this.InDotSet(c);
            }
            else
            {
                var flag2 = this == DIGIT || this == NON_DIGIT;
                if (flag2)
                {
                    InSet = this.InDigitSet(c) != this.inverted;
                }
                else
                {
                    var flag3 = this == WHITESPACE || this == NON_WHITESPACE;
                    if (flag3)
                    {
                        InSet = this.InWhitespaceSet(c) != this.inverted;
                    }
                    else
                    {
                        var flag4 = this == WORD || this == NON_WORD;
                        if (flag4)
                        {
                            InSet = this.InWordSet(c) != this.inverted;
                        }
                        else
                        {
                            InSet = this.InUserSet(c) != this.inverted;
                        }
                    }
                }
            }
            return InSet;
        }

        private bool InDotSet(char c)
        {
            if (c <= '\r')
            {
                if (c != '\n' && c != '\r')
                {
                    return true;
                }
            }
            else if (c != '\u0085' && c != '\u2028' && c != '\u2029')
            {
                return true;
            }
            return false;
        }

        private bool InDigitSet(char c)
        {
            return '0' <= c && c <= '9';
        }

        private bool InWhitespaceSet(char c)
        {
            switch (c)
            {
                case '\t':
                case '\n':
                case '\v':
                case '\f':
                case '\r':
                    break;
                default:
                    if (c != ' ')
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        private bool InWordSet(char c)
        {
            return 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' || '0' <= c && c <= '9' || c == '_';
        }

        private bool InUserSet(char value)
        {
            var num = this.contents.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var obj = RuntimeHelpers.GetObjectValue(this.contents[i]);
                var flag = obj is char;
                if (flag)
                {
                    var c = Conversions.ToChar(obj);
                    var flag2 = c == value;
                    if (flag2)
                    {
                        return true;
                    }
                }
                else
                {
                    var flag3 = obj is Range;
                    if (flag3)
                    {
                        var r = (Range) obj;
                        var flag4 = r.Inside(value);
                        if (flag4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        var flag5 = obj is CharacterSetElement;
                        if (flag5)
                        {
                            var e = (CharacterSetElement) obj;
                            var flag6 = e.InSet(value);
                            if (flag6)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override void PrintTo(TextWriter output, string indent)
        {
            output.WriteLine(indent + this);
        }

        public override string ToString()
        {
            var flag = this == DOT;
            string toString;
            if (flag)
            {
                toString = ".";
            }
            else
            {
                var flag2 = this == DIGIT;
                if (flag2)
                {
                    toString = "\\d";
                }
                else
                {
                    var flag3 = this == NON_DIGIT;
                    if (flag3)
                    {
                        toString = "\\D";
                    }
                    else
                    {
                        var flag4 = this == WHITESPACE;
                        if (flag4)
                        {
                            toString = "\\s";
                        }
                        else
                        {
                            var flag5 = this == NON_WHITESPACE;
                            if (flag5)
                            {
                                toString = "\\S";
                            }
                            else
                            {
                                var flag6 = this == WORD;
                                if (flag6)
                                {
                                    toString = "\\w";
                                }
                                else
                                {
                                    var flag7 = this == NON_WORD;
                                    if (flag7)
                                    {
                                        toString = "\\W";
                                    }
                                    else
                                    {
                                        var buffer = new StringBuilder();
                                        var flag8 = this.inverted;
                                        if (flag8)
                                        {
                                            buffer.Append("^[");
                                        }
                                        else
                                        {
                                            buffer.Append("[");
                                        }
                                        var num = this.contents.Count - 1;
                                        for (var i = 0; i <= num; i++)
                                        {
                                            buffer.Append(RuntimeHelpers.GetObjectValue(this.contents[i]));
                                        }
                                        buffer.Append("]");
                                        toString = buffer.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return toString;
        }

        private class Range
        {
            private readonly char max;
            private readonly char min;

            public Range(char min, char max)
            {
                this.min = min;
                this.max = max;
            }

            public bool Inside(char c)
            {
                return this.min <= c && c <= this.max;
            }

            public override string ToString()
            {
                return Conversions.ToString(this.min) + "-" + Conversions.ToString(this.max);
            }
        }
    }
}