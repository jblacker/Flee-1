namespace Flee.CalculationEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Exceptions;
    using Microsoft.VisualBasic.CompilerServices;

    public class CalculationEngine
	{
		private readonly DependencyManager<ExpressionResultPair> myDependencies;

		private readonly Dictionary<string, ExpressionResultPair> myNameNodeMap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
		private EventHandler<NodeEventArgs> nodeRecalculatedEvent;

		public event EventHandler<NodeEventArgs> NodeRecalculated
		{
			[CompilerGenerated]
			add
			{
				var eventHandler = this.nodeRecalculatedEvent;
				EventHandler<NodeEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					var value2 = (EventHandler<NodeEventArgs>)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref this.nodeRecalculatedEvent, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
			[CompilerGenerated]
			remove
			{
				var eventHandler = this.nodeRecalculatedEvent;
				EventHandler<NodeEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					var value2 = (EventHandler<NodeEventArgs>)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref this.nodeRecalculatedEvent, value2, eventHandler2);
				}
				while (eventHandler != eventHandler2);
			}
		}

		public int Count => this.myDependencies.Count;

	    public string DependencyGraph => this.myDependencies.DependencyGraph;

	    public CalculationEngine()
		{
			this.myDependencies = new DependencyManager<ExpressionResultPair>(new PairEqualityComparer());
			this.myNameNodeMap = new Dictionary<string, ExpressionResultPair>(StringComparer.OrdinalIgnoreCase);
		}

		private void AddTemporaryHead(string headName)
		{
			var pair = new GenericExpressionResultPair<int>();
			pair.SetName(headName);
			var flag = !this.myNameNodeMap.ContainsKey(headName);
			if (flag)
			{
				this.myDependencies.AddTail(pair);
				this.myNameNodeMap.Add(headName, pair);
				return;
			}
			throw new ArgumentException($"An expression already exists at '{headName}'");
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
				var ex = expr_1D;
				this.Clear();
				throw new BatchLoadCompileException(info.Name, info.ExpressionText, ex);
			}
		}

		private ExpressionResultPair GetTail(string tailName)
		{
			Utility.AssertNotNull(tailName, "name");
			ExpressionResultPair pair;
			this.myNameNodeMap.TryGetValue(tailName, out pair);
			return pair;
		}

		private ExpressionResultPair GetTailWithValidate(string tailName)
		{
			Utility.AssertNotNull(tailName, "name");
			var pair = this.GetTail(tailName);
			var flag = pair == null;
			if (flag)
			{
				throw new ArgumentException($"No expression is associated with the name '{tailName}'");
			}
			return pair;
		}

		private string[] GetNames(IList<ExpressionResultPair> pairs)
		{
			var names = new string[pairs.Count - 1 + 1];
			var num = names.Length - 1;
			for (var i = 0; i <= num; i++)
			{
				names[i] = pairs[i].Name;
			}
			return names;
		}

		private ExpressionResultPair[] GetRootTails(string[] roots)
		{
			var flag = roots.Length == 0;
			ExpressionResultPair[] getRootTails;
			if (flag)
			{
				getRootTails = this.myDependencies.GetTails();
			}
			else
			{
				var arr = new ExpressionResultPair[roots.Length - 1 + 1];
				var num = arr.Length - 1;
				for (var i = 0; i <= num; i++)
				{
					arr[i] = this.GetTailWithValidate(roots[i]);
				}
				getRootTails = arr;
			}
			return getRootTails;
		}

		internal void FixTemporaryHead(IDynamicExpression expression, ExpressionContext context, Type resultType)
		{
			var pairType = typeof(GenericExpressionResultPair<>);
			pairType = pairType.MakeGenericType(resultType);
			var pair = (ExpressionResultPair)Activator.CreateInstance(pairType);
			var headName = context.CalcEngineExpressionName;
			pair.SetName(headName);
			pair.SetExpression(expression);
			var oldPair = this.myNameNodeMap[headName];
			this.myDependencies.ReplaceDependency(oldPair, pair);
			this.myNameNodeMap[headName] = pair;
			pair.Recalculate();
		}

		internal void AddDependency(string tailName, ExpressionContext context)
		{
			var actualTail = this.GetTail(tailName);
			var headName = context.CalcEngineExpressionName;
			var actualHead = this.GetTail(headName);
			this.myDependencies.AddDepedency(actualTail, actualHead);
		}

		internal Type ResolveTailType(string tailName)
		{
			var actualTail = this.GetTail(tailName);
			return actualTail.ResultType;
		}

		internal bool HasTail(string tailName)
		{
			return this.myNameNodeMap.ContainsKey(tailName);
		}

		internal void EmitLoad(string tailName, FleeIlGenerator ilg)
		{
			var pi = typeof(ExpressionContext).GetProperty("CalculationEngine");
			ilg.Emit(OpCodes.Callvirt, pi.GetGetMethod());
			var methods = typeof(CalculationEngine).FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public, Type.FilterNameIgnoreCase, "GetResult");
			MethodInfo mi = null;
			var array = methods;
			checked
			{
				for (var i = 0; i < array.Length; i++)
				{
					var method = (MethodInfo)array[i];
					var isGenericMethod = method.IsGenericMethod;
					if (isGenericMethod)
					{
						mi = method;
						break;
					}
				}
				var resultType = this.ResolveTailType(tailName);
			    if (mi != null)
			    {
			        mi = mi.MakeGenericMethod(resultType);
			        ilg.Emit(OpCodes.Ldstr, tailName);
			        ilg.Emit(OpCodes.Call, mi);
			    }
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
			var tail = this.GetTail(name);
			var flag = tail == null;
			checked
			{
				bool remove;
				if (flag)
				{
					remove = false;
				}
				else
				{
					var dependents = this.myDependencies.GetDependents(tail);
					this.myDependencies.Remove(dependents);
					var array = dependents;
					for (var i = 0; i < array.Length; i++)
					{
						var pair = array[i];
						this.myNameNodeMap.Remove(pair.Name);
					}
					remove = true;
				}
				return remove;
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
			var infos = loader.GetBachInfos();
			var array = infos;
			checked
			{
				for (var i = 0; i < array.Length; i++)
				{
					var info = array[i];
					this.DoBatchLoadAdd(info);
				}
			}
		}

		public T GetResult<T>(string name)
		{
			var tail = this.GetTailWithValidate(name);
			var flag = typeof(T) != tail.ResultType;
			if (flag)
			{
				var msg = $"The result type of '{name}' ('{tail.ResultType.Name}') does not match the supplied type argument ('{typeof(T).Name}')";
				throw new ArgumentException(msg);
			}
			var actualTail = (GenericExpressionResultPair<T>)tail;
			return actualTail.ResultPair;
		}

		public object GetResult(string name)
		{
			var tail = this.GetTailWithValidate(name);
			return tail.ResultAsObject;
		}

		public IExpression GetExpression(string name)
		{
			var tail = this.GetTailWithValidate(name);
			return tail.Expression;
		}

		public string[] GetDependents(string name)
		{
			var pair = this.GetTail(name);
			var dependents = new List<ExpressionResultPair>();
			var flag = pair != null;
			if (flag)
			{
				this.myDependencies.GetDirectDependents(pair, dependents);
			}
			return this.GetNames(dependents);
		}

		public string[] GetPrecedents(string name)
		{
			var pair = this.GetTail(name);
			var dependents = new List<ExpressionResultPair>();
			var flag = pair != null;
			if (flag)
			{
				this.myDependencies.GetDirectPrecedents(pair, dependents);
			}
			return this.GetNames(dependents);
		}

		public bool HasDependents(string name)
		{
			var pair = this.GetTail(name);
			return pair != null && this.myDependencies.HasDependents(pair);
		}

		public bool HasPrecedents(string name)
		{
			var pair = this.GetTail(name);
			return pair != null && this.myDependencies.HasPrecedents(pair);
		}

		public bool Contains(string name)
		{
			Utility.AssertNotNull(name, "name");
			return this.myNameNodeMap.ContainsKey(name);
		}

	    public void Recalculate(params string[] roots)
	    {
	        var rootTails = this.GetRootTails(roots);
	        var tempDependents = this.myDependencies.CloneDependents(rootTails);
	        var sources = tempDependents.GetSources(rootTails);
	        var calcList = tempDependents.TopologicalSort(sources);
	        var args = new NodeEventArgs();
            
	        foreach (var pair in calcList)
	        {
	            pair.Recalculate();

	            args.SetData(pair.Name, RuntimeHelpers.GetObjectValue(pair.ResultAsObject));
	            var nodeRecalculatedEvent1 = this.nodeRecalculatedEvent;
	            nodeRecalculatedEvent1?.Invoke(this, args);
	        }
	    }

	    public void Clear()
		{
			this.myDependencies.Clear();
			this.myNameNodeMap.Clear();
		}
	}
}
