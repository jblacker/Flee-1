using System;
using System.Globalization;
using System.Reflection;

namespace Flee
{
    internal class ExplicitOperatorMethodBinder : CustomBinder
    {
        private readonly Type myReturnType;

        private readonly Type myArgType;

        public ExplicitOperatorMethodBinder(Type returnType, Type argType)
        {
            this.myReturnType = returnType;
            this.myArgType = argType;
        }

        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            checked
            {
                MethodBase selectMethod;
                for (int i = 0; i < match.Length; i++)
                {
                    MethodInfo mi = (MethodInfo)match[i];
                    ParameterInfo[] parameters = mi.GetParameters();
                    ParameterInfo firstParameter = parameters[0];
                    bool flag = firstParameter.ParameterType == this.myArgType & mi.ReturnType == this.myReturnType;
                    if (flag)
                    {
                        selectMethod = mi;
                        return selectMethod;
                    }
                }
                return null;
            }
        }

        /// <summary>Selects a method to invoke from the given set of methods, based on the supplied arguments.</summary>
        /// <returns>The matching method.</returns>
        /// <param name="bindingAttr">A bitwise combination of <see cref="T:System.Reflection.BindingFlags" /> values. </param>
        /// <param name="match">The set of methods that are candidates for matching. For example, when a <see cref="T:System.Reflection.Binder" /> object is used by <see cref="Overload:System.Type.InvokeMember" />, this parameter specifies the set of methods that reflection has determined to be possible matches, typically because they have the correct member name. The default implementation provided by <see cref="P:System.Type.DefaultBinder" /> changes the order of this array.</param>
        /// <param name="args">The arguments that are passed in. The binder can change the order of the arguments in this array; for example, the default binder changes the order of arguments if the <paramref name="names" /> parameter is used to specify an order other than positional order. If a binder implementation coerces argument types, the types and values of the arguments can be changed as well. </param>
        /// <param name="modifiers">An array of parameter modifiers that enable binding to work with parameter signatures in which the types have been modified. The default binder implementation does not use this parameter.</param>
        /// <param name="culture">An instance of <see cref="T:System.Globalization.CultureInfo" /> that is used to control the coercion of data types, in binder implementations that coerce types. If <paramref name="culture" /> is null, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used. Note   For example, if a binder implementation allows coercion of string values to numeric types, this parameter is necessary to convert a String that represents 1000 to a Double value, because 1000 is represented differently by different cultures. The default binder does not do such string coercions.</param>
        /// <param name="names">The parameter names, if parameter names are to be considered when matching, or null if arguments are to be treated as purely positional. For example, parameter names must be used if arguments are not supplied in positional order. </param>
        /// <param name="state">After the method returns, <paramref name="state" /> contains a binder-provided object that keeps track of argument reordering. The binder creates this object, and the binder is the sole consumer of this object. If <paramref name="state" /> is not null when BindToMethod returns, you must pass <paramref name="state" /> to the <see cref="M:System.Reflection.Binder.ReorderArgumentArray(System.Object[]@,System.Object)" /> method if you want to restore <paramref name="args" /> to its original order, for example, so that you can retrieve the values of ref parameters (ByRef parameters in Visual Basic). </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">For the default binder, <paramref name="match" /> contains multiple methods that are equally good matches for <paramref name="args" />. For example, <paramref name="args" /> contains a MyClass object that implements the IMyClass interface, and <paramref name="match" /> contains a method that takes MyClass and a method that takes IMyClass. </exception>
        /// <exception cref="T:System.MissingMethodException">For the default binder, <paramref name="match" /> contains no methods that can accept the arguments supplied in <paramref name="args" />.</exception>
        /// <exception cref="T:System.ArgumentException">For the default binder, <paramref name="match" /> is null or an empty array.</exception>
        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers,
            CultureInfo culture, string[] names, out object state)
        {
            throw new NotImplementedException();
        }
    }
}