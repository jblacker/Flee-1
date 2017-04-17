namespace Flee.PerCederberg.Grammatica.Runtime.RE
{
    using System.IO;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Matcher
	{
		private readonly Element element;

		private LookAheadReader input;

		private readonly bool ignoreCase;

		private int start;

		private int length;

		private bool endOfString;

		internal Matcher(Element e, LookAheadReader input, bool ignoreCase)
		{
			this.element = e;
			this.input = input;
			this.ignoreCase = ignoreCase;
			this.Reset();
		}

		public bool IsCaseInsensitive()
		{
			return this.ignoreCase;
		}

		public void Reset()
		{
			this.length = -1;
			this.endOfString = false;
		}

		public void Reset(string str)
		{
			this.Reset(new StringReader(str));
		}

		public void Reset(TextReader input)
		{
			bool flag = input is LookAheadReader;
			if (flag)
			{
				this.Reset((LookAheadReader)input);
			}
			else
			{
				this.Reset(new LookAheadReader(input));
			}
		}

		private void Reset(LookAheadReader input)
		{
			this.input = input;
			this.Reset();
		}

		public int Start()
		{
			return this.start;
		}

		public int End()
		{
			bool flag = this.length > 0;
			int end;
			if (flag)
			{
				end = this.start + this.length;
			}
			else
			{
				end = this.start;
			}
			return end;
		}

		public int Length()
		{
			return this.length;
		}

		public bool HasReadEndOfString()
		{
			return this.endOfString;
		}

		public bool MatchFromBeginning()
		{
			return this.MatchFrom(0);
		}

		public bool MatchFrom(int pos)
		{
			this.Reset();
			this.start = pos;
			this.length = this.element.Match(this, this.input, this.start, 0);
			return this.length >= 0;
		}

		public override string ToString()
		{
			bool flag = this.length <= 0;
			string toString;
			if (flag)
			{
				toString = "";
			}
			else
			{
				try
				{
					toString = this.input.PeekString(this.start, this.length);
				}
				catch (IOException exception)
				{
					ProjectData.SetProjectError(exception);
					toString = "";
					ProjectData.ClearProjectError();
				}
			}
			return toString;
		}

		internal void SetReadEndOfString()
		{
			this.endOfString = true;
		}
	}
}
