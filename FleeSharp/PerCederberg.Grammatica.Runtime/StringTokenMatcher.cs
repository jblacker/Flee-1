namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class StringTokenMatcher : TokenMatcher
	{
		private readonly ArrayList patterns;

		private readonly Automaton start;

		private TokenPattern tokenPattern;

		private readonly bool ignoreCase;

		public StringTokenMatcher(bool ignoreCase)
		{
			this.patterns = new ArrayList();
			this.start = new Automaton();
			this.ignoreCase = false;
			this.ignoreCase = ignoreCase;
		}

		public void Reset()
		{
			this.tokenPattern = null;
		}

		public override TokenPattern GetMatchedPattern()
		{
			return this.tokenPattern;
		}

		public override int GetMatchedLength()
		{
			bool flag = this.tokenPattern == null;
		    var getMatchedLength = flag ? 0 : this.tokenPattern.Pattern.Length;
			return getMatchedLength;
		}

		public TokenPattern GetPattern(int id)
		{
			int num = this.patterns.Count - 1;
		    for (int i = 0; i <= num; i++)
			{
				var pattern = (TokenPattern)this.patterns[i];
				bool flag = pattern.Id == id;
				if (flag)
				{
				    var getPattern = pattern;
				    return getPattern;
				}
			}
		    return null;
		}

		public void AddPattern(TokenPattern pattern)
		{
			this.patterns.Add(pattern);
			this.start.AddMatch(pattern.Pattern, this.ignoreCase, pattern);
		}

		public bool Match(LookAheadReader input)
		{
			this.Reset();
			this.tokenPattern = (TokenPattern)this.start.MatchFrom(input, 0, this.ignoreCase);
			return this.tokenPattern != null;
		}

		public override string ToString()
		{
			var buffer = new StringBuilder();
			int num = this.patterns.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				buffer.Append(RuntimeHelpers.GetObjectValue(this.patterns[i]));
				buffer.Append("\n\n");
			}
			return buffer.ToString();
		}
	}
}
