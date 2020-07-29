using System;

namespace MagicBus.Messages.Common
{
    public class ExceptionMessage : MessageBase
    {
        public string SourceMessageId { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionType { get; set; }
        

        public ExceptionMessage() { }

        public ExceptionMessage(
            MessageBase sourceMessage,
            Exception ex)
        {
            SourceMessageId = sourceMessage?.MessageId;
            CorrelationId = sourceMessage?.CorrelationId;
            StackTrace = ex.StackTrace;
            Message = ex.Message;
            ExceptionType = ex.GetType().FullName;
        }
    }
}