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

namespace FleeSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Extensions;

    public sealed class VariableCollection : IDictionary<string, object>
    {
        private readonly ExpressionContext myContext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private EventHandler<InvokeFunctionEventArgs> invokeFunctionEvent;

        private IDictionary<string, IVariable> myVariables;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private EventHandler<ResolveFunctionEventArgs> resolveFunctionEvent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private EventHandler<ResolveVariableTypeEventArgs> resolveVariableTypeEvent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private EventHandler<ResolveVariableValueEventArgs> resolveVariableValueEvent;

        internal VariableCollection(ExpressionContext context)
        {
            this.myContext = context;
            this.CreateDictionary();
            this.HookOptions();
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

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        //void ICollection<KeyValuePair<string, object>>.Add1(KeyValuePair<string, object> item)
        //{
        //	this.Add(item.Key, RuntimeHelpers.GetObjectValue(item.Value));
        //}

        public void Clear()
        {
            this.myVariables.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string name)
        {
            return this.myVariables.ContainsKey(name);
        }

        //bool ICollection<KeyValuePair<string, object>>.Contains1(KeyValuePair<string, object> item)
        //{
        //	return this.ContainsKey(item.Key);
        //}

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            var dict = this.GetNameValueDictionary();
            ICollection<KeyValuePair<string, object>> coll = dict;
            coll.CopyTo(array, arrayIndex);
        }

        public int Count => this.myVariables.Count;


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            var dict = this.GetNameValueDictionary();
            return dict.GetEnumerator();
        }


        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

        public object this[string name]
        {
            get
            {
                var v = this.GetVariable(name, true);
                return v.ValueAsObject;
            }
            set
            {
                IVariable v;
                var flag = this.myVariables.TryGetValue(name, out v);
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

        public ICollection<string> Keys => this.myVariables.Keys;

        public bool Remove(string name)
        {
            this.myVariables.Remove(name);
            return false;
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
            var v = this.GetVariable(key, false);
            var flag = v != null;
            value = flag ? RuntimeHelpers.GetObjectValue(v.ValueAsObject) : null;
            return v != null;
        }

        public ICollection<object> Values
        {
            get
            {
                var dict = this.GetNameValueDictionary();
                return dict.Values;
            }
        }

        public event EventHandler<ResolveVariableTypeEventArgs> ResolveVariableType
        {
            [CompilerGenerated]
            add
            {
                var eventHandler = this.resolveVariableTypeEvent;
                EventHandler<ResolveVariableTypeEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<ResolveVariableTypeEventArgs>) Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.resolveVariableTypeEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                var eventHandler = this.resolveVariableTypeEvent;
                EventHandler<ResolveVariableTypeEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<ResolveVariableTypeEventArgs>) Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.resolveVariableTypeEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
        }

        public event EventHandler<ResolveVariableValueEventArgs> ResolveVariableValue
        {
            [CompilerGenerated]
            add
            {
                var eventHandler = this.resolveVariableValueEvent;
                EventHandler<ResolveVariableValueEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<ResolveVariableValueEventArgs>) Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.resolveVariableValueEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                var eventHandler = this.resolveVariableValueEvent;
                EventHandler<ResolveVariableValueEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<ResolveVariableValueEventArgs>) Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.resolveVariableValueEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
        }

        public event EventHandler<ResolveFunctionEventArgs> ResolveFunction
        {
            [CompilerGenerated]
            add
            {
                var eventHandler = this.resolveFunctionEvent;
                EventHandler<ResolveFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<ResolveFunctionEventArgs>) Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.resolveFunctionEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                var eventHandler = this.resolveFunctionEvent;
                EventHandler<ResolveFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<ResolveFunctionEventArgs>) Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.resolveFunctionEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
        }

        public event EventHandler<InvokeFunctionEventArgs> InvokeFunction
        {
            [CompilerGenerated]
            add
            {
                var eventHandler = this.invokeFunctionEvent;
                EventHandler<InvokeFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<InvokeFunctionEventArgs>) Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.invokeFunctionEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                var eventHandler = this.invokeFunctionEvent;
                EventHandler<InvokeFunctionEventArgs> eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler<InvokeFunctionEventArgs>) Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.invokeFunctionEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
        }

        private void HookOptions()
        {
            this.myContext.Options.CaseSensitiveChanged += this.OnOptionsCaseSensitiveChanged;
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

            foreach (var pair in this.myVariables)
            {
                var copyVariable = pair.Value.Clone();
                dest.myVariables.Add(pair.Key, copyVariable);
            }
            //try
            //{
            //    var enumerator = this.myVariables.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var pair = enumerator.Current;
            //        var copyVariable = pair.Value.Clone();
            //        dest.myVariables.Add(pair.Key, copyVariable);
            //    }
            //}
            //finally
            //{
            //    IEnumerator<KeyValuePair<string, IVariable>> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
        }

        internal void DefineVariableInternal(string name, Type variableType, object variableValue)
        {
            Utility.AssertNotNull(variableType, "variableType");
            var flag = this.myVariables.ContainsKey(name);
            if (flag)
            {
                var msg = Utility.GetGeneralErrorMessage("VariableWithNameAlreadyDefined", name);
                throw new ArgumentException(msg);
            }
            var v = this.CreateVariable(variableType, RuntimeHelpers.GetObjectValue(variableValue));
            this.myVariables.Add(name, v);
        }

        internal Type GetVariableTypeInternal(string name)
        {
            IVariable value;
            var success = this.myVariables.TryGetValue(name, out value);
            var flag = success;
            Type getVariableTypeInternal;
            if (flag)
            {
                getVariableTypeInternal = value.VariableType;
            }
            else
            {
                var args = new ResolveVariableTypeEventArgs(name);
                var resolveVariableTypeEvent1 = this.resolveVariableTypeEvent;
                resolveVariableTypeEvent1?.Invoke(this, args);
                getVariableTypeInternal = args.VariableType;
            }
            return getVariableTypeInternal;
        }

        private IVariable GetVariable(string name, bool throwOnNotFound)
        {
            IVariable value;
            var success = this.myVariables.TryGetValue(name, out value);
            var flag = !success & throwOnNotFound;
            if (flag)
            {
                var msg = Utility.GetGeneralErrorMessage("UndefinedVariable", name);
                throw new ArgumentException(msg);
            }
            return value;
        }

        private IVariable CreateVariable(Type variableValueType, object variableValue)
        {
            var expression = variableValue as IExpression;
            ExpressionOptions options = null;
            var flag = expression != null;
            if (flag)
            {
                options = expression.Context.Options;
                variableValueType = options.ResultType;
            }
            var flag2 = expression != null;
            Type variableType;
            if (flag2)
            {
                var flag3 = !options.IsGeneric;
                variableType = flag3 ? typeof(DynamicExpressionVariable<>) : typeof(GenericExpressionVariable<>);
            }
            else
            {
                this.myContext.AssertTypeIsAccessible(variableValueType);
                variableType = typeof(GenericVariable<>);
            }
            variableType = variableType.MakeGenericType(variableValueType);
            return (IVariable) Activator.CreateInstance(variableType);
        }

        internal Type ResolveOnDemandFunction(string name, Type[] argumentTypes)
        {
            var args = new ResolveFunctionEventArgs(name, argumentTypes);
            var resolveFunctionEvent1 = this.resolveFunctionEvent;
            resolveFunctionEvent1?.Invoke(this, args);
            return args.ReturnType;
        }

        private static T ReturnGenericValue<T>(object value)
        {
            var flag = value == null;
            T returnGenericValue;
            if (flag)
            {
                returnGenericValue = default(T);
            }
            else
            {
                returnGenericValue = (T) value;
            }
            return returnGenericValue;
        }

        private static void ValidateSetValueType(Type requiredType, object value)
        {
            var flag = value == null;
            if (!flag)
            {
                var valueType = value.GetType();
                var flag2 = !requiredType.IsAssignableFrom(valueType);
                if (flag2)
                {
                    var msg = Utility.GetGeneralErrorMessage("VariableValueNotAssignableToType", valueType.Name, requiredType.Name);
                    throw new ArgumentException(msg);
                }
            }
        }

        internal static MethodInfo GetVariableLoadMethod(Type variableType)
        {
            var mi = typeof(VariableCollection).GetMethod("GetVariableValueInternal", BindingFlags.Instance | BindingFlags.Public);
            return mi.MakeGenericMethod(variableType);
        }

        internal static MethodInfo GetFunctionInvokeMethod(Type returnType)
        {
            var mi = typeof(VariableCollection).GetMethod("GetFunctionResultInternal", BindingFlags.Instance | BindingFlags.Public);
            return mi.MakeGenericMethod(returnType);
        }

        internal static MethodInfo GetVirtualPropertyLoadMethod(Type returnType)
        {
            var mi = typeof(VariableCollection).GetMethod("GetVirtualPropertyValueInternal",
                BindingFlags.Instance | BindingFlags.Public);
            return mi.MakeGenericMethod(returnType);
        }

        private Dictionary<string, object> GetNameValueDictionary()
        {
            var dict = new Dictionary<string, object>();
            this.myVariables.Each(d => dict.Add(d.Key, RuntimeHelpers.GetObjectValue(d.Value.ValueAsObject)));

            //try
            //{
            //    var enumerator = this.myVariables.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var pair = enumerator.Current;
            //        dict.Add(pair.Key, RuntimeHelpers.GetObjectValue(pair.Value.ValueAsObject));
            //    }
            //}
            //finally
            //{
            //    IEnumerator<KeyValuePair<string, IVariable>> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
            return dict;
        }

        public Type GetVariableType(string name)
        {
            var v = this.GetVariable(name, true);
            return v.VariableType;
        }

        public void DefineVariable(string name, Type variableType)
        {
            this.DefineVariableInternal(name, variableType, null);
        }

        public T GetVariableValueInternal<T>(string name)
        {
            var arg140 = this.myVariables;
            IVariable variable;
            var flag = arg140.TryGetValue(name, out variable);
            var v = (IGenericVariable<T>) variable;
            var flag2 = flag;
            T getVariableValueInternal;
            if (flag2)
            {
                getVariableValueInternal = v.GetValue();
            }
            else
            {
                var vTemp = new GenericVariable<T>();
                var args = new ResolveVariableValueEventArgs(name, typeof(T));
                var resolveVariableValueEvent1 = this.resolveVariableValueEvent;
                resolveVariableValueEvent1?.Invoke(this, args);
                ValidateSetValueType(typeof(T), RuntimeHelpers.GetObjectValue(args.VariableValue));
                vTemp.ValueAsObject = RuntimeHelpers.GetObjectValue(args.VariableValue);
                v = vTemp;
                getVariableValueInternal = v.GetValue();
            }
            return getVariableValueInternal;
        }

        public T GetVirtualPropertyValueInternal<T>(string name, object component)
        {
            var coll = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(component));
            var pd = coll.Find(name, true);
            var value = RuntimeHelpers.GetObjectValue(pd.GetValue(RuntimeHelpers.GetObjectValue(component)));
            ValidateSetValueType(typeof(T), RuntimeHelpers.GetObjectValue(value));
            return ReturnGenericValue<T>(RuntimeHelpers.GetObjectValue(value));
        }

        public T GetFunctionResultInternal<T>(string name, object[] arguments)
        {
            var args = new InvokeFunctionEventArgs(name, arguments);
            var invokeFunctionEvent1 = this.invokeFunctionEvent;
            invokeFunctionEvent1?.Invoke(this, args);
            var result = RuntimeHelpers.GetObjectValue(args.Result);
            ValidateSetValueType(typeof(T), RuntimeHelpers.GetObjectValue(result));
            return ReturnGenericValue<T>(RuntimeHelpers.GetObjectValue(result));
        }
    }
}