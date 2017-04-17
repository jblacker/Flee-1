namespace Flee.PerCederberg.Grammatica.Runtime.RE
{
    using System;
    using System.IO;

    internal abstract class Element : ICloneable
	{
		public abstract object Clone();

		public abstract int Match(Matcher m, LookAheadReader input, int start, int skip);

		public abstract void PrintTo(TextWriter output, string indent);
	}
}
