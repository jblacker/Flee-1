using System;

namespace Ciloci.Flee
{
	internal interface IVariable
	{
		Type VariableType
		{
			get;
		}

		object ValueAsObject
		{
			get;
			set;
		}

		IVariable Clone();
	}
}
