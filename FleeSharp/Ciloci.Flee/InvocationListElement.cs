using System;
using System.Collections;

namespace Ciloci.Flee
{
	internal class InvocationListElement : ExpressionElement
	{
		private MemberElement MyTail;

		public override Type ResultType
		{
			get
			{
				return this.MyTail.ResultType;
			}
		}

		public InvocationListElement(IList elements, IServiceProvider services)
		{
			this.HandleFirstElement(elements, services);
			InvocationListElement.LinkElements(elements);
			InvocationListElement.Resolve(elements, services);
			this.MyTail = (MemberElement)elements[elements.Count - 1];
		}

		private static void LinkElements(IList elements)
		{
			int num = elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				MemberElement current = (MemberElement)elements[i];
				MemberElement nextElement = null;
				bool flag = i + 1 < elements.Count;
				if (flag)
				{
					nextElement = (MemberElement)elements[i + 1];
				}
				current.Link(nextElement);
			}
		}

		private void HandleFirstElement(IList elements, IServiceProvider services)
		{
			ExpressionElement first = (ExpressionElement)elements[0];
			bool flag = !(first is MemberElement);
			if (flag)
			{
				ExpressionMemberElement actualFirst = new ExpressionMemberElement(first);
				elements[0] = actualFirst;
			}
			else
			{
				this.ResolveNamespaces(elements, services);
			}
		}

		private void ResolveNamespaces(IList elements, IServiceProvider services)
		{
			ExpressionContext context = (ExpressionContext)services.GetService(typeof(ExpressionContext));
			ImportBase currentImport = context.Imports.RootImport;
			while (true)
			{
				string name = InvocationListElement.GetName(elements);
				bool flag = name == null;
				if (flag)
				{
					break;
				}
				ImportBase import = currentImport.FindImport(name);
				bool flag2 = import == null;
				if (flag2)
				{
					break;
				}
				currentImport = import;
				elements.RemoveAt(0);
				bool flag3 = elements.Count > 0;
				if (flag3)
				{
					MemberElement newFirst = (MemberElement)elements[0];
					newFirst.SetImport(currentImport);
				}
			}
			bool flag4 = elements.Count == 0;
			if (flag4)
			{
				base.ThrowCompileException("NamespaceCannotBeUsedAsType", CompileExceptionReason.TypeMismatch, new object[]
				{
					currentImport.Name
				});
			}
		}

		private static string GetName(IList elements)
		{
			bool flag = elements.Count == 0;
			string GetName;
			if (flag)
			{
				GetName = null;
			}
			else
			{
				IdentifierElement fpe = elements[0] as IdentifierElement;
				bool flag2 = fpe == null;
				if (flag2)
				{
					GetName = null;
				}
				else
				{
					GetName = fpe.MemberName;
				}
			}
			return GetName;
		}

		private static void Resolve(IList elements, IServiceProvider services)
		{
			try
			{
				IEnumerator enumerator = elements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MemberElement element = (MemberElement)enumerator.Current;
					element.Resolve(services);
				}
			}
			finally
			{
				IEnumerator enumerator;
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
		}

		public override void Emit(FleeILGenerator ilg, IServiceProvider services)
		{
			this.MyTail.Emit(ilg, services);
		}
	}
}
