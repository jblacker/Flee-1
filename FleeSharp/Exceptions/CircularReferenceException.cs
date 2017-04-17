namespace Flee.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
	public class CircularReferenceException : Exception
	{
		private string MyCircularReferenceSource;

		public override string Message
		{
			get
			{
				bool flag = this.MyCircularReferenceSource == null;
				string Message;
				if (flag)
				{
					Message = "Circular reference detected in calculation engine";
				}
				else
				{
					Message = string.Format("Circular reference detected in calculation engine at '{0}'", this.MyCircularReferenceSource);
				}
				return Message;
			}
		}

		internal CircularReferenceException()
		{
		}

		internal CircularReferenceException(string circularReferenceSource)
		{
			this.MyCircularReferenceSource = circularReferenceSource;
		}

		private CircularReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
