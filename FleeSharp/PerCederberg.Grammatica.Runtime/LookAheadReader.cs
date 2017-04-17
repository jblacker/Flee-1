// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp.PerCederberg.Grammatica.Runtime
{
    using System;
    using System.IO;
    using System.Threading;
    using Microsoft.VisualBasic.CompilerServices;

    internal class LookAheadReader : TextReader
    {
        private const int StreamBlockSize = 4096;
        private const int BufferBlockSize = 1024;

        private char[] buffer;

        private TextReader input;

        private int length;

        private int pos;

        public LookAheadReader(TextReader input)
        {
            this.buffer = new char[4096];
            this.input = null;
            this.LineNumber = 1;
            this.ColumnNumber = 1;
            this.input = input;
        }

        public override int Read()
        {
            this.ReadAhead(1);
            var flag = this.pos >= this.length;
            int read;
            if (flag)
            {
                read = -1;
            }
            else
            {
                this.UpdateLineColumnNumbers(1);
                read = Convert.ToInt32(this.buffer[Math.Max(Interlocked.Increment(ref this.pos), this.pos - 1)]);
            }
            return read;
        }

        public override int Read(char[] cbuf, int off, int len)
        {
            this.ReadAhead(len);
            var flag = this.pos >= this.length;
            int read;
            if (flag)
            {
                read = -1;
            }
            else
            {
                var count = this.length - this.pos;
                var flag2 = count > len;
                if (flag2)
                {
                    count = len;
                }
                this.UpdateLineColumnNumbers(count);
                Array.Copy(this.buffer, this.pos, cbuf, off, count);
                this.pos += count;
                read = count;
            }
            return read;
        }

        public string ReadString(int len)
        {
            this.ReadAhead(len);
            var flag = this.pos >= this.length;
            string readString;
            if (flag)
            {
                readString = null;
            }
            else
            {
                var count = this.length - this.pos;
                var flag2 = count > len;
                if (flag2)
                {
                    count = len;
                }
                this.UpdateLineColumnNumbers(count);
                var result = new string(this.buffer, this.pos, count);
                this.pos += count;
                readString = result;
            }
            return readString;
        }

        public override int Peek()
        {
            return this.Peek(0);
        }

        public int Peek(int off)
        {
            this.ReadAhead(off + 1);
            var flag = this.pos + off >= this.length;
            int peek;
            if (flag)
            {
                peek = -1;
            }
            else
            {
                peek = Convert.ToInt32(this.buffer[this.pos + off]);
            }
            return peek;
        }

        public string PeekString(int off, int len)
        {
            this.ReadAhead(off + len + 1);
            var flag = this.pos + off >= this.length;
            string peekString;
            if (flag)
            {
                peekString = null;
            }
            else
            {
                var count = this.length - (this.pos + off);
                var flag2 = count > len;
                if (flag2)
                {
                    count = len;
                }
                peekString = new string(this.buffer, this.pos + off, count);
            }
            return peekString;
        }

        public override void Close()
        {
            this.buffer = null;
            this.pos = 0;
            this.length = 0;
            var flag = this.input != null;
            if (flag)
            {
                this.input.Close();
                this.input = null;
            }
        }

        private void ReadAhead(int offset)
        {
            var flag = this.input == null || this.pos + offset < this.length;
            if (!flag)
            {
                var flag2 = this.pos > 1024;
                if (flag2)
                {
                    Array.Copy(this.buffer, this.pos, this.buffer, 0, this.length - this.pos);
                    this.length -= this.pos;
                    this.pos = 0;
                }
                var size = this.pos + offset - this.length + 1;
                var flag3 = size % 4096 != 0;
                if (flag3)
                {
                    size = (int) Math.Round(size / 4096.0 * 4096.0);
                    size += 4096;
                }
                this.EnsureBufferCapacity(this.length + size);
                int readSize;
                try
                {
                    readSize = this.input.Read(this.buffer, this.length, size);
                }
                catch (IOException exprEe)
                {
                    ProjectData.SetProjectError(exprEe);
                    this.input = null;
                    throw;
                }
                var flag4 = readSize > 0;
                if (flag4)
                {
                    this.length += readSize;
                }
                var flag5 = readSize < size;
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
            var flag = this.buffer.Length >= size;
            if (!flag)
            {
                var flag2 = size % 1024 != 0;
                if (flag2)
                {
                    size = (int) Math.Round(size / 1024.0 * 1024.0);
                    size += 1024;
                }
                var newbuf = new char[size - 1 + 1];
                Array.Copy(this.buffer, 0, newbuf, 0, this.length);
                this.buffer = newbuf;
            }
        }

        private void UpdateLineColumnNumbers(int offset)
        {
            var num = offset - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = this.buffer[this.pos + i] == '\n';
                if (flag)
                {
                    this.LineNumber++;
                    this.ColumnNumber = 1;
                }
                else
                {
                    this.ColumnNumber++;
                }
            }
        }

        public int ColumnNumber { get; private set; }

        public int LineNumber { get; private set; }
    }
}