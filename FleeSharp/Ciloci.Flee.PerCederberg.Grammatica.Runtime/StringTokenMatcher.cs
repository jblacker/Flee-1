using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class StringTokenMatcher : TokenMatcher
	{
		private ArrayList patterns;

		private Automaton start;

		private TokenPattern m_match;

		private bool ignoreCase;

		public StringTokenMatcher(bool ignoreCase)
		{
			this.patterns = new ArrayList();
			this.start = new Automaton();
			this.ignoreCase = false;
			this.ignoreCase = ignoreCase;
		}

		public void Reset()
		{
			this.m_match = null;
		}

		public override TokenPattern GetMatchedPattern()
		{
			return this.m_match;
		}

		public override int GetMatchedLength()
		{
			bool flag = this.m_match == null;
			int GetMatchedLength;
			if (flag)
			{
				GetMatchedLength = 0;
			}
			else
			{
				GetMatchedLength = this.m_match.Pattern.Length;
			}
			return GetMatchedLength;
		}

		public TokenPattern GetPattern(int id)
		{
			int num = this.patterns.Count - 1;
			TokenPattern GetPattern;
			for (int i = 0; i <= num; i++)
			{
				TokenPattern pattern = (TokenPattern)this.patterns[i];
				bool flag = pattern.Id == id;
				if (flag)
				{
					GetPattern = pattern;
					return GetPattern;
				}
			}
			GetPattern = null;
			return GetPattern;
		}

		public void AddPattern(TokenPattern pattern)
		{
			this.patterns.Add(pattern);
			this.start.AddMatch(pattern.Pattern, this.ignoreCase, pattern);
		}

		public bool Match(LookAheadReader input)
		{
			this.Reset();
			this.m_match = (TokenPattern)this.start.MatchFrom(input, 0, this.ignoreCase);
			return this.m_match != null;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
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
