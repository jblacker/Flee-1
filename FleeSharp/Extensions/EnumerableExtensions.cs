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

namespace Flee.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        /// <summary>
        ///   Iterates through the specified enumerable object.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="enumerable"> The enumerable. </param>
        /// <param name="function"> The function. </param>
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> function)
        {
            foreach (var t in enumerable)
            {
                function(t);
            }
        }

        /// <summary>
        ///   Apply a given function to each element of a collection, returning a new collection with the items altered by function.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <typeparam name="TR"> The type of the new enumerable. </typeparam>
        /// <param name="enumerable"> The enumerable. </param>
        /// <param name="function"> The function. </param>
        /// <returns> </returns>
        public static IEnumerable<TR> Map<T, TR>(this IEnumerable<T> enumerable, Func<T, TR> function)
        {
            foreach (var t in enumerable)
            {
                yield return function(t);
            }
        }
    }
}