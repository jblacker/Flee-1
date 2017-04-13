using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Threading;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class LookAheadReader : TextReader
	{
		private const int STREAM_BLOCK_SIZE = 4096;

		private const int BUFFER_BLOCK_SIZE = 1024;

		private char[] buffer;

		private int pos;

		private int length;

		private TextReader input;

		private int line;

		private int column;

		public int LineNumber
		{
			get
			{
				return this.line;
			}
		}

		public int ColumnNumber
		{
			get
			{
				return this.column;
			}
		}

		public LookAheadReader(TextReader input)
		{
			this.buffer = new char[4096];
			this.input = null;
			this.line = 1;
			this.column = 1;
			this.input = input;
		}

		public override int Read()
		{
			this.ReadAhead(1);
			bool flag = this.pos >= this.length;
			int Read;
			if (flag)
			{
				Read = -1;
			}
			else
			{
				this.UpdateLineColumnNumbers(1);
				Read = Convert.ToInt32(this.buffer[Math.Max(Interlocked.Increment(ref this.pos), this.pos - 1)]);
			}
			return Read;
		}

		public override int Read(char[] cbuf, int off, int len)
		{
			this.ReadAhead(len);
			bool flag = this.pos >= this.length;
			int Read;
			if (flag)
			{
				Read = -1;
			}
			else
			{
				int count = this.length - this.pos;
				bool flag2 = count > len;
				if (flag2)
				{
					count = len;
				}
				this.UpdateLineColumnNumbers(count);
				Array.Copy(this.buffer, this.pos, cbuf, off, count);
				this.pos += count;
				Read = count;
			}
			return Read;
		}

		public string ReadString(int len)
		{
			this.ReadAhead(len);
			bool flag = this.pos >= this.length;
			string ReadString;
			if (flag)
			{
				ReadString = null;
			}
			else
			{
				int count = this.length - this.pos;
				bool flag2 = count > len;
				if (flag2)
				{
					count = len;
				}
				this.UpdateLineColumnNumbers(count);
				string result = new string(this.buffer, this.pos, count);
				this.pos += count;
				ReadString = result;
			}
			return ReadString;
		}

		public override int Peek()
		{
			return this.Peek(0);
		}

		public int Peek(int off)
		{
			this.ReadAhead(off + 1);
			bool flag = this.pos + off >= this.length;
			int Peek;
			if (flag)
			{
				Peek = -1;
			}
			else
			{
				Peek = Convert.ToInt32(this.buffer[this.pos + off]);
			}
			return Peek;
		}

		public string PeekString(int off, int len)
		{
			this.ReadAhead(off + len + 1);
			bool flag = this.pos + off >= this.length;
			string PeekString;
			if (flag)
			{
				PeekString = null;
			}
			else
			{
				int count = this.length - (this.pos + off);
				bool flag2 = count > len;
				if (flag2)
				{
					count = len;
				}
				PeekString = new string(this.buffer, this.pos + off, count);
			}
			return PeekString;
		}

		public override void Close()
		{
			this.buffer = null;
			this.pos = 0;
			this.length = 0;
			bool flag = this.input != null;
			if (flag)
			{
				this.input.Close();
				this.input = null;
			}
		}

		private void ReadAhead(int offset)
		{
			bool flag = this.input == null || this.pos + offset < this.length;
			if (!flag)
			{
				bool flag2 = this.pos > 1024;
				if (flag2)
				{
					Array.Copy(this.buffer, this.pos, this.buffer, 0, this.length - this.pos);
					this.length -= this.pos;
					this.pos = 0;
				}
				int size = this.pos + offset - this.length + 1;
				bool flag3 = size % 4096 != 0;
				if (flag3)
				{
					size = (int)Math.Round((double)size / 4096.0 * 4096.0);
					size += 4096;
				}
				this.EnsureBufferCapacity(this.length + size);
				int readSize;
				try
				{
					readSize = this.input.Read(this.buffer, this.length, size);
				}
				catch (IOException expr_EE)
				{
					ProjectData.SetProjectError(expr_EE);
					this.input = null;
					throw;
				}
				bool flag4 = readSize > 0;
				if (flag4)
				{
					this.length += readSize;
				}
				bool flag5 = readSize < size;
				if (flag5)
				{
					try
					{
						this.input.Close();
					}
					finally
					{
						this.input = null;
					}
				}
			}
		}

		private void EnsureBufferCapacity(int size)
		{
			bool flag = this.buffer.Length >= size;
			if (!flag)
			{
				bool flag2 = size % 1024 != 0;
				if (flag2)
				{
					size = (int)Math.Round((double)size / 1024.0 * 1024.0);
					size += 1024;
				}
				char[] newbuf = new char[size - 1 + 1];
				Array.Copy(this.buffer, 0, newbuf, 0, this.length);
				this.buffer = newbuf;
			}
		}

		private void UpdateLineColumnNumbers(int offset)
		{
			int num = offset - 1;
			for (int i = 0; i <= num; i++)
			{
				bool flag = this.buffer[this.pos + i] == '\n';
				if (flag)
				{
					this.line++;
					this.column = 1;
				}
				else
				{
					this.column++;
				}
			}
		}
	}
}
