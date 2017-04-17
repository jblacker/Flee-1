using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Flee
{
    internal class IdentifierElement : MemberElement
    {
        private FieldInfo myField;

        private PropertyInfo myProperty;

        private PropertyDescriptor myPropertyDescriptor;

        private Type myVariableType;

        private Type myCalcEngineReferenceType;

        private Type MemberOwnerType
        {
            get
            {
                var flag = this.myField != null;
                Type memberOwnerType;
                if (flag)
                {
                    memberOwnerType = this.myField.ReflectedType;
                }
                else
                {
                    var flag2 = this.myPropertyDescriptor != null;
                    if (flag2)
                    {
                        memberOwnerType = this.myPropertyDescriptor.ComponentType;
                    }
                    else
                    {
                        var flag3 = this.myProperty != null;
                        memberOwnerType = flag3 ? this.myProperty.ReflectedType : null;
                    }
                }
                return memberOwnerType;
            }
        }

        public override Type ResultType
        {
            get
            {
                var flag = this.myCalcEngineReferenceType != null;
                Type resultType;
                if (flag)
                {
                    resultType = this.myCalcEngineReferenceType;
                }
                else
                {
                    var flag2 = this.myVariableType != null;
                    if (flag2)
                    {
                        resultType = this.myVariableType;
                    }
                    else
                    {
                        var flag3 = this.myPropertyDescriptor != null;
                        if (flag3)
                        {
                            resultType = this.myPropertyDescriptor.PropertyType;
                        }
                        else
                        {
                            var flag4 = this.myField != null;
                            if (flag4)
                            {
                                resultType = this.myField.FieldType;
                            }
                            else
                            {
                                var mi = this.myProperty.GetGetMethod(true);
                                resultType = mi.ReturnType;
                            }
                        }
                    }
                }
                return resultType;
            }
        }

        protected override bool RequiresAddress => this.myPropertyDescriptor == null;

        protected override bool IsPublic
        {
            get
            {
                var flag = this.myVariableType != null | this.myCalcEngineReferenceType != null;
                bool isPublic;
                if (flag)
                {
                    isPublic = true;
                }
                else
                {
                    var flag2 = this.myVariableType != null;
                    if (flag2)
                    {
                        isPublic = true;
                    }
                    else
                    {
                        var flag3 = this.myPropertyDescriptor != null;
                        if (flag3)
                        {
                            isPublic = true;
                        }
                        else
                        {
                            var flag4 = this.myField != null;
                            if (flag4)
                            {
                                isPublic = this.myField.IsPublic;
                            }
                            else
                            {
                                var mi = this.myProperty.GetGetMethod(true);
                                isPublic = mi.IsPublic;
                            }
                        }
                    }
                }
                return isPublic;
            }
        }

        protected override bool SupportsStatic
        {
            get
            {
                var flag = this.myVariableType != null;
                bool supportsStatic;
                if (flag)
                {
                    supportsStatic = false;
                }
                else
                {
                    var flag2 = this.myPropertyDescriptor != null;
                    if (flag2)
                    {
                        supportsStatic = false;
                    }
                    else
                    {
                        var flag3 = this.myOptions.IsOwnerType(this.MemberOwnerType) && this.myPrevious == null;
                        supportsStatic = (flag3 || this.myPrevious == null);
                    }
                }
                return supportsStatic;
            }
        }

        protected override bool SupportsInstance
        {
            get
            {
                var flag = this.myVariableType != null;
                bool supportsInstance;
                if (flag)
                {
                    supportsInstance = true;
                }
                else
                {
                    var flag2 = this.myPropertyDescriptor != null;
                    if (flag2)
                    {
                        supportsInstance = true;
                    }
                    else
                    {
                        var flag3 = this.myOptions.IsOwnerType(this.MemberOwnerType) && this.myPrevious == null;
                        supportsInstance = (flag3 || this.myPrevious != null);
                    }
                }
                return supportsInstance;
            }
        }

        public override bool IsStatic
        {
            get
            {
                var flag = this.myVariableType != null | this.myCalcEngineReferenceType != null;
                bool isStatic;
                if (flag)
                {
                    isStatic = false;
                }
                else
                {
                    var flag2 = this.myVariableType != null;
                    if (flag2)
                    {
                        isStatic = false;
                    }
                    else
                    {
                        var flag3 = this.myField != null;
                        if (flag3)
                        {
                            isStatic = this.myField.IsStatic;
                        }
                        else
                        {
                            var flag4 = this.myPropertyDescriptor != null;
                            if (flag4)
                            {
                                isStatic = false;
                            }
                            else
                            {
                                var mi = this.myProperty.GetGetMethod(true);
                                isStatic = mi.IsStatic;
                            }
                        }
                    }
                }
                return isStatic;
            }
        }

        public IdentifierElement(string name)
        {
            this.myName = name;
        }

        protected override void ResolveInternal()
        {
            var flag = this.ResolveFieldProperty(this.myPrevious);
            if (flag)
            {
                this.AddReferencedVariable(this.myPrevious);
            }
            else
            {
                this.myVariableType = this.myContext.Variables.GetVariableTypeInternal(this.myName);
                var flag2 = this.myPrevious == null && this.myVariableType != null;
                if (flag2)
                {
                    this.AddReferencedVariable(this.myPrevious);
                }
                else
                {
                    var ce = this.myContext.CalculationEngine;
                    var flag3 = ce != null;
                    if (flag3)
                    {
                        ce.AddDependency(this.myName, this.myContext);
                        this.myCalcEngineReferenceType = ce.ResolveTailType(this.myName);
                    }
                    else
                    {
                        var flag4 = this.myPrevious == null;
                        if (flag4)
                        {
                            this.ThrowCompileException("NoIdentifierWithName", CompileExceptionReason.UndefinedName, this.myName);
                        }
                        else
                        {
                            this.ThrowCompileException("NoIdentifierWithNameOnType", CompileExceptionReason.UndefinedName, this.myName, this.myPrevious.TargetType.Name);
                        }
                    }
                }
            }
        }

        private bool ResolveFieldProperty(MemberElement previous)
        {
            var members = this.GetMembers(MemberTypes.Field | MemberTypes.Property);
            members = this.GetAccessibleMembers(members);
            var flag = members.Length == 0;
            bool resolveFieldProperty = false;
            if (flag)
            {
                resolveFieldProperty = this.ResolveVirtualProperty(previous);
            }
            else
            {
                var flag2 = members.Length > 1;
                if (flag2)
                {
                    var flag3 = previous == null;
                    if (flag3)
                    {
                        this.ThrowCompileException("IdentifierIsAmbiguous", CompileExceptionReason.AmbiguousMatch, this.myName);
                    }
                    else
                    {
                        this.ThrowCompileException("IdentifierIsAmbiguousOnType", CompileExceptionReason.AmbiguousMatch, this.myName, previous.TargetType.Name);
                    }
                }
                else
                {
                    this.myField = (members[0] as FieldInfo);
                    var flag4 = this.myField != null;
                    if (flag4)
                    {
                        resolveFieldProperty = true;
                    }
                    else
                    {
                        this.myProperty = (PropertyInfo)members[0];
                        resolveFieldProperty = true;
                    }
                }
            }
            return resolveFieldProperty;
        }

        private bool ResolveVirtualProperty(MemberElement previous)
        {
            var flag = previous == null;
            bool resolveVirtualProperty;
            if (flag)
            {
                resolveVirtualProperty = false;
            }
            else
            {
                var coll = TypeDescriptor.GetProperties(previous.ResultType);
                this.myPropertyDescriptor = coll.Find(this.myName, true);
                resolveVirtualProperty = (this.myPropertyDescriptor != null);
            }
            return resolveVirtualProperty;
        }

        private void AddReferencedVariable(MemberElement previous)
        {
            var flag = previous != null;
            if (!flag)
            {
                var flag2 = this.myVariableType != null || this.myOptions.IsOwnerType(this.MemberOwnerType);
                if (flag2)
                {
                    var info = (ExpressionInfo)this.myServices.GetService(typeof(ExpressionInfo));
                    info.AddReferencedVariable(this.myName);
                }
            }
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            base.Emit(ilg, services);
            this.EmitFirst(ilg);
            var flag = this.myCalcEngineReferenceType != null;
            if (flag)
            {
                this.EmitReferenceLoad(ilg);
            }
            else
            {
                var flag2 = this.myVariableType != null;
                if (flag2)
                {
                    this.EmitVariableLoad(ilg);
                }
                else
                {
                    var flag3 = this.myField != null;
                    if (flag3)
                    {
                        this.EmitFieldLoad(this.myField, ilg, services);
                    }
                    else
                    {
                        var flag4 = this.myPropertyDescriptor != null;
                        if (flag4)
                        {
                            this.EmitVirtualPropertyLoad(ilg);
                        }
                        else
                        {
                            this.EmitPropertyLoad(this.myProperty, ilg);
                        }
                    }
                }
            }
        }

        private void EmitReferenceLoad(FleeIlGenerator ilg)
        {
            ilg.Emit(OpCodes.Ldarg_1);
            this.myContext.CalculationEngine.EmitLoad(this.myName, ilg);
        }

        private void EmitFirst(FleeIlGenerator ilg)
        {
            var flag = this.myPrevious != null;
            if (!flag)
            {
                var isVariable = this.myVariableType != null;
                var flag2 = isVariable;
                if (flag2)
                {
                    EmitLoadVariables(ilg);
                }
                else
                {
                    var flag3 = this.myOptions.IsOwnerType(this.MemberOwnerType) & !this.IsStatic;
                    if (flag3)
                    {
                        this.EmitLoadOwner(ilg);
                    }
                }
            }
        }

        private void EmitVariableLoad(FleeIlGenerator ilg)
        {
            var mi = VariableCollection.GetVariableLoadMethod(this.myVariableType);
            ilg.Emit(OpCodes.Ldstr, this.myName);
            this.EmitMethodCall(mi, ilg);
        }

        private void EmitFieldLoad(FieldInfo fi, FleeIlGenerator ilg, IServiceProvider services)
        {
            var isLiteral = fi.IsLiteral;
            if (isLiteral)
            {
                EmitLiteral(fi, ilg, services);
            }
            else
            {
                var flag = this.ResultType.IsValueType & this.NextRequiresAddress;
                EmitLdfld(fi, flag, ilg);
            }
        }

        private static void EmitLdfld(FieldInfo fi, bool indirect, FleeIlGenerator ilg)
        {
            var isStatic = fi.IsStatic;
            if (isStatic)
            {
                ilg.Emit(indirect ? OpCodes.Ldsflda : OpCodes.Ldsfld, fi);
            }
            else if (indirect)
            {
                ilg.Emit(OpCodes.Ldflda, fi);
            }
            else
            {
                ilg.Emit(OpCodes.Ldfld, fi);
            }
        }

        private static void EmitLiteral(FieldInfo fi, FleeIlGenerator ilg, IServiceProvider services)
        {
            var value = RuntimeHelpers.GetObjectValue(fi.GetValue(null));
            var t = value.GetType();
            LiteralElement elem;
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                    elem = new BooleanLiteralElement((bool)value);
                    goto IL_F4;
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                    elem = new Int32LiteralElement(Convert.ToInt32(RuntimeHelpers.GetObjectValue(value)));
                    goto IL_F4;
                case TypeCode.UInt32:
                    elem = new UInt32LiteralElement((uint)value);
                    goto IL_F4;
                case TypeCode.Int64:
                    elem = new Int64LiteralElement((long)value);
                    goto IL_F4;
                case TypeCode.UInt64:
                    elem = new UInt64LiteralElement((ulong)value);
                    goto IL_F4;
                case TypeCode.Single:
                    elem = new SingleLiteralElement((float)value);
                    goto IL_F4;
                case TypeCode.Double:
                    elem = new DoubleLiteralElement((double)value);
                    goto IL_F4;
                case TypeCode.String:
                    elem = new StringLiteralElement((string)value);
                    goto IL_F4;
            }
            elem = null;
            Debug.Fail("Unsupported constant type");
            IL_F4:
            elem.Emit(ilg, services);
        }

        private void EmitPropertyLoad(PropertyInfo pi, FleeIlGenerator ilg)
        {
            var getter = pi.GetGetMethod(true);
            this.EmitMethodCall(getter, ilg);
        }

        private void EmitVirtualPropertyLoad(FleeIlGenerator ilg)
        {
            var index = ilg.GetTempLocalIndex(this.myPrevious.ResultType);
            Utility.EmitStoreLocal(ilg, index);
            EmitLoadVariables(ilg);
            ilg.Emit(OpCodes.Ldstr, this.myName);
            Utility.EmitLoadLocal(ilg, index);
            ImplicitConverter.EmitImplicitConvert(this.myPrevious.ResultType, typeof(object), ilg);
            var mi = VariableCollection.GetVirtualPropertyLoadMethod(this.ResultType);
            this.EmitMethodCall(mi, ilg);
        }
    }
}