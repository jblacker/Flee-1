using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Flee
{
    public sealed class VariableCollection : IDictionary<string, object>
    {
        private IDictionary<string, IVariable> myVariables;

        private readonly ExpressionContext myContext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private EventHandler<ResolveVariableTypeEventArgs> resolveVariableTypeEvent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private EventHandler<ResolveVariableValueEventArgs> resolveVariableValueEvent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private EventHandler<ResolveFunctionEventArgs> resolveFunctionEvent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private EventHandler<InvokeFunctionEventArgs> invokeFunctionEvent;

        public event EventHandler<ResolveVariableTypeEventArgs> ResolveVariableType
        {
            [CompilerGenerated]
            add
            {
                EventHandler<ResolveVariableTypeEventArgs> eventHandler = this.resolveVariableTypeEvent;
                EventHandler<ResolveVariableTypeEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<ResolveVariableTypeEventArgs> value2 = (EventHandler<ResolveVariableTypeEventArgs>)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<ResolveVariableTypeEventArgs>>(ref this.resolveVariableTypeEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                EventHandler<ResolveVariableTypeEventArgs> eventHandler = this.resolveVariableTypeEvent;
                EventHandler<ResolveVariableTypeEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<ResolveVariableTypeEventArgs> value2 = (EventHandler<ResolveVariableTypeEventArgs>)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<ResolveVariableTypeEventArgs>>(ref this.resolveVariableTypeEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }

        public event EventHandler<ResolveVariableValueEventArgs> ResolveVariableValue
        {
            [CompilerGenerated]
            add
            {
                EventHandler<ResolveVariableValueEventArgs> eventHandler = this.resolveVariableValueEvent;
                EventHandler<ResolveVariableValueEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<ResolveVariableValueEventArgs> value2 = (EventHandler<ResolveVariableValueEventArgs>)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<ResolveVariableValueEventArgs>>(ref this.resolveVariableValueEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                EventHandler<ResolveVariableValueEventArgs> eventHandler = this.resolveVariableValueEvent;
                EventHandler<ResolveVariableValueEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<ResolveVariableValueEventArgs> value2 = (EventHandler<ResolveVariableValueEventArgs>)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<ResolveVariableValueEventArgs>>(ref this.resolveVariableValueEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }

        public event EventHandler<ResolveFunctionEventArgs> ResolveFunction
        {
            [CompilerGenerated]
            add
            {
                EventHandler<ResolveFunctionEventArgs> eventHandler = this.resolveFunctionEvent;
                EventHandler<ResolveFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<ResolveFunctionEventArgs> value2 = (EventHandler<ResolveFunctionEventArgs>)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<ResolveFunctionEventArgs>>(ref this.resolveFunctionEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                EventHandler<ResolveFunctionEventArgs> eventHandler = this.resolveFunctionEvent;
                EventHandler<ResolveFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<ResolveFunctionEventArgs> value2 = (EventHandler<ResolveFunctionEventArgs>)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<ResolveFunctionEventArgs>>(ref this.resolveFunctionEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }

        public event EventHandler<InvokeFunctionEventArgs> InvokeFunction
        {
            [CompilerGenerated]
            add
            {
                EventHandler<InvokeFunctionEventArgs> eventHandler = this.invokeFunctionEvent;
                EventHandler<InvokeFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<InvokeFunctionEventArgs> value2 = (EventHandler<InvokeFunctionEventArgs>)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<InvokeFunctionEventArgs>>(ref this.invokeFunctionEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                EventHandler<InvokeFunctionEventArgs> eventHandler = this.invokeFunctionEvent;
                EventHandler<InvokeFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler<InvokeFunctionEventArgs> value2 = (EventHandler<InvokeFunctionEventArgs>)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler<InvokeFunctionEventArgs>>(ref this.invokeFunctionEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }

        public int Count
        {
            get
            {
                return this.myVariables.Count;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[string name]
        {
            get
            {
                IVariable v = this.GetVariable(name, true);
                return v.ValueAsObject;
            }
            set
            {
                IVariable v = null;
                bool flag = this.myVariables.TryGetValue(name, out v);
                if (flag)
                {
                    v.ValueAsObject = RuntimeHelpers.GetObjectValue(value);
                }
                else
                {
                    this.Add(name, RuntimeHelpers.GetObjectValue(value));
                }
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.myVariables.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                Dictionary<string, object> dict = this.GetNameValueDictionary();
                return dict.Values;
            }
        }

        internal VariableCollection(ExpressionContext context)
        {
            this.myContext = context;
            this.CreateDictionary();
            this.HookOptions();
        }

        private void HookOptions()
        {
            this.myContext.Options.CaseSensitiveChanged += new EventHandler(this.OnOptionsCaseSensitiveChanged);
        }

        private void CreateDictionary()
        {
            this.myVariables = new Dictionary<string, IVariable>(this.myContext.Options.StringComparer);
        }

        private void OnOptionsCaseSensitiveChanged(object sender, EventArgs e)
        {
            this.CreateDictionary();
        }

        internal void Copy(VariableCollection dest)
        {
            dest.CreateDictionary();
            dest.HookOptions();
            try
            {
                IEnumerator<KeyValuePair<string, IVariable>> enumerator = this.myVariables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, IVariable> pair = enumerator.Current;
                    IVariable copyVariable = pair.Value.Clone();
                    dest.myVariables.Add(pair.Key, copyVariable);
                }
            }
            finally
            {
                IEnumerator<KeyValuePair<string, IVariable>> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
        }

        internal void DefineVariableInternal(string name, Type variableType, object variableValue)
        {
            Utility.AssertNotNull(variableType, "variableType");
            bool flag = this.myVariables.ContainsKey(name);
            if (flag)
            {
                string msg = Utility.GetGeneralErrorMessage("VariableWithNameAlreadyDefined", new object[]
                {
                    name
                });
                throw new ArgumentException(msg);
            }
            IVariable v = this.CreateVariable(variableType, RuntimeHelpers.GetObjectValue(variableValue));
            this.myVariables.Add(name, v);
        }

        internal Type GetVariableTypeInternal(string name)
        {
            IVariable value = null;
            bool success = this.myVariables.TryGetValue(name, out value);
            bool flag = success;
            Type getVariableTypeInternal;
            if (flag)
            {
                getVariableTypeInternal = value.VariableType;
            }
            else
            {
                ResolveVariableTypeEventArgs args = new ResolveVariableTypeEventArgs(name);
                EventHandler<ResolveVariableTypeEventArgs> resolveVariableTypeEvent = this.resolveVariableTypeEvent;
                if (resolveVariableTypeEvent != null)
                {
                    resolveVariableTypeEvent(this, args);
                }
                getVariableTypeInternal = args.VariableType;
            }
            return getVariableTypeInternal;
        }

        private IVariable GetVariable(string name, bool throwOnNotFound)
        {
            IVariable value = null;
            bool success = this.myVariables.TryGetValue(name, out value);
            bool flag = !success & throwOnNotFound;
            if (flag)
            {
                string msg = Utility.GetGeneralErrorMessage("UndefinedVariable", new object[]
                {
                    name
                });
                throw new ArgumentException(msg);
            }
            return value;
        }

        private IVariable CreateVariable(Type variableValueType, object variableValue)
        {
            IExpression expression = variableValue as IExpression;
            ExpressionOptions options = null;
            bool flag = expression != null;
            if (flag)
            {
                options = expression.Context.Options;
                variableValueType = options.ResultType;
            }
            bool flag2 = expression != null;
            Type variableType;
            if (flag2)
            {
                bool flag3 = !options.IsGeneric;
                if (flag3)
                {
                    variableType = typeof(DynamicExpressionVariable<>);
                }
                else
                {
                    variableType = typeof(GenericExpressionVariable<>);
                }
            }
            else
            {
                this.myContext.AssertTypeIsAccessible(variableValueType);
                variableType = typeof(GenericVariable<>);
            }
            variableType = variableType.MakeGenericType(new Type[]
            {
                variableValueType
            });
            return (IVariable)Activator.CreateInstance(variableType);
        }

        internal Type ResolveOnDemandFunction(string name, Type[] argumentTypes)
        {
            ResolveFunctionEventArgs args = new ResolveFunctionEventArgs(name, argumentTypes);
            EventHandler<ResolveFunctionEventArgs> resolveFunctionEvent = this.resolveFunctionEvent;
            if (resolveFunctionEvent != null)
            {
                resolveFunctionEvent(this, args);
            }
            return args.ReturnType;
        }

        private static T ReturnGenericValue<T>(object value)
        {
            bool flag = value == null;
            T returnGenericValue;
            if (flag)
            {
                returnGenericValue = default(T);
            }
            else
            {
                returnGenericValue = (T)((object)value);
            }
            return returnGenericValue;
        }

        private static void ValidateSetValueType(Type requiredType, object value)
        {
            bool flag = value == null;
            if (!flag)
            {
                Type valueType = value.GetType();
                bool flag2 = !requiredType.IsAssignableFrom(valueType);
                if (flag2)
                {
                    string msg = Utility.GetGeneralErrorMessage("VariableValueNotAssignableToType", new object[]
                    {
                        valueType.Name,
                        requiredType.Name
                    });
                    throw new ArgumentException(msg);
                }
            }
        }

        internal static MethodInfo GetVariableLoadMethod(Type variableType)
        {
            MethodInfo mi = typeof(VariableCollection).GetMethod("GetVariableValueInternal", BindingFlags.Instance | BindingFlags.Public);
            return mi.MakeGenericMethod(new Type[]
            {
                variableType
            });
        }

        internal static MethodInfo GetFunctionInvokeMethod(Type returnType)
        {
            MethodInfo mi = typeof(VariableCollection).GetMethod("GetFunctionResultInternal", BindingFlags.Instance | BindingFlags.Public);
            return mi.MakeGenericMethod(new Type[]
            {
                returnType
            });
        }

        internal static MethodInfo GetVirtualPropertyLoadMethod(Type returnType)
        {
            MethodInfo mi = typeof(VariableCollection).GetMethod("GetVirtualPropertyValueInternal", BindingFlags.Instance | BindingFlags.Public);
            return mi.MakeGenericMethod(new Type[]
            {
                returnType
            });
        }

        private Dictionary<string, object> GetNameValueDictionary()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                IEnumerator<KeyValuePair<string, IVariable>> enumerator = this.myVariables.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, IVariable> pair = enumerator.Current;
                    dict.Add(pair.Key, RuntimeHelpers.GetObjectValue(pair.Value.ValueAsObject));
                }
            }
            finally
            {
                IEnumerator<KeyValuePair<string, IVariable>> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            return dict;
        }

        public Type GetVariableType(string name)
        {
            IVariable v = this.GetVariable(name, true);
            return v.VariableType;
        }

        public void DefineVariable(string name, Type variableType)
        {
            this.DefineVariableInternal(name, variableType, null);
        }

        public T GetVariableValueInternal<T>(string name)
        {
            IGenericVariable<T> v = null;
            IDictionary<string, IVariable> arg140 = this.myVariables;
            IVariable variable = (IVariable)v;
            bool flag = arg140.TryGetValue(name, out variable);
            v = (IGenericVariable<T>)variable;
            bool flag2 = flag;
            T getVariableValueInternal;
            if (flag2)
            {
                getVariableValueInternal = v.GetValue();
            }
            else
            {
                GenericVariable<T> vTemp = new GenericVariable<T>();
                ResolveVariableValueEventArgs args = new ResolveVariableValueEventArgs(name, typeof(T));
                EventHandler<ResolveVariableValueEventArgs> resolveVariableValueEvent = this.resolveVariableValueEvent;
                if (resolveVariableValueEvent != null)
                {
                    resolveVariableValueEvent(this, args);
                }
                ValidateSetValueType(typeof(T), RuntimeHelpers.GetObjectValue(args.VariableValue));
                vTemp.ValueAsObject = RuntimeHelpers.GetObjectValue(args.VariableValue);
                v = vTemp;
                getVariableValueInternal = v.GetValue();
            }
            return getVariableValueInternal;
        }

        public T GetVirtualPropertyValueInternal<T>(string name, object component)
        {
            PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(component));
            PropertyDescriptor pd = coll.Find(name, true);
            object value = RuntimeHelpers.GetObjectValue(pd.GetValue(RuntimeHelpers.GetObjectValue(component)));
            ValidateSetValueType(typeof(T), RuntimeHelpers.GetObjectValue(value));
            return ReturnGenericValue<T>(RuntimeHelpers.GetObjectValue(value));
        }

        public T GetFunctionResultInternal<T>(string name, object[] arguments)
        {
            InvokeFunctionEventArgs args = new InvokeFunctionEventArgs(name, arguments);
            EventHandler<InvokeFunctionEventArgs> invokeFunctionEvent = this.invokeFunctionEvent;
            if (invokeFunctionEvent != null)
            {
                invokeFunctionEvent(this, args);
            }
            object result = RuntimeHelpers.GetObjectValue(args.Result);
            ValidateSetValueType(typeof(T), RuntimeHelpers.GetObjectValue(result));
            return ReturnGenericValue<T>(RuntimeHelpers.GetObjectValue(result));
        }

        //void ICollection<KeyValuePair<string, object>>.Add1(KeyValuePair<string, object> item)
        //{
        //	this.Add(item.Key, RuntimeHelpers.GetObjectValue(item.Value));
        //}

        public void Clear()
        {
            this.myVariables.Clear();
        }

        //bool ICollection<KeyValuePair<string, object>>.Contains1(KeyValuePair<string, object> item)
        //{
        //	return this.ContainsKey(item.Key);
        //}

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Dictionary<string, object> dict = this.GetNameValueDictionary();
            ICollection<KeyValuePair<string, object>> coll = dict;
            coll.CopyTo(array, arrayIndex);
        }

        //bool ICollection<KeyValuePair<string, object>>.Remove1(KeyValuePair<string, object> item)
        //{
        //	this.Remove(item.Key);
        //	bool Remove;
        //	return Remove;
        //}

        public void Add(string name, object value)
        {
            Utility.AssertNotNull(RuntimeHelpers.GetObjectValue(value), "value");
            this.DefineVariableInternal(name, value.GetType(), RuntimeHelpers.GetObjectValue(value));
            this[name] = RuntimeHelpers.GetObjectValue(value);
        }

        public bool ContainsKey(string name)
        {
            return this.myVariables.ContainsKey(name);
        }

        public bool Remove(string name)
        {
            this.myVariables.Remove(name);
            bool remove;
            return remove;
        }


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            Dictionary<string, object> dict = this.GetNameValueDictionary();
            return (IEnumerator<KeyValuePair<string, object>>)dict.GetEnumerator();
        }


        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <returns>true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.</returns>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        public bool TryGetValue(string key, out object value)
        {
            IVariable v = this.GetVariable(key, false);
            bool flag = v != null;
            if (flag)
            {
                value = RuntimeHelpers.GetObjectValue(v.ValueAsObject);
            }
            else
            {
                value = null;
            }
            return v != null;
        }
    }
}