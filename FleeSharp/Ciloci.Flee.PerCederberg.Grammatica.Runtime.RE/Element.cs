using System;
using System.IO;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal abstract class Element : ICloneable
	{
		public abstract object Clone();

		public abstract int Match(Matcher m, LookAheadReader input, int start, int skip);

		public abstract void PrintTo(TextWriter output, string indent);
	}
}
