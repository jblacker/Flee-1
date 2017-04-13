using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime.RE
{
	internal class Matcher
	{
		private Element element;

		private LookAheadReader input;

		private bool ignoreCase;

		private int m_start;

		private int m_length;

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
			this.m_length = -1;
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
			return this.m_start;
		}

		public int End()
		{
			bool flag = this.m_length > 0;
			int End;
			if (flag)
			{
				End = this.m_start + this.m_length;
			}
			else
			{
				End = this.m_start;
			}
			return End;
		}

		public int Length()
		{
			return this.m_length;
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
			this.m_start = pos;
			this.m_length = this.element.Match(this, this.input, this.m_start, 0);
			return this.m_length >= 0;
		}

		public override string ToString()
		{
			bool flag = this.m_length <= 0;
			string ToString;
			if (flag)
			{
				ToString = "";
			}
			else
			{
				try
				{
					ToString = this.input.PeekString(this.m_start, this.m_length);
				}
				catch (IOException expr_35)
				{
					ProjectData.SetProjectError(expr_35);
					ToString = "";
					ProjectData.ClearProjectError();
				}
			}
			return ToString;
		}

		internal void SetReadEndOfString()
		{
			this.endOfString = true;
		}
	}
}
