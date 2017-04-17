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

namespace FleeSharp.PerCederberg.Grammatica.Runtime.RE
{
    using System;
    using System.IO;

    internal class StringElement : Element
    {
        private readonly string value;

        public StringElement(char c)
            : this(c.ToString())
        {
        }

        public StringElement(string str)
        {
            this.value = str;
        }

        public string GetString()
        {
            return this.value;
        }

        public override object Clone()
        {
            return this;
        }

        public override int Match(Matcher m, LookAheadReader input, int start, int skip)
        {
            var flag = skip != 0;
            int match;
            if (flag)
            {
                match = -1;
            }
            else
            {
                var num = this.value.Length - 1;
                for (var i = 0; i <= num; i++)
                {
                    var c = input.Peek(start + i);
                    var flag2 = c < 0;
                    if (flag2)
                    {
                        m.SetReadEndOfString();
                        match = -1;
                        return match;
                    }
                    var flag3 = m.IsCaseInsensitive();
                    if (flag3)
                    {
                        c = Convert.ToInt32(char.ToLower(Convert.ToChar(c)));
                    }
                    var flag4 = c != Convert.ToInt32(this.value[i]);
                    if (flag4)
                    {
                        match = -1;
                        return match;
                    }
                }
                match = this.value.Length;
            }
            return match;
        }

        public override void PrintTo(TextWriter output, string indent)
        {
            output.WriteLine(indent + "'" + this.value + "'");
        }
    }
}