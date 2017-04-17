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
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class StringTokenMatcher : TokenMatcher
    {
        private readonly bool ignoreCase;
        private readonly ArrayList patterns;

        private readonly Automaton start;

        private TokenPattern tokenPattern;

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
            var flag = this.tokenPattern == null;
            var getMatchedLength = flag ? 0 : this.tokenPattern.Pattern.Length;
            return getMatchedLength;
        }

        public TokenPattern GetPattern(int id)
        {
            var num = this.patterns.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var pattern = (TokenPattern) this.patterns[i];
                var flag = pattern.Id == id;
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
            this.tokenPattern = (TokenPattern) this.start.MatchFrom(input, 0, this.ignoreCase);
            return this.tokenPattern != null;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            var num = this.patterns.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                buffer.Append(RuntimeHelpers.GetObjectValue(this.patterns[i]));
                buffer.Append("\n\n");
            }
            return buffer.ToString();
        }
    }
}