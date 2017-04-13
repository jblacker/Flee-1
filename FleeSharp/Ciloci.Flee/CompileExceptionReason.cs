using System;

namespace Ciloci.Flee
{
	public enum CompileExceptionReason
	{
		SyntaxError,
		ConstantOverflow,
		TypeMismatch,
		UndefinedName,
		FunctionHasNoReturnValue,
		InvalidExplicitCast,
		AmbiguousMatch,
		AccessDenied,
		InvalidFormat
	}
}
