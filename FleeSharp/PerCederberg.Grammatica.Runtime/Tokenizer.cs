namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Exceptions;
    using Microsoft.VisualBasic.CompilerServices;
    using RE;

    internal class Tokenizer
	{
		private readonly bool ignoreCase;

	    private readonly StringTokenMatcher stringMatcher;

		private readonly ArrayList regexpMatchers;

		private LookAheadReader input;

		private Token previousToken;

		public bool UseTokenList { get; set; }

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
			return this.UseTokenList;
		}

		public void SetUseTokenList(bool useTokenList)
		{
			this.UseTokenList = useTokenList;
		}

		public string GetPatternDescription(int id)
		{
			var pattern = this.stringMatcher.GetPattern(id);
			bool flag = pattern != null;
			string getPatternDescription;
			if (flag)
			{
				getPatternDescription = pattern.ToShortString();
			}
			else
			{
				int num = this.regexpMatchers.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					var re = (RegExpTokenMatcher)this.regexpMatchers[i];
					bool flag2 = re.GetPattern().Id == id;
					if (flag2)
					{
						getPatternDescription = re.GetPattern().ToShortString();
						return getPatternDescription;
					}
				}
				getPatternDescription = null;
			}
			return getPatternDescription;
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
			var type = pattern.Type;
			if (type != TokenPattern.PatternType.String)
			{
				if (type != TokenPattern.PatternType.Regexp)
				{
					throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_TOKEN, pattern.Name, 
                        Conversions.ToString(Conversions.ToDouble("pattern type ") + (double)pattern.Type + 
                        Conversions.ToDouble(" is undefined")));
				}
				try
				{
					this.regexpMatchers.Add(new RegExpTokenMatcher(pattern, this.ignoreCase, this.input));
				}
				catch (RegExpException exception)
				{
					ProjectData.SetProjectError(exception);
					var e = exception;
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
            Token token = null;

            do
            {
                token = NextToken();
                if (token == null)
                {
                    previousToken = null;
                    return null;
                }
                if (UseTokenList)
                {
                    token.Previous = previousToken;
                    previousToken = token;
                }
                if (token.Pattern.Ignore)
                {
                    token = null;
                }
                else if (token.Pattern.Error)
                {
                    throw new ParseException(
                        ParseException.ErrorType.InvalidToken,
                        token.Pattern.ErrorMessage,
                        token.StartLine,
                        token.StartColumn);
                }
            } while (token == null);
            return token;
		}

		private Token NextToken()
		{
			Token nextToken;
			try
			{
				var i = this.FindMatch();
				bool flag = i != null;
				if (flag)
				{
					int line = this.input.LineNumber;
					int column = this.input.ColumnNumber;
					string str = this.input.ReadString(i.GetMatchedLength());
					nextToken = new Token(i.GetMatchedPattern(), str, line, column);
				}
				else
				{
					bool flag2 = this.input.Peek() < 0;
					if (!flag2)
					{
						int line = this.input.LineNumber;
						int column = this.input.ColumnNumber;
						throw new ParseException(ParseException.ErrorType.UnexpectedChar, this.input.ReadString(1), line, column);
					}
					nextToken = null;
				}
			}
			catch (IOException ioException)
			{
				ProjectData.SetProjectError(ioException);
				var e = ioException;
				throw new ParseException(ParseException.ErrorType.Io, e.Message, -1, -1);
			}
			return nextToken;
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
				var re = (RegExpTokenMatcher)this.regexpMatchers[i];
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
			var buffer = new StringBuilder();
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
