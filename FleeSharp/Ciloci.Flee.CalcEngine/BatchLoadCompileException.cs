using System;
using System.Runtime.Serialization;

namespace Ciloci.Flee.CalcEngine
{
	[Serializable]
	public class BatchLoadCompileException : Exception
	{
		private string MyAtomName;

		private string MyExpressionText;

		public string AtomName
		{
			get
			{
				return this.MyAtomName;
			}
		}

		public string ExpressionText
		{
			get
			{
				return this.MyExpressionText;
			}
		}

		internal BatchLoadCompileException(string atomName, string expressionText, ExpressionCompileException innerException) : base(string.Format("Batch Load: The expression for atom '${0}' could not be compiled", atomName), innerException)
		{
			this.MyAtomName = atomName;
			this.MyExpressionText = expressionText;
		}

		private BatchLoadCompileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.MyAtomName = info.GetString("AtomName");
			this.MyExpressionText = info.GetString("ExpressionText");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("AtomName", this.MyAtomName);
			info.AddValue("ExpressionText", this.MyExpressionText);
		}
	}
}
