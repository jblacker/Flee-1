using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	[Serializable]
	internal class ParserLogException : Exception
	{
		private ArrayList errors;

		public override string Message
		{
			get
			{
				StringBuilder buffer = new StringBuilder();
				int num = this.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					bool flag = i > 0;
					if (flag)
					{
						buffer.Append("\n");
					}
					buffer.Append(this[i].Message);
				}
				return buffer.ToString();
			}
		}

		public int Count
		{
			get
			{
				return this.errors.Count;
			}
		}

		public ParseException this[int index]
		{
			get
			{
				return (ParseException)this.errors[index];
			}
		}

		public ParserLogException()
		{
			this.errors = new ArrayList();
		}

		private ParserLogException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errors = new ArrayList();
			this.errors = (ArrayList)info.GetValue("Errors", typeof(ArrayList));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Errors", this.errors, typeof(ArrayList));
		}

		public int GetErrorCount()
		{
			return this.Count;
		}

		public ParseException GetError(int index)
		{
			return this[index];
		}

		public void AddError(ParseException e)
		{
			this.errors.Add(e);
		}

		public string GetMessage()
		{
			return this.Message;
		}
	}
}
