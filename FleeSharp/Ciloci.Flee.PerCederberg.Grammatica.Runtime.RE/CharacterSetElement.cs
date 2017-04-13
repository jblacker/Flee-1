using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class CharacterSetElement : Element
	{
		private class Range
		{
			private char min;

			private char max;

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

		public static CharacterSetElement DOT = new CharacterSetElement(false);

		public static CharacterSetElement DIGIT = new CharacterSetElement(false);

		public static CharacterSetElement NON_DIGIT = new CharacterSetElement(true);

		public static CharacterSetElement WHITESPACE = new CharacterSetElement(false);

		public static CharacterSetElement NON_WHITESPACE = new CharacterSetElement(true);

		public static CharacterSetElement WORD = new CharacterSetElement(false);

		public static CharacterSetElement NON_WORD = new CharacterSetElement(true);

		private bool inverted;

		private ArrayList contents;

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
			int num = str.Length - 1;
			for (int i = 0; i <= num; i++)
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
			this.contents.Add(new CharacterSetElement.Range(min, max));
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
			bool flag = skip != 0;
			int Match;
			if (flag)
			{
				Match = -1;
			}
			else
			{
				int c = input.Peek(start);
				bool flag2 = c < 0;
				if (flag2)
				{
					m.SetReadEndOfString();
					Match = -1;
				}
				else
				{
					bool flag3 = m.IsCaseInsensitive();
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
			bool flag = this == CharacterSetElement.DOT;
			bool InSet;
			if (flag)
			{
				InSet = this.InDotSet(c);
			}
			else
			{
				bool flag2 = this == CharacterSetElement.DIGIT || this == CharacterSetElement.NON_DIGIT;
				if (flag2)
				{
					InSet = (this.InDigitSet(c) != this.inverted);
				}
				else
				{
					bool flag3 = this == CharacterSetElement.WHITESPACE || this == CharacterSetElement.NON_WHITESPACE;
					if (flag3)
					{
						InSet = (this.InWhitespaceSet(c) != this.inverted);
					}
					else
					{
						bool flag4 = this == CharacterSetElement.WORD || this == CharacterSetElement.NON_WORD;
						if (flag4)
						{
							InSet = (this.InWordSet(c) != this.inverted);
						}
						else
						{
							InSet = (this.InUserSet(c) != this.inverted);
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
					goto IL_3A;
				}
			}
			else if (c != '\u0085' && c != '\u2028' && c != '\u2029')
			{
				goto IL_3A;
			}
			bool InDotSet = false;
			return InDotSet;
			IL_3A:
			InDotSet = true;
			return InDotSet;
		}

		private bool InDigitSet(char c)
		{
			return '0' <= c && c <= '9';
		}

		private bool InWhitespaceSet(char c)
		{
			bool InWhitespaceSet;
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
					InWhitespaceSet = false;
					return InWhitespaceSet;
				}
				break;
			}
			InWhitespaceSet = true;
			return InWhitespaceSet;
		}

		private bool InWordSet(char c)
		{
			return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || ('0' <= c && c <= '9') || c == '_';
		}

		private bool InUserSet(char value)
		{
			int num = this.contents.Count - 1;
			bool InUserSet;
			for (int i = 0; i <= num; i++)
			{
				object obj = RuntimeHelpers.GetObjectValue(this.contents[i]);
				bool flag = obj is char;
				if (flag)
				{
					char c = Conversions.ToChar(obj);
					bool flag2 = c == value;
					if (flag2)
					{
						InUserSet = true;
						return InUserSet;
					}
				}
				else
				{
					bool flag3 = obj is CharacterSetElement.Range;
					if (flag3)
					{
						CharacterSetElement.Range r = (CharacterSetElement.Range)obj;
						bool flag4 = r.Inside(value);
						if (flag4)
						{
							InUserSet = true;
							return InUserSet;
						}
					}
					else
					{
						bool flag5 = obj is CharacterSetElement;
						if (flag5)
						{
							CharacterSetElement e = (CharacterSetElement)obj;
							bool flag6 = e.InSet(value);
							if (flag6)
							{
								InUserSet = true;
								return InUserSet;
							}
						}
					}
				}
			}
			InUserSet = false;
			return InUserSet;
		}

		public override void PrintTo(TextWriter output, string indent)
		{
			output.WriteLine(indent + this.ToString());
		}

		public override string ToString()
		{
			bool flag = this == CharacterSetElement.DOT;
			string ToString;
			if (flag)
			{
				ToString = ".";
			}
			else
			{
				bool flag2 = this == CharacterSetElement.DIGIT;
				if (flag2)
				{
					ToString = "\\d";
				}
				else
				{
					bool flag3 = this == CharacterSetElement.NON_DIGIT;
					if (flag3)
					{
						ToString = "\\D";
					}
					else
					{
						bool flag4 = this == CharacterSetElement.WHITESPACE;
						if (flag4)
						{
							ToString = "\\s";
						}
						else
						{
							bool flag5 = this == CharacterSetElement.NON_WHITESPACE;
							if (flag5)
							{
								ToString = "\\S";
							}
							else
							{
								bool flag6 = this == CharacterSetElement.WORD;
								if (flag6)
								{
									ToString = "\\w";
								}
								else
								{
									bool flag7 = this == CharacterSetElement.NON_WORD;
									if (flag7)
									{
										ToString = "\\W";
									}
									else
									{
										StringBuilder buffer = new StringBuilder();
										bool flag8 = this.inverted;
										if (flag8)
										{
											buffer.Append("^[");
										}
										else
										{
											buffer.Append("[");
										}
										int num = this.contents.Count - 1;
										for (int i = 0; i <= num; i++)
										{
											buffer.Append(RuntimeHelpers.GetObjectValue(this.contents[i]));
										}
										buffer.Append("]");
										ToString = buffer.ToString();
									}
								}
							}
						}
					}
				}
			}
			return ToString;
		}
	}
}
