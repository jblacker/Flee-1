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

namespace Flee.CalculationEngine
{
    using System;
    using System.Collections.Generic;
    using Exceptions;

    internal class DependencyManager<T>
    {
        private readonly Dictionary<T, Dictionary<T, object>> myDependentsMap;
        private readonly IEqualityComparer<T> myEqualityComparer;
        private readonly Dictionary<T, int> myPrecedentsMap;

        public DependencyManager(IEqualityComparer<T> comparer)
        {
            this.myEqualityComparer = comparer;
            this.myDependentsMap = new Dictionary<T, Dictionary<T, object>>(this.myEqualityComparer);
            this.myPrecedentsMap = new Dictionary<T, int>(this.myEqualityComparer);
        }

        private IDictionary<T, object> CreateInnerDictionary()
        {
            return new Dictionary<T, object>(this.myEqualityComparer);
        }

        private IDictionary<T, object> GetInnerDictionary(T tail)
        {
            Dictionary<T, object> value;
            var flag = this.myDependentsMap.TryGetValue(tail, out value);
            IDictionary<T, object> getInnerDictionary = flag ? value : null;
            return getInnerDictionary;
        }

        public DependencyManager<T> CloneDependents(T[] tails)
        {
            var seenNodes = this.CreateInnerDictionary();
            var copy = new DependencyManager<T>(this.myEqualityComparer);
            checked
            {
                for (var i = 0; i < tails.Length; i++)
                {
                    var tail = tails[i];
                    this.CloneDependentsInternal(tail, copy, seenNodes);
                }
                return copy;
            }
        }

        private void CloneDependentsInternal(T tail, DependencyManager<T> target, IDictionary<T, object> seenNodes)
        {
            if (seenNodes.ContainsKey(tail))
            {
                return;
            }
            seenNodes.Add(tail, null);
            target.AddTail(tail);

            var innerDict = this.GetInnerDictionary(tail);

            foreach (var head in innerDict.Keys)
            {
                target.AddDepedency(tail, head);
                this.CloneDependentsInternal(head, target, seenNodes);
            }
        }

        public T[] GetTails()
        {
            var arr = new T[this.myDependentsMap.Keys.Count - 1 + 1];
            this.myDependentsMap.Keys.CopyTo(arr, 0);
            return arr;
        }

        public void Clear()
        {
            this.myDependentsMap.Clear();
            this.myPrecedentsMap.Clear();
        }

        public void ReplaceDependency(T old, T replaceWith)
        {
            var value = this.myDependentsMap[old];
            this.myDependentsMap.Remove(old);
            this.myDependentsMap.Add(replaceWith, value);

            foreach (var e in this.myDependentsMap.Values)
            {
                if (e.ContainsKey(old))
                {
                    e.Remove(old);
                    e.Add(replaceWith, null);
                }
            }
        }

        public void AddTail(T tail)
        {
            var flag = !this.myDependentsMap.ContainsKey(tail);
            if (flag)
            {
                this.myDependentsMap.Add(tail, (Dictionary<T, object>) this.CreateInnerDictionary());
            }
        }

        public void AddDepedency(T tail, T head)
        {
            var innerDict = this.GetInnerDictionary(tail);
            var flag = !innerDict.ContainsKey(head);
            if (flag)
            {
                innerDict.Add(head, head);
                this.AddPrecedent(head);
            }
        }

        public void RemoveDependency(T tail, T head)
        {
            var innerDict = this.GetInnerDictionary(tail);
            this.RemoveHead(head, innerDict);
        }

        private void RemoveHead(T head, IDictionary<T, object> dict)
        {
            var flag = dict.Remove(head);
            if (flag)
            {
                this.RemovePrecedent(head);
            }
        }

        public void Remove(T[] tails)
        {
            foreach (var dependent in this.myDependentsMap.Values)
            {
                foreach (var tail in tails)
                {
                    this.RemoveHead(tail, dependent);
                }
            }

            foreach (var tail in tails)
            {
                this.myDependentsMap.Remove(tail);
            }
        }

        public void GetDirectDependents(T tail, List<T> dest)
        {
            var innerDict = (Dictionary<T, object>) this.GetInnerDictionary(tail);
            dest.AddRange(innerDict.Keys);
        }

