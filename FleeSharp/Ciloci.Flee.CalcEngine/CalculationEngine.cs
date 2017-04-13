using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Ciloci.Flee.CalcEngine
{
	public class CalculationEngine
	{
		private DependencyManager<ExpressionResultPair> MyDependencies;

		private Dictionary<string, ExpressionResultPair> MyNameNodeMap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
		private EventHandler<NodeEventArgs> NodeRecalculatedEvent;

		public event EventHandler<NodeEventArgs> NodeRecalculated
		{
			[CompilerGenerated]
			add
			{
				EventHandler<NodeEventArgs> eventHandler = this.NodeRecalculatedEvent;
				EventHandler<NodeEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler<NodeEventArgs> value2 = (EventHandler<NodeEventArgs>)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange<EventHandler<NodeEventArgs>>(ref this.NodeRecalculatedEvent, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<NodeEventArgs> eventHandler = this.NodeRecalculatedEvent;
				EventHandler<NodeEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler<NodeEventArgs> value2 = (EventHandler<NodeEventArgs>)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange<EventHandler<NodeEventArgs>>(ref this.NodeRecalculatedEvent, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public int Count
		{
			get
			{
				return this.MyDependencies.Count;
			}
		}

		public string DependencyGraph
		{
			get
			{
				return this.MyDependencies.DependencyGraph;
			}
		}

		public CalculationEngine()
		{
			this.MyDependencies = new DependencyManager<ExpressionResultPair>(new PairEqualityComparer());
			this.MyNameNodeMap = new Dictionary<string, ExpressionResultPair>(StringComparer.OrdinalIgnoreCase);
		}

		private void AddTemporaryHead(string headName)
		{
			GenericExpressionResultPair<int> pair = new GenericExpressionResultPair<int>();
			pair.SetName(headName);
			bool flag = !this.MyNameNodeMap.ContainsKey(headName);
			if (flag)
			{
				this.MyDependencies.AddTail(pair);
				this.MyNameNodeMap.Add(headName, pair);
				return;
			}
			throw new ArgumentException(string.Format("An expression already exists at '{0}'", headName));
		}

		private void DoBatchLoadAdd(BatchLoadInfo info)
		{
			try
			{
				this.Add(info.Name, info.ExpressionText, info.Context);
			}
			catch (ExpressionCompileException expr_1D)
			{
				ProjectData.SetProjectError(expr_1D);
				ExpressionCompileException ex = expr_1D;
				this.Clear();
				throw new BatchLoadCompileException(info.Name, info.ExpressionText, ex);
			}
		}

		private ExpressionResultPair GetTail(string tailName)
		{
			Utility.AssertNotNull(tailName, "name");
			ExpressionResultPair pair = null;
			this.MyNameNodeMap.TryGetValue(tailName, out pair);
			return pair;
		}

		private ExpressionResultPair GetTailWithValidate(string tailName)
		{
			Utility.AssertNotNull(tailName, "name");
			ExpressionResultPair pair = this.GetTail(tailName);
			bool flag = pair == null;
			if (flag)
			{
				throw new ArgumentException(string.Format("No expression is associated with the name '{0}'", tailName));
			}
			return pair;
		}

		private string[] GetNames(IList<ExpressionResultPair> pairs)
		{
			string[] names = new string[pairs.Count - 1 + 1];
			int num = names.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				names[i] = pairs[i].Name;
			}
			return names;
		}

		private ExpressionResultPair[] GetRootTails(string[] roots)
		{
			bool flag = roots.Length == 0;
			ExpressionResultPair[] GetRootTails;
			if (flag)
			{
				GetRootTails = this.MyDependencies.GetTails();
			}
			else
			{
				ExpressionResultPair[] arr = new ExpressionResultPair[roots.Length - 1 + 1];
				int num = arr.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					arr[i] = this.GetTailWithValidate(roots[i]);
				}
				GetRootTails = arr;
			}
			return GetRootTails;
		}

		internal void FixTemporaryHead(IDynamicExpression expression, ExpressionContext context, Type resultType)
		{
			Type pairType = typeof(GenericExpressionResultPair<>);
			pairType = pairType.MakeGenericType(new Type[]
			{
				resultType
			});
			ExpressionResultPair pair = (ExpressionResultPair)Activator.CreateInstance(pairType);
			string headName = context.CalcEngineExpressionName;
			pair.SetName(headName);
			pair.SetExpression(expression);
			ExpressionResultPair oldPair = this.MyNameNodeMap[headName];
			this.MyDependencies.ReplaceDependency(oldPair, pair);
			this.MyNameNodeMap[headName] = pair;
			pair.Recalculate();
		}

		internal void AddDependency(string tailName, ExpressionContext context)
		{
			ExpressionResultPair actualTail = this.GetTail(tailName);
			string headName = context.CalcEngineExpressionName;
			ExpressionResultPair actualHead = this.GetTail(headName);
			this.MyDependencies.AddDepedency(actualTail, actualHead);
		}

		internal Type ResolveTailType(string tailName)
		{
			ExpressionResultPair actualTail = this.GetTail(tailName);
			return actualTail.ResultType;
		}

		internal bool HasTail(string tailName)
		{
			return this.MyNameNodeMap.ContainsKey(tailName);
		}

		internal void EmitLoad(string tailName, FleeILGenerator ilg)
		{
			PropertyInfo pi = typeof(ExpressionContext).GetProperty("CalculationEngine");
			ilg.Emit(OpCodes.Callvirt, pi.GetGetMethod());
			MemberInfo[] methods = typeof(CalculationEngine).FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, Type.FilterNameIgnoreCase, "GetResult");
			MethodInfo mi = null;
			MemberInfo[] array = methods;
			checked
			{
				for (int i = 0; i < array.Length; i++)
				{
					MethodInfo method = (MethodInfo)array[i];
					bool isGenericMethod = method.IsGenericMethod;
					if (isGenericMethod)
					{
						mi = method;
						break;
					}
				}
				Type resultType = this.ResolveTailType(tailName);
				mi = mi.MakeGenericMethod(new Type[]
				{
					resultType
				});
				ilg.Emit(OpCodes.Ldstr, tailName);
				ilg.Emit(OpCodes.Call, mi);
			}
		}

		public void Add(string atomName, string expression, ExpressionContext context)
		{
			Utility.AssertNotNull(atomName, "atomName");
			Utility.AssertNotNull(expression, "expression");
			Utility.AssertNotNull(context, "context");
			this.AddTemporaryHead(atomName);
			context.SetCalcEngine(this, atomName);
			context.CompileDynamic(expression);
		}

		public bool Remove(string name)
		{
			ExpressionResultPair tail = this.GetTail(name);
			bool flag = tail == null;
			checked
			{
				bool Remove;
				if (flag)
				{
					Remove = false;
				}
				else
				{
					ExpressionResultPair[] dependents = this.MyDependencies.GetDependents(tail);
					this.MyDependencies.Remove(dependents);
					ExpressionResultPair[] array = dependents;
					for (int i = 0; i < array.Length; i++)
					{
						ExpressionResultPair pair = array[i];
						this.MyNameNodeMap.Remove(pair.Name);
					}
					Remove = true;
				}
				return Remove;
			}
		}

		public BatchLoader CreateBatchLoader()
		{
			return new BatchLoader();
		}

		public void BatchLoad(BatchLoader loader)
		{
			Utility.AssertNotNull(loader, "loader");
			this.Clear();
			BatchLoadInfo[] infos = loader.GetBachInfos();
			BatchLoadInfo[] array = infos;
			checked
			{
				for (int i = 0; i < array.Length; i++)
				{
					BatchLoadInfo info = array[i];
					this.DoBatchLoadAdd(info);
				}
			}
		}

		public T GetResult<T>(string name)
		{
			ExpressionResultPair tail = this.GetTailWithValidate(name);
			bool flag = typeof(T) != tail.ResultType;
			if (flag)
			{
				string msg = string.Format("The result type of '{0}' ('{1}') does not match the supplied type argument ('{2}')", name, tail.ResultType.Name, typeof(T).Name);
				throw new ArgumentException(msg);
			}
			GenericExpressionResultPair<T> actualTail = (GenericExpressionResultPair<T>)tail;
			return actualTail.Result;
		}

		public object GetResult(string name)
		{
			ExpressionResultPair tail = this.GetTailWithValidate(name);
			return tail.ResultAsObject;
		}

		public IExpression GetExpression(string name)
		{
			ExpressionResultPair tail = this.GetTailWithValidate(name);
			return tail.Expression;
		}

		public string[] GetDependents(string name)
		{
			ExpressionResultPair pair = this.GetTail(name);
			List<ExpressionResultPair> dependents = new List<ExpressionResultPair>();
			bool flag = pair != null;
			if (flag)
			{
				this.MyDependencies.GetDirectDependents(pair, dependents);
			}
			return this.GetNames(dependents);
		}

		public string[] GetPrecedents(string name)
		{
			ExpressionResultPair pair = this.GetTail(name);
			List<ExpressionResultPair> dependents = new List<ExpressionResultPair>();
			bool flag = pair != null;
			if (flag)
			{
				this.MyDependencies.GetDirectPrecedents(pair, dependents);
			}
			return this.GetNames(dependents);
		}

		public bool HasDependents(string name)
		{
			ExpressionResultPair pair = this.GetTail(name);
			return pair != null && this.MyDependencies.HasDependents(pair);
		}

		public bool HasPrecedents(string name)
		{
			ExpressionResultPair pair = this.GetTail(name);
			return pair != null && this.MyDependencies.HasPrecedents(pair);
		}

		public bool Contains(string name)
		{
			Utility.AssertNotNull(name, "name");
			return this.MyNameNodeMap.ContainsKey(name);
		}

		public void Recalculate(params string[] roots)
		{
			ExpressionResultPair[] rootTails = this.GetRootTails(roots);
			DependencyManager<ExpressionResultPair> tempDependents = this.MyDependencies.CloneDependents(rootTails);
			Queue<ExpressionResultPair> sources = tempDependents.GetSources(rootTails);
			IList<ExpressionResultPair> calcList = tempDependents.TopologicalSort(sources);
			NodeEventArgs args = new NodeEventArgs();
			try
			{
				IEnumerator<ExpressionResultPair> enumerator = calcList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ExpressionResultPair pair = enumerator.Current;
					pair.Recalculate();
					args.SetData(pair.Name, RuntimeHelpers.GetObjectValue(pair.ResultAsObject));
					EventHandler<NodeEventArgs> nodeRecalculatedEvent = this.NodeRecalculatedEvent;
					if (nodeRecalculatedEvent != null)
					{
						nodeRecalculatedEvent(this, args);
					}
				}
			}
			finally
			{
				IEnumerator<ExpressionResultPair> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}

		public void Clear()
		{
			this.MyDependencies.Clear();
			this.MyNameNodeMap.Clear();
		}
	}
}
