using System;

namespace Ciloci.Flee
{
	internal delegate T ExpressionEvaluator<T>(object owner, ExpressionContext context, VariableCollection variables);
}