        public T[] GetDependents(T tail)
        {
            var dependents = (Dictionary<T, object>) this.CreateInnerDictionary();
            this.GetDependentsRecursive(tail, dependents);
            var arr = new T[dependents.Count - 1 + 1];
            dependents.Keys.CopyTo(arr, 0);
            return arr;
        }

        private void GetDependentsRecursive(T tail, Dictionary<T, object> dependents)
        {
            dependents[tail] = null;
            var directDependents = (Dictionary<T, object>) this.GetInnerDictionary(tail);

            foreach (var pair in directDependents.Keys)
            {
                this.GetDependentsRecursive(pair, dependents);
            }
        }

        public void GetDirectPrecedents(T head, IList<T> dest)
        {
            foreach (var t in this.myDependentsMap.Keys)
            {
                var innerDict = (Dictionary<T, object>) this.GetInnerDictionary(t);
                if (innerDict.ContainsKey(head))
                {
                    dest.Add(t);
                }
            }
        }

        private void AddPrecedent(T head)
        {
            int count;
            this.myPrecedentsMap.TryGetValue(head, out count);
            this.myPrecedentsMap[head] = count + 1;
        }

        private void RemovePrecedent(T head)
        {
            var count = this.myPrecedentsMap[head] - 1;
            var flag = count == 0;
            if (flag)
            {
                this.myPrecedentsMap.Remove(head);
            }
            else
            {
                this.myPrecedentsMap[head] = count;
            }
        }

        public bool HasPrecedents(T head)
        {
            return this.myPrecedentsMap.ContainsKey(head);
        }

        public bool HasDependents(T tail)
        {
            var innerDict = (Dictionary<T, object>) this.GetInnerDictionary(tail);
            return innerDict.Count > 0;
        }

        private string FormatValues(ICollection<T> values)
        {
            var strings = new string[values.Count - 1 + 1];
            var keys = new T[values.Count - 1 + 1];
            values.CopyTo(keys, 0);
            var num = keys.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                strings[i] = keys[i].ToString();
            }
            var flag = strings.Length == 0;
            var formatValues = flag ? "<empty>" : string.Join(",", strings);
            return formatValues;
        }

        public Queue<T> GetSources(T[] rootTails)
        {
            var q = new Queue<T>();
            checked
            {
                for (var i = 0; i < rootTails.Length; i++)
                {
                    var rootTail = rootTails[i];
                    var flag = !this.HasPrecedents(rootTail);
                    if (flag)
                    {
                        q.Enqueue(rootTail);
                    }
                }
                return q;
            }
        }

        public IList<T> TopologicalSort(Queue<T> sources)
        {
            IList<T> output = new List<T>();
            var directDependents = new List<T>();

            while (sources.Count > 0)
            {
                var n = sources.Dequeue();
                output.Add(n);

                directDependents.Clear();
                this.GetDirectDependents(n, directDependents);

                foreach (var m in directDependents)
                {
                    this.RemoveDependency(n, m);

                    if (this.HasPrecedents(m) == false)
                    {
                        sources.Enqueue(m);
                    }
                }
            }

            if (output.Count != this.Count)
            {
                throw new CircularReferenceException();
            }

            return output;
        }

        public int Count => this.myDependentsMap.Count;

        public string DependencyGraph
        {
            get
            {
                var lines = new string[this.myDependentsMap.Count - 1 + 1];
                using (var enumerator = this.myDependentsMap.GetEnumerator())
                {
                    var index = 0;
                    while (enumerator.MoveNext())
                    {
                        var pair = enumerator.Current;
                        var key = pair.Key;
                        var s = this.FormatValues(pair.Value.Keys);

                        lines[index] = $"{key} -> {s}";
                        index++;
                    }
                }
                return string.Join(Environment.NewLine, lines);
            }
        }

        public string Precedents
        {
            get
            {
                var list = new List<string>();

                using (var enumerator = this.myPrecedentsMap.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var pair = enumerator.Current;
                        list.Add(pair.ToString());
                    }
                }
                return string.Join(Environment.NewLine, list.ToArray());
            }
        }
    }
}