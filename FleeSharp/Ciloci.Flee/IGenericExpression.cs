using System;

namespace Ciloci.Flee
{
	public interface IGenericExpression<T> : IExpression
	{
		T Evaluate();
	}
}
