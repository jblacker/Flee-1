using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class ParserCreationException : Exception
	{
		public enum ErrorType
		{
			INTERNAL,
			INVALID_PARSER,
			INVALID_TOKEN,
			INVALID_PRODUCTION,
			INFINITE_LOOP,
			INHERENT_AMBIGUITY
		}

		private ParserCreationException.ErrorType m_type;

		private string m_name;

		private string m_info;

		private ArrayList m_details;

		public ParserCreationException.ErrorType Type
		{
			get
			{
				return this.m_type;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string Info
		{
			get
			{
				return this.m_info;
			}
		}

		public string Details
		{
			get
			{
				StringBuilder buffer = new StringBuilder();
				bool flag = this.m_details == null;
				string Details;
				if (flag)
				{
					Details = null;
				}
				else
				{
					int num = this.m_details.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						bool flag2 = i > 0;
						if (flag2)
						{
							buffer.Append(", ");
							bool flag3 = i + 1 == this.m_details.Count;
							if (flag3)
							{
								buffer.Append("and ");
							}
						}
						buffer.Append(RuntimeHelpers.GetObjectValue(this.m_details[i]));
					}
					Details = buffer.ToString();
				}
				return Details;
			}
		}

		public override string Message
		{
			get
			{
				StringBuilder buffer = new StringBuilder();
				switch (this.m_type)
				{
				case ParserCreationException.ErrorType.INVALID_PARSER:
					buffer.Append("parser is invalid, as ");
					buffer.Append(this.m_info);
					break;
				case ParserCreationException.ErrorType.INVALID_TOKEN:
					buffer.Append("token '");
					buffer.Append(this.m_name);
					buffer.Append("' is invalid, as ");
					buffer.Append(this.m_info);
					break;
				case ParserCreationException.ErrorType.INVALID_PRODUCTION:
					buffer.Append("production '");
					buffer.Append(this.m_name);
					buffer.Append("' is invalid, as ");
					buffer.Append(this.m_info);
					break;
				case ParserCreationException.ErrorType.INFINITE_LOOP:
					buffer.Append("infinite loop found in production pattern '");
					buffer.Append(this.m_name);
					buffer.Append("'");
					break;
				case ParserCreationException.ErrorType.INHERENT_AMBIGUITY:
				{
					buffer.Append("inherent ambiguity in production '");
					buffer.Append(this.m_name);
					buffer.Append("'");
					bool flag = this.m_info != null;
					if (flag)
					{
						buffer.Append(" ");
						buffer.Append(this.m_info);
					}
					bool flag2 = this.m_details != null;
					if (flag2)
					{
						buffer.Append(" starting with ");
						bool flag3 = this.m_details.Count > 1;
						if (flag3)
						{
							buffer.Append("tokens ");
						}
						else
						{
							buffer.Append("token ");
						}
						buffer.Append(this.Details);
					}
					break;
				}
				default:
					buffer.Append("internal error");
					break;
				}
				return buffer.ToString();
			}
		}

		public ParserCreationException(ParserCreationException.ErrorType type, string info) : this(type, null, info)
		{
		}

		public ParserCreationException(ParserCreationException.ErrorType type, string name, string info) : this(type, name, info, null)
		{
		}

		public ParserCreationException(ParserCreationException.ErrorType type, string name, string info, ArrayList details)
		{
			this.m_type = type;
			this.m_name = name;
			this.m_info = info;
			this.m_details = details;
		}

		public ParserCreationException.ErrorType GetErrorType()
		{
			return this.Type;
		}

		public string GetName()
		{
			return this.Name;
		}

		public string GetInfo()
		{
			return this.Info;
		}

		public string GetDetails()
		{
			return this.Details;
		}

		public string GetMessage()
		{
			return this.Message;
		}
	}
}
