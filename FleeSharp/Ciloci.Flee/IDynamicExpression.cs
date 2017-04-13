using System;

namespace Ciloci.Flee
{
	public interface IDynamicExpression : IExpression
	{
		object Evaluate();
	}
}
