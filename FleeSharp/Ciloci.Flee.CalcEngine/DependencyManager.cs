using System;
using System.Collections.Generic;

namespace Ciloci.Flee.CalcEngine
{
	internal class DependencyManager<T>
	{
		private Dictionary<T, Dictionary<T, object>> MyDependentsMap;

		private IEqualityComparer<T> MyEqualityComparer;

		private Dictionary<T, int> MyPrecedentsMap;

		public string Precedents
		{
			get
			{
				List<string> list = new List<string>();
				try
				{
					Dictionary<T, int>.Enumerator enumerator = this.MyPrecedentsMap.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<T, int> pair = enumerator.Current;
						list.Add(pair.ToString());
					}
				}
				finally
				{
					Dictionary<T, int>.Enumerator enumerator;
					((IDisposable)enumerator).Dispose();
				}
				return string.Join(Environment.NewLine, list.ToArray());
			}
		}

		public string DependencyGraph
		{
			get
			{
				string[] lines = new string[this.MyDependentsMap.Count - 1 + 1];
				try
				{
					Dictionary<T, Dictionary<T, object>>.Enumerator enumerator = this.MyDependentsMap.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<T, Dictionary<T, object>> pair = enumerator.Current;
						T key = pair.Key;
						string s = this.FormatValues(pair.Value.Keys);
						int index;
						lines[index] = string.Format("{0} -> {1}", key, s);
						index++;
					}
				}
				finally
				{
					Dictionary<T, Dictionary<T, object>>.Enumerator enumerator;
					((IDisposable)enumerator).Dispose();
				}
				return string.Join(Environment.NewLine, lines);
			}
		}

		public int Count
		{
			get
			{
				return this.MyDependentsMap.Count;
			}
		}

		public DependencyManager(IEqualityComparer<T> comparer)
		{
			this.MyEqualityComparer = comparer;
			this.MyDependentsMap = new Dictionary<T, Dictionary<T, object>>(this.MyEqualityComparer);
			this.MyPrecedentsMap = new Dictionary<T, int>(this.MyEqualityComparer);
		}

		private IDictionary<T, object> CreateInnerDictionary()
		{
			return new Dictionary<T, object>(this.MyEqualityComparer);
		}

		private IDictionary<T, object> GetInnerDictionary(T tail)
		{
			Dictionary<T, object> value = null;
			bool flag = this.MyDependentsMap.TryGetValue(tail, out value);
			IDictionary<T, object> GetInnerDictionary;
			if (flag)
			{
				GetInnerDictionary = value;
			}
			else
			{
				GetInnerDictionary = null;
			}
			return GetInnerDictionary;
		}

		public DependencyManager<T> CloneDependents(T[] tails)
		{
			IDictionary<T, object> seenNodes = this.CreateInnerDictionary();
			DependencyManager<T> copy = new DependencyManager<T>(this.MyEqualityComparer);
			checked
			{
				for (int i = 0; i < tails.Length; i++)
				{
					T tail = tails[i];
					this.CloneDependentsInternal(tail, copy, seenNodes);
				}
				return copy;
			}
		}

		private void CloneDependentsInternal(T tail, DependencyManager<T> target, IDictionary<T, object> seenNodes)
		{
			bool flag = seenNodes.ContainsKey(tail);
			if (!flag)
			{
				seenNodes.Add(tail, null);
				target.AddTail(tail);
				IDictionary<T, object> innerDict = this.GetInnerDictionary(tail);
				try
				{
					IEnumerator<T> enumerator = innerDict.Keys.GetEnumerator();
					while (enumerator.MoveNext())
					{
						T head = enumerator.Current;
						target.AddDepedency(tail, head);
						this.CloneDependentsInternal(head, target, seenNodes);
					}
				}
				finally
				{
					IEnumerator<T> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
			}
		}

		public T[] GetTails()
		{
			T[] arr = new T[this.MyDependentsMap.Keys.Count - 1 + 1];
			this.MyDependentsMap.Keys.CopyTo(arr, 0);
			return arr;
		}

		public void Clear()
		{
			this.MyDependentsMap.Clear();
			this.MyPrecedentsMap.Clear();
		}

		public void ReplaceDependency(T old, T replaceWith)
		{
			Dictionary<T, object> value = this.MyDependentsMap[old];
			this.MyDependentsMap.Remove(old);
			this.MyDependentsMap.Add(replaceWith, value);
			try
			{
				Dictionary<T, Dictionary<T, object>>.ValueCollection.Enumerator enumerator = this.MyDependentsMap.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Dictionary<T, object> innerDict = enumerator.Current;
					bool flag = innerDict.ContainsKey(old);
					if (flag)
					{
						innerDict.Remove(old);
						innerDict.Add(replaceWith, null);
					}
				}
			}
			finally
			{
				Dictionary<T, Dictionary<T, object>>.ValueCollection.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
		}

		public void AddTail(T tail)
		{
			bool flag = !this.MyDependentsMap.ContainsKey(tail);
			if (flag)
			{
				this.MyDependentsMap.Add(tail, (Dictionary<T, object>)this.CreateInnerDictionary());
			}
		}

		public void AddDepedency(T tail, T head)
		{
			IDictionary<T, object> innerDict = this.GetInnerDictionary(tail);
			bool flag = !innerDict.ContainsKey(head);
			if (flag)
			{
				innerDict.Add(head, head);
				this.AddPrecedent(head);
			}
		}

		public void RemoveDependency(T tail, T head)
		{
			IDictionary<T, object> innerDict = this.GetInnerDictionary(tail);
			this.RemoveHead(head, innerDict);
		}

		private void RemoveHead(T head, IDictionary<T, object> dict)
		{
			bool flag = dict.Remove(head);
			if (flag)
			{
				this.RemovePrecedent(head);
			}
		}

		public void Remove(T[] tails)
		{
			checked
			{
				try
				{
					Dictionary<T, Dictionary<T, object>>.ValueCollection.Enumerator enumerator = this.MyDependentsMap.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Dictionary<T, object> innerDict = enumerator.Current;
						for (int i = 0; i < tails.Length; i++)
						{
							T tail = tails[i];
							this.RemoveHead(tail, innerDict);
						}
					}
				}
				finally
				{
					Dictionary<T, Dictionary<T, object>>.ValueCollection.Enumerator enumerator;
					((IDisposable)enumerator).Dispose();
				}
				for (int j = 0; j < tails.Length; j++)
				{
					T tail2 = tails[j];
					this.MyDependentsMap.Remove(tail2);
				}
			}
		}

		public void GetDirectDependents(T tail, List<T> dest)
		{
			Dictionary<T, object> innerDict = (Dictionary<T, object>)this.GetInnerDictionary(tail);
			dest.AddRange(innerDict.Keys);
		}

		public T[] GetDependents(T tail)
		{
			Dictionary<T, object> dependents = (Dictionary<T, object>)this.CreateInnerDictionary();
			this.GetDependentsRecursive(tail, dependents);
			T[] arr = new T[dependents.Count - 1 + 1];
			dependents.Keys.CopyTo(arr, 0);
			return arr;
		}

		private void GetDependentsRecursive(T tail, Dictionary<T, object> dependents)
		{
			dependents[tail] = null;
			Dictionary<T, object> directDependents = (Dictionary<T, object>)this.GetInnerDictionary(tail);
			try
			{
				Dictionary<T, object>.KeyCollection.Enumerator enumerator = directDependents.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T pair = enumerator.Current;
					this.GetDependentsRecursive(pair, dependents);
				}
			}
			finally
			{
				Dictionary<T, object>.KeyCollection.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
		}

		public void GetDirectPrecedents(T head, IList<T> dest)
		{
			try
			{
				Dictionary<T, Dictionary<T, object>>.KeyCollection.Enumerator enumerator = this.MyDependentsMap.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T tail = enumerator.Current;
					Dictionary<T, object> innerDict = (Dictionary<T, object>)this.GetInnerDictionary(tail);
					bool flag = innerDict.ContainsKey(head);
					if (flag)
					{
						dest.Add(tail);
					}
				}
			}
			finally
			{
				Dictionary<T, Dictionary<T, object>>.KeyCollection.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
		}

		private void AddPrecedent(T head)
		{
			int count = 0;
			this.MyPrecedentsMap.TryGetValue(head, out count);
			this.MyPrecedentsMap[head] = count + 1;
		}

		private void RemovePrecedent(T head)
		{
			int count = this.MyPrecedentsMap[head] - 1;
			bool flag = count == 0;
			if (flag)
			{
				this.MyPrecedentsMap.Remove(head);
			}
			else
			{
				this.MyPrecedentsMap[head] = count;
			}
		}

		public bool HasPrecedents(T head)
		{
			return this.MyPrecedentsMap.ContainsKey(head);
		}

		public bool HasDependents(T tail)
		{
			Dictionary<T, object> innerDict = (Dictionary<T, object>)this.GetInnerDictionary(tail);
			return innerDict.Count > 0;
		}

		private string FormatValues(ICollection<T> values)
		{
			string[] strings = new string[values.Count - 1 + 1];
			T[] keys = new T[values.Count - 1 + 1];
			values.CopyTo(keys, 0);
			int num = keys.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				strings[i] = keys[i].ToString();
			}
			bool flag = strings.Length == 0;
			string FormatValues;
			if (flag)
			{
				FormatValues = "<empty>";
			}
			else
			{
				FormatValues = string.Join(",", strings);
			}
			return FormatValues;
		}

		public Queue<T> GetSources(T[] rootTails)
		{
			Queue<T> q = new Queue<T>();
			checked
			{
				for (int i = 0; i < rootTails.Length; i++)
				{
					T rootTail = rootTails[i];
					bool flag = !this.HasPrecedents(rootTail);
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
			IList<T> directDependents = new List<T>();
			while (sources.Count > 0)
			{
				T i = sources.Dequeue();
				output.Add(i);
				directDependents.Clear();
				this.GetDirectDependents(i, (List<T>)directDependents);
				try
				{
					IEnumerator<T> enumerator = directDependents.GetEnumerator();
					while (enumerator.MoveNext())
					{
						T j = enumerator.Current;
						this.RemoveDependency(i, j);
						bool flag = !this.HasPrecedents(j);
						if (flag)
						{
							sources.Enqueue(j);
						}
					}
				}
				finally
				{
					IEnumerator<T> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
			}
			bool flag2 = output.Count != this.Count;
			if (flag2)
			{
				throw new CircularReferenceException();
			}
			return output;
		}
	}
}
