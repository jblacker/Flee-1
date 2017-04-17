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
namespace FleeSharp.Tests
{
    using System;

    public class Monitor
    {
        public int I;

        public string S;

        public DateTime DT;

        public static string SharedString = "string";

        public DateTime this[int i] => this.DT;

        public DateTime this[double d, string s] => this.DT;

        public int this[string s, params int[] args] => -100;

        public Monitor()
        {
            this.I = 900;
            this.S = "monitor";
            this.DT = new DateTime(2007, 1, 1);
        }

        public int GetI()
        {
            return this.I;
        }

        public static implicit operator double(Monitor value)
        {
            return 1.0;
        }
    }
}