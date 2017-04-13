using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	[Serializable]
	internal class ParseException : Exception
	{
		public enum ErrorType
		{
			INTERNAL,
			IO,
			UNEXPECTED_EOF,
			UNEXPECTED_CHAR,
			UNEXPECTED_TOKEN,
			INVALID_TOKEN,
			ANALYSIS
		}

		private ParseException.ErrorType m_type;

		private string m_info;

		private ArrayList m_details;

		private int m_line;

		private int m_column;

		public ParseException.ErrorType Type
		{
			get
			{
				return this.m_type;
			}
		}

		public string Info
		{
			get
			{
				return this.m_info;
			}
		}

		public ArrayList Details
		{
			get
			{
				return new ArrayList(this.m_details);
			}
		}

		public int Line
		{
			get
			{
				return this.m_line;
			}
		}

		public int Column
		{
			get
			{
				return this.m_column;
			}
		}

		public override string Message
		{
			get
			{
				StringBuilder buffer = new StringBuilder();
				buffer.AppendLine(this.ErrorMessage);
				bool flag = this.m_line > 0 && this.m_column > 0;
				if (flag)
				{
					string msg = FleeResourceManager.Instance.GetCompileErrorString("LineColumn");
					msg = string.Format(msg, this.m_line, this.m_column);
					buffer.AppendLine(msg);
				}
				return buffer.ToString();
			}
		}

		public string ErrorMessage
		{
			get
			{
				List<string> args = new List<string>();
				switch (this.m_type)
				{
				case ParseException.ErrorType.INTERNAL:
				case ParseException.ErrorType.IO:
				case ParseException.ErrorType.UNEXPECTED_CHAR:
				case ParseException.ErrorType.INVALID_TOKEN:
				case ParseException.ErrorType.ANALYSIS:
					args.Add(this.m_info);
					break;
				case ParseException.ErrorType.UNEXPECTED_TOKEN:
					args.Add(this.m_info);
					args.Add(this.GetMessageDetails());
					break;
				}
				string msg = FleeResourceManager.Instance.GetCompileErrorString(this.m_type.ToString());
				return string.Format(msg, args.ToArray());
			}
		}

		public ParseException(ParseException.ErrorType type, string info, int line, int column) : this(type, info, null, line, column)
		{
		}

		public ParseException(ParseException.ErrorType type, string info, ArrayList details, int line, int column)
		{
			this.m_type = type;
			this.m_info = info;
			this.m_details = details;
			this.m_line = line;
			this.m_column = column;
		}

		private ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.m_type = (ParseException.ErrorType)info.GetInt32("Type");
			this.m_info = info.GetString("Info");
			this.m_details = (ArrayList)info.GetValue("Details", typeof(ArrayList));
			this.m_line = info.GetInt32("Line");
			this.m_column = info.GetInt32("Column");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Type", (int)this.m_type);
			info.AddValue("Info", this.m_info);
			info.AddValue("Details", this.m_details);
			info.AddValue("Line", this.m_line);
			info.AddValue("Column", this.m_column);
		}

		public ParseException.ErrorType GetErrorType()
		{
			return this.Type;
		}

		public string GetInfo()
		{
			return this.Info;
		}

		public ArrayList GetDetails()
		{
			return this.Details;
		}

		public int GetLine()
		{
			return this.Line;
		}

		public int GetColumn()
		{
			return this.m_column;
		}

		public string GetMessage()
		{
			return this.Message;
		}

		public string GetErrorMessage()
		{
			return this.ErrorMessage;
		}

		private string GetMessageDetails()
		{
			StringBuilder buffer = new StringBuilder();
			int num = this.m_details.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					buffer.Append(", ");
					bool flag2 = i + 1 == this.m_details.Count;
					if (flag2)
					{
						buffer.Append("or ");
					}
				}
				buffer.Append(RuntimeHelpers.GetObjectValue(this.m_details[i]));
			}
			return buffer.ToString();
		}
	}
}
