namespace Flee.PerCederberg.Grammatica.Runtime
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using RE;

    internal class RegExpTokenMatcher : TokenMatcher
	{
		private readonly TokenPattern pattern;

		private readonly RegExp regExp;

		private readonly Matcher matcher;

		public RegExpTokenMatcher(TokenPattern pattern, bool ignoreCase, LookAheadReader input)
		{
			this.pattern = pattern;
			this.regExp = new RegExp(pattern.Pattern, ignoreCase);
			this.matcher = this.regExp.Matcher(input);
		}

		public void Reset(LookAheadReader input)
		{
			this.matcher.Reset(input);
		}

		public TokenPattern GetPattern()
		{
			return this.pattern;
		}

		public override TokenPattern GetMatchedPattern()
		{
			bool flag = this.matcher == null || this.matcher.Length() <= 0;
		    var getMatchedPattern = flag ? null : this.pattern;
			return getMatchedPattern;
		}

		public override int GetMatchedLength()
		{
			return Conversions.ToInteger(Interaction.IIf(this.matcher == null, 0, this.matcher?.Length()));
		}

		public bool Match()
		{
			return this.matcher.MatchFromBeginning();
		}

		public override string ToString()
		{
			return (this.pattern + "\n" + (this.regExp) + "\n");
		}
	}
}
