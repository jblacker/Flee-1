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

namespace Flee.PerCederberg.Grammatica.Runtime.RE
{
    using System.IO;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Matcher
    {
        private readonly Element element;

        private readonly bool ignoreCase;

        private bool endOfString;

        private LookAheadReader input;

        private int length;

        private int start;

        internal Matcher(Element e, LookAheadReader input, bool ignoreCase)
        {
            this.element = e;
            this.input = input;
            this.ignoreCase = ignoreCase;
            this.Reset();
        }

        public bool IsCaseInsensitive()
        {
            return this.ignoreCase;
        }

        public void Reset()
        {
            this.length = -1;
            this.endOfString = false;
        }

        public void Reset(string str)
        {
            this.Reset(new StringReader(str));
        }

        public void Reset(TextReader input)
        {
            var flag = input is LookAheadReader;
            if (flag)
            {
                this.Reset((LookAheadReader) input);
            }
            else
            {
                this.Reset(new LookAheadReader(input));
            }
        }

        private void Reset(LookAheadReader input)
        {
            this.input = input;
            this.Reset();
        }

        public int Start()
        {
            return this.start;
        }

        public int End()
        {
            var flag = this.length > 0;
            int end;
            if (flag)
            {
                end = this.start + this.length;
            }
            else
            {
                end = this.start;
            }
            return end;
        }

        public int Length()
        {
            return this.length;
        }

        public bool HasReadEndOfString()
        {
            return this.endOfString;
        }

        public bool MatchFromBeginning()
        {
            return this.MatchFrom(0);
        }

        public bool MatchFrom(int pos)
        {
            this.Reset();
            this.start = pos;
            this.length = this.element.Match(this, this.input, this.start, 0);
            return this.length >= 0;
        }

        public override string ToString()
        {
            var flag = this.length <= 0;
            string toString;
            if (flag)
            {
                toString = "";
            }
            else
            {
                try
                {
                    toString = this.input.PeekString(this.start, this.length);
                }
                catch (IOException exception)
                {
                    ProjectData.SetProjectError(exception);
                    toString = "";
                    ProjectData.ClearProjectError();
                }
            }
            return toString;
        }

        internal void SetReadEndOfString()
        {
            this.endOfString = true;
        }
    }
}