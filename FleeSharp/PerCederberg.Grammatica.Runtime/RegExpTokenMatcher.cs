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

namespace FleeSharp.PerCederberg.Grammatica.Runtime
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using RE;

    internal class RegExpTokenMatcher : TokenMatcher
    {
        private readonly Matcher matcher;
        private readonly TokenPattern pattern;

        private readonly RegExp regExp;

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
            var flag = this.matcher == null || this.matcher.Length() <= 0;
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
            return this.pattern + "\n" + this.regExp + "\n";
        }
    }
}