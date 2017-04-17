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
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Exceptions;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;

    internal class RegExp
    {
        private readonly Element element;

        private readonly bool ignoreCase;

        private readonly string pattern;

        private int pos;

        public RegExp(string pattern)
            : this(pattern, false)
        {
        }

        public RegExp(string pattern, bool ignoreCase)
        {
            this.pattern = pattern;
            this.ignoreCase = ignoreCase;
            this.element = this.ParseExpr();
            var flag = this.pos < pattern.Length;
            if (flag)
            {
                throw new RegExpException(RegExpException.ErrorType.UnexpectedCharacter, this.pos, pattern);
            }
        }

        public Matcher Matcher(string str)
        {
            return this.Matcher(new StringReader(str));
        }

        public Matcher Matcher(TextReader input)
        {
            var flag = input is LookAheadReader;
            var matcher = flag ? this.Matcher((LookAheadReader) input) : this.Matcher(new LookAheadReader(input));
            return matcher;
        }

        private Matcher Matcher(LookAheadReader input)
        {
            return new Matcher((Element) this.element.Clone(), input, this.ignoreCase);
        }

        public override string ToString()
        {
            var str = new StringWriter();
            str.WriteLine("Regular Expression");
            str.WriteLine(" Pattern: " + this.pattern);
            str.Write(" Flags:");
            var flag = this.ignoreCase;
            if (flag)
            {
                str.Write(" caseignore");
            }
            str.WriteLine();
            str.WriteLine(" Compiled:");
            this.element.PrintTo(str, " ");
            return str.ToString();
        }

        private Element ParseExpr()
        {
            var first = this.ParseTerm();
            var flag = this.PeekChar(0) != Convert.ToInt32('|');
            Element parseExpr;
            if (flag)
            {
                parseExpr = first;
            }
            else
            {
                this.ReadChar('|');
                var second = this.ParseExpr();
                parseExpr = new AlternativeElement(first, second);
            }
            return parseExpr;
        }

        private Element ParseTerm()
        {
            var list = new ArrayList {this.ParseFact()};
            while (true)
            {
                var i = this.PeekChar(0);
                var flag = i == -1;
                if (flag)
                {
                    break;
                }
                var c = Convert.ToChar(i);
                if (c <= '+')
                {
                    if (c == ')' || c == '+')
                    {
                        return this.CombineElements(list);
                    }
                }
                else
                {
                    if (c == '?' || c == ']')
                    {
                        return this.CombineElements(list);
                    }
                    switch (c)
                    {
                        case '{':
                        case '|':
                        case '}':
                            return this.CombineElements(list);
                    }
                }
                list.Add(this.ParseFact());
            }
            var parseTerm = this.CombineElements(list);
            return parseTerm;
        }

        private Element ParseFact()
        {
            var elem = this.ParseAtom();
            var i = this.PeekChar(0);
            var flag = i == -1;
            Element parseFact;
            if (flag)
            {
                parseFact = elem;
            }
            else
            {
                var c = Convert.ToChar(i);
                if (c <= '+')
                {
                    if (c != '*' && c != '+')
                    {
                        return elem;
                    }
                }
                else if (c != '?' && c != '{')
                {
                    return elem;
                }
                parseFact = this.ParseAtomModifier(elem);
                return parseFact;
            }
            return parseFact;
        }

        private Element ParseAtom()
        {
            var i = this.PeekChar(0);
            var flag = i == -1;
            if (flag)
            {
                throw new RegExpException(RegExpException.ErrorType.UnexpectedCharacter, this.pos, this.pattern);
            }
            var c = Convert.ToChar(i);
            Element parseAtom;
            if (c <= '?')
            {
                switch (c)
                {
                    case '(':
                    {
                        this.ReadChar('(');
                        var elem = this.ParseExpr();
                        this.ReadChar(')');
                        parseAtom = elem;
                        return parseAtom;
                    }
                    case ')':
                    case '*':
                    case '+':
                        break;
                    case ',':
                    case '-':
                        return this.ParseChar();
                    case '.':
                        this.ReadChar('.');
                        parseAtom = CharacterSetElement.DOT;
                        return parseAtom;
                    default:
                        if (c != '?')
                        {
                            return this.ParseChar();
                        }
                        break;
                }
            }
            else
            {
                if (c == '[')
                {
                    this.ReadChar('[');
                    var elem = this.ParseCharSet();
                    this.ReadChar(']');
                    parseAtom = elem;
                    return parseAtom;
                }
                if (c != ']')
                {
                    switch (c)
                    {
                        case '{':
                        case '|':
                        case '}':
                            break;
                        default:
                            return this.ParseChar();
                    }
                }
            }
            throw new RegExpException(RegExpException.ErrorType.UnexpectedCharacter, this.pos, this.pattern);
        }

        private Element ParseAtomModifier(Element elem)
        {
            var type = RepeatElement.RepeatType.Greedy;
            var c = this.ReadChar();
            int min;
            int max;
            if (c <= '+')
            {
                if (c == '*')
                {
                    min = 0;
                    max = -1;
                    type = this.RepeatType(type);
                    return new RepeatElement(elem, min, max, type);
                }
                if (c == '+')
                {
                    min = 1;
                    max = -1;
                    type = this.RepeatType(type);
                    return new RepeatElement(elem, min, max, type);
                }
            }
            else
            {
                if (c == '?')
                {
                    min = 0;
                    max = 1;
                    type = this.RepeatType(type);
                    return new RepeatElement(elem, min, max, type);
                }
                if (c == '{')
                {
                    var firstPos = this.pos - 1;
                    min = this.ReadNumber();
                    max = min;
                    var flag = this.PeekChar(0) == Convert.ToInt32(',');
                    if (flag)
                    {
                        this.ReadChar(',');
                        max = -1;
                        var flag2 = this.PeekChar(0) != Convert.ToInt32('}');
                        if (flag2)
                        {
                            max = this.ReadNumber();
                        }
                    }
                    this.ReadChar('}');
                    var flag3 = max == 0 || max > 0 && min > max;
                    if (flag3)
                    {
                        throw new RegExpException(RegExpException.ErrorType.InvalidRepeatCount, firstPos, this.pattern);
                    }
                    type = this.RepeatType(type);
                    return new RepeatElement(elem, min, max, type);
                }
            }
            throw new RegExpException(RegExpException.ErrorType.UnexpectedCharacter, this.pos - 1, this.pattern);
        }

        private RepeatElement.RepeatType RepeatType(RepeatElement.RepeatType type)
        {
            var flag4 = this.PeekChar(0) == Convert.ToInt32('?');
            if (flag4)
            {
                this.ReadChar('?');
                type = RepeatElement.RepeatType.Reluctant;
            }
            else
            {
                var flag5 = this.PeekChar(0) == Convert.ToInt32('+');
                if (flag5)
                {
                    this.ReadChar('+');
                    type = RepeatElement.RepeatType.Possessive;
                }
            }
            return type;
        }

        private Element ParseCharSet()
        {
            var repeat = true;
            var flag = this.PeekChar(0) == Convert.ToInt32('^');
            CharacterSetElement charset;
            if (flag)
            {
                this.ReadChar('^');
                charset = new CharacterSetElement(true);
            }
            else
            {
                charset = new CharacterSetElement(false);
            }
            while (this.PeekChar(0) > 0 && repeat)
            {
                var start = Convert.ToChar(this.PeekChar(0));
                if (start != '\\')
                {
                    if (start != ']')
                    {
                        this.ReadChar(start);
                        var flag2 = this.PeekChar(0) == Convert.ToInt32('-') && this.PeekChar(1) > 0 &&
                            this.PeekChar(1) != Convert.ToInt32(']');
                        if (flag2)
                        {
                            this.ReadChar('-');
                            var end = this.ReadChar();
                            charset.AddRange(this.FixChar(start), this.FixChar(end));
                        }
                        else
                        {
                            charset.AddCharacter(this.FixChar(start));
                        }
                    }
                    else
                    {
                        repeat = false;
                    }
                }
                else
                {
                    var elem = this.ParseEscapeChar();
                    var flag3 = elem is StringElement;
                    if (flag3)
                    {
                        charset.AddCharacters((StringElement) elem);
                    }
                    else
                    {
                        charset.AddCharacterSet((CharacterSetElement) elem);
                    }
                }
            }
            return charset;
        }

        private Element ParseChar()
        {
            var c = Convert.ToChar(this.PeekChar(0));
            if (c != '$')
            {
                Element parseChar;
                if (c != '\\')
                {
                    if (c == '^')
                    {
                        throw new RegExpException(RegExpException.ErrorType.UnsupportedSpecialCharacter, this.pos, this.pattern);
                    }
                    parseChar = new StringElement(this.FixChar(this.ReadChar()));
                }
                else
                {
                    parseChar = this.ParseEscapeChar();
                }
                return parseChar;
            }

            throw new RegExpException(RegExpException.ErrorType.UnsupportedSpecialCharacter, this.pos, this.pattern);
        }

        private Element ParseEscapeChar()
        {
            this.ReadChar('\\');
            var c = this.ReadChar();
            Element parseEscapeChar;
            if (c <= 'S')
            {
                if (c != '0')
                {
                    if (c == 'D')
                    {
                        parseEscapeChar = CharacterSetElement.NON_DIGIT;
                        return parseEscapeChar;
                    }
                    if (c == 'S')
                    {
                        parseEscapeChar = CharacterSetElement.NON_WHITESPACE;
                        return parseEscapeChar;
                    }
                }
                else
                {
                    c = this.ReadChar();
                    var flag = c < '0' || c > '3';
                    if (flag)
                    {
                        throw new RegExpException(RegExpException.ErrorType.UnsupportedEscapeCharacter, this.pos - 3, this.pattern);
                    }
                    var value = Convert.ToInt32(c) - Convert.ToInt32('0');
                    c = Convert.ToChar(this.PeekChar(0));
                    var flag2 = '0' <= c && c <= '7';
                    if (flag2)
                    {
                        value *= 8;
                        value += Convert.ToInt32(this.ReadChar()) - Convert.ToInt32('0');
                        c = Convert.ToChar(this.PeekChar(0));
                        var flag3 = '0' <= c && c <= '7';
                        if (flag3)
                        {
                            value *= 8;
                            value += Convert.ToInt32(this.ReadChar()) - Convert.ToInt32('0');
                        }
                    }
                    parseEscapeChar = new StringElement(this.FixChar(Convert.ToChar(value)));
                    return parseEscapeChar;
                }
            }
            else
            {
                if (c == 'W')
                {
                    parseEscapeChar = CharacterSetElement.NON_WORD;
                    return parseEscapeChar;
                }
                switch (c)
                {
                    case 'a':
                        parseEscapeChar = new StringElement('\a');
                        return parseEscapeChar;
                    case 'b':
                    case 'c':
                        break;
                    case 'd':
                        parseEscapeChar = CharacterSetElement.DIGIT;
                        return parseEscapeChar;
                    case 'e':
                        parseEscapeChar = new StringElement('\u001b');
                        return parseEscapeChar;
                    case 'f':
                        parseEscapeChar = new StringElement('\f');
                        return parseEscapeChar;
                    default:
                    {
                        string str;
                        switch (c)
                        {
                            case 'n':
                                parseEscapeChar = new StringElement('\n');
                                return parseEscapeChar;
                            case 'o':
                            case 'p':
                            case 'q':
                            case 'v':
                                return this.ParseEscapeChar(c);
                            case 'r':
                                parseEscapeChar = new StringElement('\r');
                                return parseEscapeChar;
                            case 's':
                                parseEscapeChar = CharacterSetElement.WHITESPACE;
                                return parseEscapeChar;
                            case 't':
                                parseEscapeChar = new StringElement('\t');
                                return parseEscapeChar;
                            case 'u':
                                break;
                            case 'w':
                                parseEscapeChar = CharacterSetElement.WORD;
                                return parseEscapeChar;
                            case 'x':
                                str = this.ReadChar() + this.ReadChar().ToString();
                                try
                                {
                                    var value = int.Parse(str, NumberStyles.AllowHexSpecifier);
                                    parseEscapeChar = new StringElement(this.FixChar(Convert.ToChar(value)));
                                    return parseEscapeChar;
                                }
                                catch (FormatException formatException)
                                {
                                    ProjectData.SetProjectError(formatException);
                                    throw new RegExpException(RegExpException.ErrorType.UnsupportedEscapeCharacter,
                                        this.pos - str.Length - 2, this.pattern);
                                }
                            default:
                                return this.ParseEscapeChar(c);
                        }
                        str = this.ReadChar() + this.ReadChar().ToString() + this.ReadChar() + this.ReadChar();
                        try
                        {
                            var value = int.Parse(str, NumberStyles.AllowHexSpecifier);
                            parseEscapeChar = new StringElement(this.FixChar(Convert.ToChar(value)));
                            return parseEscapeChar;
                        }
                        catch (FormatException formatException)
                        {
                            ProjectData.SetProjectError(formatException);
                            throw new RegExpException(RegExpException.ErrorType.UnsupportedEscapeCharacter, this.pos - str.Length - 2,
                                this.pattern);
                        }
                    }
                }
            }
            parseEscapeChar = this.ParseEscapeChar(c);
            return parseEscapeChar;
        }

        private Element ParseEscapeChar(char c)
        {
            var flag4 = 'A' <= c && c <= 'Z' || 'a' <= c && c <= 'z';
            if (flag4)
            {
                throw new RegExpException(RegExpException.ErrorType.UnsupportedEscapeCharacter, this.pos - 2, this.pattern);
            }
            Element parseEscapeChar = new StringElement(this.FixChar(c));
            return parseEscapeChar;
        }

        private char FixChar(char c)
        {
            return Conversions.ToChar(Interaction.IIf(this.ignoreCase, char.ToLower(c), c));
        }

        private int ReadNumber()
        {
            var buf = new StringBuilder();
            var c = this.PeekChar(0);
            while (Convert.ToInt32('0') <= c && c <= Convert.ToInt32('9'))
            {
                buf.Append(this.ReadChar());
                c = this.PeekChar(0);
            }
            var flag = buf.Length <= 0;
            if (flag)
            {
                throw new RegExpException(RegExpException.ErrorType.UnexpectedCharacter, this.pos, this.pattern);
            }
            return int.Parse(buf.ToString());
        }

        private char ReadChar()
        {
            var c = this.PeekChar(0);
            var flag = c < 0;
            if (flag)
            {
                throw new RegExpException(RegExpException.ErrorType.UnterminatedPattern, this.pos, this.pattern);
            }
            this.pos++;
            return Convert.ToChar(c);
        }

        private char ReadChar(char c)
        {
            var flag = c != this.ReadChar();
            if (flag)
            {
                throw new RegExpException(RegExpException.ErrorType.UnexpectedCharacter, this.pos - 1, this.pattern);
            }
            return c;
        }

        private int PeekChar(int count)
        {
            var flag = this.pos + count < this.pattern.Length;
            int peekChar;
            if (flag)
            {
                peekChar = Convert.ToInt32(this.pattern[this.pos + count]);
            }
            else
            {
                peekChar = -1;
            }
            return peekChar;
        }

        private Element CombineElements(ArrayList list)
        {
            var prev = (Element) list[0];
            var num = list.Count - 2;
            Element elem;
            for (var i = 1; i <= num; i++)
            {
                elem = (Element) list[i];
                var flag = prev is StringElement && elem is StringElement;
                if (flag)
                {
                    var str = ((StringElement) prev).GetString() + ((StringElement) elem).GetString();
                    elem = new StringElement(str);
                    list.RemoveAt(i);
                    list[i - 1] = elem;
                    i--;
                }
                prev = elem;
            }
            elem = (Element) list[list.Count - 1];
            var num2 = list.Count - 2;
            for (var i = num2; i >= 0; i += -1)
            {
                prev = (Element) list[i];
                elem = new CombineElement(prev, elem);
            }
            return elem;
        }
    }
}