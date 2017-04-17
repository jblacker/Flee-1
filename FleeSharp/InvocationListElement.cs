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
    using System.Collections.Generic;

    internal class InvocationListElement : ExpressionElement
    {
        private readonly MemberElement myTail;

        public InvocationListElement(IList<object> elements, IServiceProvider services)
        {
            this.HandleFirstElement(elements, services);
            LinkElements(elements);
            Resolve(elements, services);
            this.myTail = (MemberElement) elements[elements.Count - 1];
        }

        private static void LinkElements(IList<object> elements)
        {
            var num = elements.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var current = (MemberElement) elements[i];
                MemberElement nextElement = null;
                var flag = i + 1 < elements.Count;
                if (flag)
                {
                    nextElement = (MemberElement) elements[i + 1];
                }
                current.Link(nextElement);
            }
        }

        private void HandleFirstElement(IList<object> elements, IServiceProvider services)
        {
            var first = (ExpressionElement) elements[0];
            var flag = !(first is MemberElement);
            if (flag)
            {
                var actualFirst = new ExpressionMemberElement(first);
                elements[0] = actualFirst;
            }
            else
            {
                this.ResolveNamespaces(elements, services);
            }
        }

        private void ResolveNamespaces(IList<object> elements, IServiceProvider services)
        {
            var context = (ExpressionContext) services.GetService(typeof(ExpressionContext));
            ImportBase currentImport = context.Imports.RootImport;
            while (true)
            {
                var name = GetName(elements);
                var flag = name == null;
                if (flag)
                {
                    break;
                }
                var import = currentImport.FindImport(name);
                var flag2 = import == null;
                if (flag2)
                {
                    break;
                }
                currentImport = import;
                elements.RemoveAt(0);
                var flag3 = elements.Count > 0;
                if (flag3)
                {
                    var newFirst = (MemberElement) elements[0];
                    newFirst.SetImport(currentImport);
                }
            }
            var flag4 = elements.Count == 0;
            if (flag4)
            {
                this.ThrowCompileException("NamespaceCannotBeUsedAsType", CompileExceptionReason.TypeMismatch, currentImport.Name);
            }
        }

        private static string GetName(IList<object> elements)
        {
            var flag = elements.Count == 0;
            string getName;
            if (flag)
            {
                getName = null;
            }
            else
            {
                var fpe = elements[0] as IdentifierElement;
                var flag2 = fpe == null;
                getName = flag2 ? null : fpe.MemberName;
            }
            return getName;
        }

        private static void Resolve(IEnumerable<object> elements, IServiceProvider services)
        {
            foreach (var element in elements)
            {
                var member = (MemberElement) element;
                member.Resolve(services);
            }

            //try
            //{
            //    IEnumerator enumerator = elements.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        MemberElement element = (MemberElement)enumerator.Current;
            //        element.Resolve(services);
            //    }
            //}
            //finally
            //{
            //    IEnumerator enumerator;
            //    if (enumerator is IDisposable)
            //    {
            //        (enumerator as IDisposable).Dispose();
            //    }
            //}
        }

        public override void Emit(FleeIlGenerator ilg, IServiceProvider services)
        {
            this.myTail.Emit(ilg, services);
        }

        public override Type ResultType => this.myTail.ResultType;
    }
}