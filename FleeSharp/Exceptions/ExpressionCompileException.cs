namespace Flee.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using PerCederberg.Grammatica.Runtime;

    [Serializable]
    public sealed class ExpressionCompileException : Exception
    {
        private CompileExceptionReason MyReason;

        public override string Message
        {
            get
            {
                bool flag = this.MyReason == CompileExceptionReason.SyntaxError;
                string Message;
                if (flag)
                {
                    Exception innerEx = this.InnerException;
                    string msg = string.Format("{0}: {1}", Utility.GetCompileErrorMessage("SyntaxError", new object[0]), innerEx.Message);
                    Message = msg;
                }
                else
                {
                    Message = base.Message;
                }
                return Message;
            }
        }

        public CompileExceptionReason Reason
        {
            get
            {
                return this.MyReason;
            }
        }

        internal ExpressionCompileException(string message, CompileExceptionReason reason) : base(message)
        {
            this.MyReason = reason;
        }

        internal ExpressionCompileException(ParserLogException parseException) : base(string.Empty, parseException)
        {
            this.MyReason = CompileExceptionReason.SyntaxError;
        }

        private ExpressionCompileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.MyReason = (CompileExceptionReason)info.GetInt32("Reason");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Reason", (int)this.MyReason);
        }
    }
}