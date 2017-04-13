using System;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal abstract class TokenMatcher
	{
		public abstract TokenPattern GetMatchedPattern();

		public abstract int GetMatchedLength();
	}
}
