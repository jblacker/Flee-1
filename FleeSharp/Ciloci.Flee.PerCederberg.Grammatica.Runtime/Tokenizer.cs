using Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class Tokenizer
	{
		private bool ignoreCase;

		private bool m_useTokenList;

		private StringTokenMatcher stringMatcher;

		private ArrayList regexpMatchers;

		private LookAheadReader input;

		private Token previousToken;

		public bool UseTokenList
		{
			get
			{
				return this.m_useTokenList;
			}
			set
			{
				this.m_useTokenList = value;
			}
		}

		public Tokenizer(TextReader input) : this(input, false)
		{
		}

		public Tokenizer(TextReader input, bool ignoreCase)
		{
			this.ignoreCase = false;
			this.regexpMatchers = new ArrayList();
			this.stringMatcher = new StringTokenMatcher(ignoreCase);
			this.input = new LookAheadReader(input);
			this.ignoreCase = ignoreCase;
		}

		public bool GetUseTokenList()
		{
			return this.m_useTokenList;
		}

		public void SetUseTokenList(bool useTokenList)
		{
			this.m_useTokenList = useTokenList;
		}

		public string GetPatternDescription(int id)
		{
			TokenPattern pattern = this.stringMatcher.GetPattern(id);
			bool flag = pattern != null;
			string GetPatternDescription;
			if (flag)
			{
				GetPatternDescription = pattern.ToShortString();
			}
			else
			{
				int num = this.regexpMatchers.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					RegExpTokenMatcher re = (RegExpTokenMatcher)this.regexpMatchers[i];
					bool flag2 = re.GetPattern().Id == id;
					if (flag2)
					{
						GetPatternDescription = re.GetPattern().ToShortString();
						return GetPatternDescription;
					}
				}
				GetPatternDescription = null;
			}
			return GetPatternDescription;
		}

		public int GetCurrentLine()
		{
			return this.input.LineNumber;
		}

		public int GetCurrentColumn()
		{
			return this.input.ColumnNumber;
		}

		public void AddPattern(TokenPattern pattern)
		{
			TokenPattern.PatternType type = pattern.Type;
			if (type != TokenPattern.PatternType.STRING)
			{
				if (type != TokenPattern.PatternType.REGEXP)
				{
					throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_TOKEN, pattern.Name, Conversions.ToString(Conversions.ToDouble("pattern type ") + (double)pattern.Type + Conversions.ToDouble(" is undefined")));
				}
				try
				{
					this.regexpMatchers.Add(new RegExpTokenMatcher(pattern, this.ignoreCase, this.input));
				}
				catch (RegExpException expr_46)
				{
					ProjectData.SetProjectError(expr_46);
					RegExpException e = expr_46;
					throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_TOKEN, pattern.Name, "regular expression contains error(s): " + e.Message);
				}
			}
			else
			{
				this.stringMatcher.AddPattern(pattern);
			}
		}

		public void Reset(TextReader input)
		{
			this.input.Close();
			this.input = new LookAheadReader(input);
			this.previousToken = null;
			this.stringMatcher.Reset();
			int num = this.regexpMatchers.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				((RegExpTokenMatcher)this.regexpMatchers[i]).Reset(this.input);
			}
		}

		public Token Next()
		{
			Token token;
			while (true)
			{
				token = this.NextToken();
				bool flag = this.m_useTokenList && token != null;
				if (flag)
				{
					token.Previous = this.previousToken;
					this.previousToken = token;
				}
				bool flag2 = token == null;
				if (flag2)
				{
					break;
				}
				bool error = token.Pattern.Error;
				if (error)
				{
					goto Block_3;
				}
				bool ignore = token.Pattern.Ignore;
				if (ignore)
				{
					token = null;
				}
				if (token != null)
				{
					goto Block_5;
				}
			}
			Token Next = null;
			return Next;
			Block_3:
			throw new ParseException(ParseException.ErrorType.INVALID_TOKEN, token.Pattern.ErrorMessage, token.StartLine, token.StartColumn);
			Block_5:
			Next = token;
			return Next;
		}

		private Token NextToken()
		{
			Token NextToken;
			try
			{
				TokenMatcher i = this.FindMatch();
				bool flag = i != null;
				if (flag)
				{
					int line = this.input.LineNumber;
					int column = this.input.ColumnNumber;
					string str = this.input.ReadString(i.GetMatchedLength());
					NextToken = new Token(i.GetMatchedPattern(), str, line, column);
				}
				else
				{
					bool flag2 = this.input.Peek() < 0;
					if (!flag2)
					{
						int line = this.input.LineNumber;
						int column = this.input.ColumnNumber;
						throw new ParseException(ParseException.ErrorType.UNEXPECTED_CHAR, this.input.ReadString(1), line, column);
					}
					NextToken = null;
				}
			}
			catch (IOException expr_98)
			{
				ProjectData.SetProjectError(expr_98);
				IOException e = expr_98;
				throw new ParseException(ParseException.ErrorType.IO, e.Message, -1, -1);
			}
			return NextToken;
		}

		private TokenMatcher FindMatch()
		{
			TokenMatcher bestMatch = null;
			int bestLength = 0;
			bool flag = this.stringMatcher.Match(this.input);
			if (flag)
			{
				bestMatch = this.stringMatcher;
				bestLength = bestMatch.GetMatchedLength();
			}
			int num = this.regexpMatchers.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				RegExpTokenMatcher re = (RegExpTokenMatcher)this.regexpMatchers[i];
				bool flag2 = re.Match() && re.GetMatchedLength() > bestLength;
				if (flag2)
				{
					bestMatch = re;
					bestLength = re.GetMatchedLength();
				}
			}
			return bestMatch;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(this.stringMatcher);
			int num = this.regexpMatchers.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				buffer.Append(RuntimeHelpers.GetObjectValue(this.regexpMatchers[i]));
			}
			return buffer.ToString();
		}
	}
}
