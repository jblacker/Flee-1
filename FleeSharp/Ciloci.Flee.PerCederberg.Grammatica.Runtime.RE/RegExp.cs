using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class RegExp
	{
		private Element element;

		private string pattern;

		private bool ignoreCase;

		private int pos;

		public RegExp(string pattern) : this(pattern, false)
		{
		}

		public RegExp(string pattern, bool ignoreCase)
		{
			this.pattern = pattern;
			this.ignoreCase = ignoreCase;
			this.element = this.ParseExpr();
			bool flag = this.pos < pattern.Length;
			if (flag)
			{
				throw new RegExpException(RegExpException.ErrorType.UNEXPECTED_CHARACTER, this.pos, pattern);
			}
		}

		public Matcher Matcher(string str)
		{
			return this.Matcher(new StringReader(str));
		}

		public Matcher Matcher(TextReader input)
		{
			bool flag = input is LookAheadReader;
			Matcher Matcher;
			if (flag)
			{
				Matcher = this.Matcher((LookAheadReader)input);
			}
			else
			{
				Matcher = this.Matcher(new LookAheadReader(input));
			}
			return Matcher;
		}

		private Matcher Matcher(LookAheadReader input)
		{
			return new Matcher((Element)this.element.Clone(), input, this.ignoreCase);
		}

		public override string ToString()
		{
			StringWriter str = new StringWriter();
			str.WriteLine("Regular Expression");
			str.WriteLine(" Pattern: " + this.pattern);
			str.Write(" Flags:");
			bool flag = this.ignoreCase;
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
			Element first = this.ParseTerm();
			bool flag = this.PeekChar(0) != Convert.ToInt32('|');
			Element ParseExpr;
			if (flag)
			{
				ParseExpr = first;
			}
			else
			{
				this.ReadChar('|');
				Element second = this.ParseExpr();
				ParseExpr = new AlternativeElement(first, second);
			}
			return ParseExpr;
		}

		private Element ParseTerm()
		{
			ArrayList list = new ArrayList();
			list.Add(this.ParseFact());
			while (true)
			{
				int i = this.PeekChar(0);
				bool flag = i == -1;
				if (flag)
				{
					break;
				}
				char c = Convert.ToChar(i);
				if (c <= '+')
				{
					if (c == ')' || c == '+')
					{
						goto IL_73;
					}
				}
				else
				{
					if (c == '?' || c == ']')
					{
						goto IL_73;
					}
					switch (c)
					{
					case '{':
					case '|':
					case '}':
						goto IL_73;
					}
				}
				list.Add(this.ParseFact());
			}
			Element ParseTerm = this.CombineElements(list);
			return ParseTerm;
			IL_73:
			ParseTerm = this.CombineElements(list);
			return ParseTerm;
		}

		private Element ParseFact()
		{
			Element elem = this.ParseAtom();
			int i = this.PeekChar(0);
			bool flag = i == -1;
			Element ParseFact;
			if (flag)
			{
				ParseFact = elem;
			}
			else
			{
				char c = Convert.ToChar(i);
				if (c <= '+')
				{
					if (c != '*' && c != '+')
					{
						goto IL_53;
					}
				}
				else if (c != '?' && c != '{')
				{
					goto IL_53;
				}
				ParseFact = this.ParseAtomModifier(elem);
				return ParseFact;
				IL_53:
				ParseFact = elem;
			}
			return ParseFact;
		}

		private Element ParseAtom()
		{
			int i = this.PeekChar(0);
			bool flag = i == -1;
			if (flag)
			{
				throw new RegExpException(RegExpException.ErrorType.UNEXPECTED_CHARACTER, this.pos, this.pattern);
			}
			char c = Convert.ToChar(i);
			Element ParseAtom;
			if (c <= '?')
			{
				switch (c)
				{
				case '(':
				{
					this.ReadChar('(');
					Element elem = this.ParseExpr();
					this.ReadChar(')');
					ParseAtom = elem;
					return ParseAtom;
				}
				case ')':
				case '*':
				case '+':
					break;
				case ',':
				case '-':
					goto IL_EC;
				case '.':
					this.ReadChar('.');
					ParseAtom = CharacterSetElement.DOT;
					return ParseAtom;
				default:
					if (c != '?')
					{
						goto IL_EC;
					}
					break;
				}
			}
			else
			{
				if (c == '[')
				{
					this.ReadChar('[');
					Element elem = this.ParseCharSet();
					this.ReadChar(']');
					ParseAtom = elem;
					return ParseAtom;
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
						goto IL_EC;
					}
				}
			}
			throw new RegExpException(RegExpException.ErrorType.UNEXPECTED_CHARACTER, this.pos, this.pattern);
			IL_EC:
			ParseAtom = this.ParseChar();
			return ParseAtom;
		}

		private Element ParseAtomModifier(Element elem)
		{
			RepeatElement.RepeatType type = RepeatElement.RepeatType.GREEDY;
			char c = this.ReadChar();
			int min;
			int max;
			if (c <= '+')
			{
				if (c == '*')
				{
					min = 0;
					max = -1;
					goto IL_FB;
				}
				if (c == '+')
				{
					min = 1;
					max = -1;
					goto IL_FB;
				}
			}
			else
			{
				if (c == '?')
				{
					min = 0;
					max = 1;
					goto IL_FB;
				}
				if (c == '{')
				{
					int firstPos = this.pos - 1;
					min = this.ReadNumber();
					max = min;
					bool flag = this.PeekChar(0) == Convert.ToInt32(',');
					if (flag)
					{
						this.ReadChar(',');
						max = -1;
						bool flag2 = this.PeekChar(0) != Convert.ToInt32('}');
						if (flag2)
						{
							max = this.ReadNumber();
						}
					}
					this.ReadChar('}');
					bool flag3 = max == 0 || (max > 0 && min > max);
					if (flag3)
					{
						throw new RegExpException(RegExpException.ErrorType.INVALID_REPEAT_COUNT, firstPos, this.pattern);
					}
					goto IL_FB;
				}
			}
			throw new RegExpException(RegExpException.ErrorType.UNEXPECTED_CHARACTER, this.pos - 1, this.pattern);
			IL_FB:
			bool flag4 = this.PeekChar(0) == Convert.ToInt32('?');
			if (flag4)
			{
				this.ReadChar('?');
				type = RepeatElement.RepeatType.RELUCTANT;
			}
			else
			{
				bool flag5 = this.PeekChar(0) == Convert.ToInt32('+');
				if (flag5)
				{
					this.ReadChar('+');
					type = RepeatElement.RepeatType.POSSESSIVE;
				}
			}
			return new RepeatElement(elem, min, max, type);
		}

		private Element ParseCharSet()
		{
			bool repeat = true;
			bool flag = this.PeekChar(0) == Convert.ToInt32('^');
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
				char start = Convert.ToChar(this.PeekChar(0));
				if (start != '\\')
				{
					if (start != ']')
					{
						this.ReadChar(start);
						bool flag2 = this.PeekChar(0) == Convert.ToInt32('-') && this.PeekChar(1) > 0 && this.PeekChar(1) != Convert.ToInt32(']');
						if (flag2)
						{
							this.ReadChar('-');
							char end = this.ReadChar();
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
					Element elem = this.ParseEscapeChar();
					bool flag3 = elem is StringElement;
					if (flag3)
					{
						charset.AddCharacters((StringElement)elem);
					}
					else
					{
						charset.AddCharacterSet((CharacterSetElement)elem);
					}
				}
			}
			return charset;
		}

		private Element ParseChar()
		{
			char c = Convert.ToChar(this.PeekChar(0));
			if (c != '$')
			{
				Element ParseChar;
				if (c != '\\')
				{
					if (c == '^')
					{
						goto IL_2E;
					}
					ParseChar = new StringElement(this.FixChar(this.ReadChar()));
				}
				else
				{
					ParseChar = this.ParseEscapeChar();
				}
				return ParseChar;
			}
			IL_2E:
			throw new RegExpException(RegExpException.ErrorType.UNSUPPORTED_SPECIAL_CHARACTER, this.pos, this.pattern);
		}

		private Element ParseEscapeChar()
		{
			this.ReadChar('\\');
			char c = this.ReadChar();
			Element ParseEscapeChar;
			if (c <= 'S')
			{
				if (c != '0')
				{
					if (c == 'D')
					{
						ParseEscapeChar = CharacterSetElement.NON_DIGIT;
						return ParseEscapeChar;
					}
					if (c == 'S')
					{
						ParseEscapeChar = CharacterSetElement.NON_WHITESPACE;
						return ParseEscapeChar;
					}
				}
				else
				{
					c = this.ReadChar();
					bool flag = c < '0' || c > '3';
					if (flag)
					{
						throw new RegExpException(RegExpException.ErrorType.UNSUPPORTED_ESCAPE_CHARACTER, this.pos - 3, this.pattern);
					}
					int value = Convert.ToInt32(c) - Convert.ToInt32('0');
					c = Convert.ToChar(this.PeekChar(0));
					bool flag2 = '0' <= c && c <= '7';
					if (flag2)
					{
						value *= 8;
						value += Convert.ToInt32(this.ReadChar()) - Convert.ToInt32('0');
						c = Convert.ToChar(this.PeekChar(0));
						bool flag3 = '0' <= c && c <= '7';
						if (flag3)
						{
							value *= 8;
							value += Convert.ToInt32(this.ReadChar()) - Convert.ToInt32('0');
						}
					}
					ParseEscapeChar = new StringElement(this.FixChar(Convert.ToChar(value)));
					return ParseEscapeChar;
				}
			}
			else
			{
				if (c == 'W')
				{
					ParseEscapeChar = CharacterSetElement.NON_WORD;
					return ParseEscapeChar;
				}
				switch (c)
				{
				case 'a':
					ParseEscapeChar = new StringElement('\a');
					return ParseEscapeChar;
				case 'b':
				case 'c':
					break;
				case 'd':
					ParseEscapeChar = CharacterSetElement.DIGIT;
					return ParseEscapeChar;
				case 'e':
					ParseEscapeChar = new StringElement('\u001b');
					return ParseEscapeChar;
				case 'f':
					ParseEscapeChar = new StringElement('\f');
					return ParseEscapeChar;
				default:
				{
					string str;
					switch (c)
					{
					case 'n':
						ParseEscapeChar = new StringElement('\n');
						return ParseEscapeChar;
					case 'o':
					case 'p':
					case 'q':
					case 'v':
						goto IL_2F4;
					case 'r':
						ParseEscapeChar = new StringElement('\r');
						return ParseEscapeChar;
					case 's':
						ParseEscapeChar = CharacterSetElement.WHITESPACE;
						return ParseEscapeChar;
					case 't':
						goto IL_26E;
					case 'u':
						break;
					case 'w':
						ParseEscapeChar = CharacterSetElement.WORD;
						return ParseEscapeChar;
					case 'x':
						str = this.ReadChar().ToString() + this.ReadChar().ToString();
						try
						{
							int value = int.Parse(str, NumberStyles.AllowHexSpecifier);
							ParseEscapeChar = new StringElement(this.FixChar(Convert.ToChar(value)));
							return ParseEscapeChar;
						}
						catch (FormatException expr_1BD)
						{
							ProjectData.SetProjectError(expr_1BD);
							throw new RegExpException(RegExpException.ErrorType.UNSUPPORTED_ESCAPE_CHARACTER, this.pos - str.Length - 2, this.pattern);
						}
						break;
					default:
						goto IL_2F4;
					}
					str = this.ReadChar().ToString() + this.ReadChar().ToString() + this.ReadChar().ToString() + this.ReadChar().ToString();
					try
					{
						int value = int.Parse(str, NumberStyles.AllowHexSpecifier);
						ParseEscapeChar = new StringElement(this.FixChar(Convert.ToChar(value)));
						return ParseEscapeChar;
					}
					catch (FormatException expr_249)
					{
						ProjectData.SetProjectError(expr_249);
						throw new RegExpException(RegExpException.ErrorType.UNSUPPORTED_ESCAPE_CHARACTER, this.pos - str.Length - 2, this.pattern);
					}
					IL_26E:
					ParseEscapeChar = new StringElement('\t');
					return ParseEscapeChar;
				}
				}
			}
			IL_2F4:
			bool flag4 = ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z');
			if (flag4)
			{
				throw new RegExpException(RegExpException.ErrorType.UNSUPPORTED_ESCAPE_CHARACTER, this.pos - 2, this.pattern);
			}
			ParseEscapeChar = new StringElement(this.FixChar(c));
			return ParseEscapeChar;
		}

		private char FixChar(char c)
		{
			return Conversions.ToChar(Interaction.IIf(this.ignoreCase, char.ToLower(c), c));
		}

		private int ReadNumber()
		{
			StringBuilder buf = new StringBuilder();
			int c = this.PeekChar(0);
			while (Convert.ToInt32('0') <= c && c <= Convert.ToInt32('9'))
			{
				buf.Append(this.ReadChar());
				c = this.PeekChar(0);
			}
			bool flag = buf.Length <= 0;
			if (flag)
			{
				throw new RegExpException(RegExpException.ErrorType.UNEXPECTED_CHARACTER, this.pos, this.pattern);
			}
			return int.Parse(buf.ToString());
		}

		private char ReadChar()
		{
			int c = this.PeekChar(0);
			bool flag = c < 0;
			if (flag)
			{
				throw new RegExpException(RegExpException.ErrorType.UNTERMINATED_PATTERN, this.pos, this.pattern);
			}
			this.pos++;
			return Convert.ToChar(c);
		}

		private char ReadChar(char c)
		{
			bool flag = c != this.ReadChar();
			if (flag)
			{
				throw new RegExpException(RegExpException.ErrorType.UNEXPECTED_CHARACTER, this.pos - 1, this.pattern);
			}
			return c;
		}

		private int PeekChar(int count)
		{
			bool flag = this.pos + count < this.pattern.Length;
			int PeekChar;
			if (flag)
			{
				PeekChar = Convert.ToInt32(this.pattern[this.pos + count]);
			}
			else
			{
				PeekChar = -1;
			}
			return PeekChar;
		}

		private Element CombineElements(ArrayList list)
		{
			Element prev = (Element)list[0];
			int num = list.Count - 2;
			Element elem;
			for (int i = 1; i <= num; i++)
			{
				elem = (Element)list[i];
				bool flag = prev is StringElement && elem is StringElement;
				if (flag)
				{
					string str = ((StringElement)prev).GetString() + ((StringElement)elem).GetString();
					elem = new StringElement(str);
					list.RemoveAt(i);
					list[i - 1] = elem;
					i--;
				}
				prev = elem;
			}
			elem = (Element)list[list.Count - 1];
			int num2 = list.Count - 2;
			for (int i = num2; i >= 0; i += -1)
			{
				prev = (Element)list[i];
				elem = new CombineElement(prev, elem);
			}
			return elem;
		}
	}
}
