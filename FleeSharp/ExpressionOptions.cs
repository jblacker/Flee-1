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

namespace Flee
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.VisualBasic.CompilerServices;

    public sealed class ExpressionOptions
    {
        private readonly ExpressionContext myOwner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [CompilerGenerated]
        private EventHandler caseSensitiveChangedEvent;

        private PropertyDictionary myProperties;

        internal ExpressionOptions(ExpressionContext owner)
        {
            this.myOwner = owner;
            this.myProperties = new PropertyDictionary();
            this.InitializeProperties();
        }

        internal event EventHandler CaseSensitiveChanged
        {
            [CompilerGenerated]
            add
            {
                var eventHandler = this.caseSensitiveChangedEvent;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler) Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.caseSensitiveChangedEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                var eventHandler = this.caseSensitiveChangedEvent;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler) Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref this.caseSensitiveChangedEvent, value2, eventHandler2);
                } while (eventHandler != eventHandler2);
            }
        }

        private void InitializeProperties()
        {
            this.StringComparison = StringComparison.Ordinal;
            this.OwnerMemberAccess = BindingFlags.Public;
            this.myProperties.SetToDefault<bool>("CaseSensitive");
            this.myProperties.SetToDefault<bool>("Checked");
            this.myProperties.SetToDefault<bool>("EmitToAssembly");
            this.myProperties.SetToDefault<Type>("ResultType");
            this.myProperties.SetToDefault<bool>("IsGeneric");
            this.myProperties.SetToDefault<bool>("IntegersAsDoubles");
            this.myProperties.SetValue("ParseCulture", CultureInfo.CurrentCulture);
            this.SetParseCulture(this.ParseCulture);
            this.myProperties.SetValue("RealLiteralDataType", RealLiteralDataType.Double);
        }

        private void SetParseCulture(CultureInfo ci)
        {
            var po = this.myOwner.ParserOptions;
            po.DecimalSeparator = Conversions.ToChar(ci.NumberFormat.NumberDecimalSeparator);
            po.FunctionArgumentSeparator = Conversions.ToChar(ci.TextInfo.ListSeparator);
            po.DateTimeFormat = ci.DateTimeFormat.ShortDatePattern;
        }

        internal ExpressionOptions Clone()
        {
            var clonedOptions = (ExpressionOptions) this.MemberwiseClone();
            clonedOptions.myProperties = this.myProperties.Clone();
            return clonedOptions;
        }

        internal bool IsOwnerType(Type t)
        {
            return this.OwnerType.IsAssignableFrom(t);
        }

        internal void SetOwnerType(Type ownerType)
        {
            this.OwnerType = ownerType;
        }

        public bool CaseSensitive
        {
            get { return this.myProperties.GetValue<bool>("CaseSensitive"); }
            set
            {
                var flag = this.CaseSensitive != value;
                if (flag)
                {
                    this.myProperties.SetValue("CaseSensitive", value);
                    var caseSensitiveCe = this.caseSensitiveChangedEvent;
                    caseSensitiveCe?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool Checked
        {
            get { return this.myProperties.GetValue<bool>("Checked"); }
            set { this.myProperties.SetValue("Checked", value); }
        }

        public bool EmitToAssembly
        {
            get { return this.myProperties.GetValue<bool>("EmitToAssembly"); }
            set { this.myProperties.SetValue("EmitToAssembly", value); }
        }

        public bool IntegersAsDoubles
        {
            get { return this.myProperties.GetValue<bool>("IntegersAsDoubles"); }
            set { this.myProperties.SetValue("IntegersAsDoubles", value); }
        }

        internal bool IsGeneric
        {
            get { return this.myProperties.GetValue<bool>("IsGeneric"); }
            set { this.myProperties.SetValue("IsGeneric", value); }
        }

        internal MemberFilter MemberFilter
        {
            get
            {
                var caseSensitive = this.CaseSensitive;
                var memberFilter = caseSensitive ? Type.FilterName : Type.FilterNameIgnoreCase;
                return memberFilter;
            }
        }

        internal StringComparison MemberStringComparison
        {
            get
            {
                var caseSensitive = this.CaseSensitive;
                var memberStringComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return memberStringComparison;
            }
        }

        public BindingFlags OwnerMemberAccess
        {
            get { return this.myProperties.GetValue<BindingFlags>("OwnerMemberAccess"); }
            set { this.myProperties.SetValue("OwnerMemberAccess", value); }
        }

        internal Type OwnerType { get; private set; }

        public CultureInfo ParseCulture
        {
            get { return this.myProperties.GetValue<CultureInfo>("ParseCulture"); }
            set
            {
                Utility.AssertNotNull(value, "ParseCulture");
                var flag = value.LCID != this.ParseCulture.LCID;
                if (flag)
                {
                    this.myProperties.SetValue("ParseCulture", value);
                    this.SetParseCulture(value);
                    this.myOwner.ParserOptions.RecreateParser();
                }
            }
        }

        public RealLiteralDataType RealLiteralDataType
        {
            get { return this.myProperties.GetValue<RealLiteralDataType>("RealLiteralDataType"); }
            set { this.myProperties.SetValue("RealLiteralDataType", value); }
        }

        public Type ResultType
        {
            get { return this.myProperties.GetValue<Type>("ResultType"); }
            set
            {
                Utility.AssertNotNull(value, "value");
                this.myProperties.SetValue("ResultType", value);
            }
        }

        internal IEqualityComparer<string> StringComparer
        {
            get
            {
                var caseSensitive = this.CaseSensitive;
                IEqualityComparer<string> stringComparer = caseSensitive
                    ? System.StringComparer.Ordinal : System.StringComparer.OrdinalIgnoreCase;
                return stringComparer;
            }
        }

        public StringComparison StringComparison
        {
            get { return this.myProperties.GetValue<StringComparison>("StringComparison"); }
            set { this.myProperties.SetValue("StringComparison", value); }
        }
    }
}