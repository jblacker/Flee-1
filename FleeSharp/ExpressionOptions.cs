using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Flee
{
    public sealed class ExpressionOptions
    {
        private PropertyDictionary MyProperties;

        private Type MyOwnerType;

        private ExpressionContext MyOwner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private EventHandler CaseSensitiveChangedEvent;

        internal event EventHandler CaseSensitiveChanged
        {
            [CompilerGenerated]
            add
            {
                EventHandler eventHandler = this.CaseSensitiveChangedEvent;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.CaseSensitiveChangedEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                EventHandler eventHandler = this.CaseSensitiveChangedEvent;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.CaseSensitiveChangedEvent, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }

        public Type ResultType
        {
            get
            {
                return this.MyProperties.GetValue<Type>("ResultType");
            }
            set
            {
                Utility.AssertNotNull(value, "value");
                this.MyProperties.SetValue("ResultType", value);
            }
        }

        public bool Checked
        {
            get
            {
                return this.MyProperties.GetValue<bool>("Checked");
            }
            set
            {
                this.MyProperties.SetValue("Checked", value);
            }
        }

        public StringComparison StringComparison
        {
            get
            {
                return this.MyProperties.GetValue<StringComparison>("StringComparison");
            }
            set
            {
                this.MyProperties.SetValue("StringComparison", value);
            }
        }

        public bool EmitToAssembly
        {
            get
            {
                return this.MyProperties.GetValue<bool>("EmitToAssembly");
            }
            set
            {
                this.MyProperties.SetValue("EmitToAssembly", value);
            }
        }

        public BindingFlags OwnerMemberAccess
        {
            get
            {
                return this.MyProperties.GetValue<BindingFlags>("OwnerMemberAccess");
            }
            set
            {
                this.MyProperties.SetValue("OwnerMemberAccess", value);
            }
        }

        public bool CaseSensitive
        {
            get
            {
                return this.MyProperties.GetValue<bool>("CaseSensitive");
            }
            set
            {
                bool flag = this.CaseSensitive != value;
                if (flag)
                {
                    this.MyProperties.SetValue("CaseSensitive", value);
                    EventHandler caseSensitiveChangedEvent = this.CaseSensitiveChangedEvent;
                    if (caseSensitiveChangedEvent != null)
                    {
                        caseSensitiveChangedEvent(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool IntegersAsDoubles
        {
            get
            {
                return this.MyProperties.GetValue<bool>("IntegersAsDoubles");
            }
            set
            {
                this.MyProperties.SetValue("IntegersAsDoubles", value);
            }
        }

        public CultureInfo ParseCulture
        {
            get
            {
                return this.MyProperties.GetValue<CultureInfo>("ParseCulture");
            }
            set
            {
                Utility.AssertNotNull(value, "ParseCulture");
                bool flag = value.LCID != this.ParseCulture.LCID;
                if (flag)
                {
                    this.MyProperties.SetValue("ParseCulture", value);
                    this.SetParseCulture(value);
                    this.MyOwner.ParserOptions.RecreateParser();
                }
            }
        }

        public RealLiteralDataType RealLiteralDataType
        {
            get
            {
                return this.MyProperties.GetValue<RealLiteralDataType>("RealLiteralDataType");
            }
            set
            {
                this.MyProperties.SetValue("RealLiteralDataType", value);
            }
        }

        internal IEqualityComparer<string> StringComparer
        {
            get
            {
                bool caseSensitive = this.CaseSensitive;
                IEqualityComparer<string> StringComparer;
                if (caseSensitive)
                {
                    StringComparer = System.StringComparer.Ordinal;
                }
                else
                {
                    StringComparer = System.StringComparer.OrdinalIgnoreCase;
                }
                return StringComparer;
            }
        }

        internal MemberFilter MemberFilter
        {
            get
            {
                bool caseSensitive = this.CaseSensitive;
                MemberFilter MemberFilter;
                if (caseSensitive)
                {
                    MemberFilter = Type.FilterName;
                }
                else
                {
                    MemberFilter = Type.FilterNameIgnoreCase;
                }
                return MemberFilter;
            }
        }

        internal StringComparison MemberStringComparison
        {
            get
            {
                bool caseSensitive = this.CaseSensitive;
                StringComparison MemberStringComparison;
                if (caseSensitive)
                {
                    MemberStringComparison = StringComparison.Ordinal;
                }
                else
                {
                    MemberStringComparison = StringComparison.OrdinalIgnoreCase;
                }
                return MemberStringComparison;
            }
        }

        internal Type OwnerType
        {
            get
            {
                return this.MyOwnerType;
            }
        }

        internal bool IsGeneric
        {
            get
            {
                return this.MyProperties.GetValue<bool>("IsGeneric");
            }
            set
            {
                this.MyProperties.SetValue("IsGeneric", value);
            }
        }

        internal ExpressionOptions(ExpressionContext owner)
        {
            this.MyOwner = owner;
            this.MyProperties = new PropertyDictionary();
            this.InitializeProperties();
        }

        private void InitializeProperties()
        {
            this.StringComparison = StringComparison.Ordinal;
            this.OwnerMemberAccess = BindingFlags.Public;
            this.MyProperties.SetToDefault<bool>("CaseSensitive");
            this.MyProperties.SetToDefault<bool>("Checked");
            this.MyProperties.SetToDefault<bool>("EmitToAssembly");
            this.MyProperties.SetToDefault<Type>("ResultType");
            this.MyProperties.SetToDefault<bool>("IsGeneric");
            this.MyProperties.SetToDefault<bool>("IntegersAsDoubles");
            this.MyProperties.SetValue("ParseCulture", CultureInfo.CurrentCulture);
            this.SetParseCulture(this.ParseCulture);
            this.MyProperties.SetValue("RealLiteralDataType", RealLiteralDataType.Double);
        }

        private void SetParseCulture(CultureInfo ci)
        {
            ExpressionParserOptions po = this.MyOwner.ParserOptions;
            po.DecimalSeparator = Conversions.ToChar(ci.NumberFormat.NumberDecimalSeparator);
            po.FunctionArgumentSeparator = Conversions.ToChar(ci.TextInfo.ListSeparator);
            po.DateTimeFormat = ci.DateTimeFormat.ShortDatePattern;
        }

        internal ExpressionOptions Clone()
        {
            ExpressionOptions clonedOptions = (ExpressionOptions)this.MemberwiseClone();
            clonedOptions.MyProperties = this.MyProperties.Clone();
            return clonedOptions;
        }

        internal bool IsOwnerType(Type t)
        {
            return this.MyOwnerType.IsAssignableFrom(t);
        }

        internal void SetOwnerType(Type ownerType)
        {
            this.MyOwnerType = ownerType;
        }
    }
}