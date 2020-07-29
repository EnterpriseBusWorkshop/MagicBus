using System;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using Serilog;

namespace MagicBus.Providers.Messaging
{
    public class LoggingMessageReader: IMessageReader
    {
        private readonly IMessageReader _reader;

        public LoggingMessageReader(IMessageReader reader)
        {
            _reader = reader;
        }

        public MessageBase ReadMessage(string messageBody)
        {
            var message = _reader.ReadMessage(messageBody);
            LogMessageRead(message);
            return message;
        }

        public T ReadMessage<T>(string messageBody) where T : MessageBase
        {
            var message = _reader.ReadMessage<T>(messageBody);
            LogMessageRead(message);
            return message;
        }

        public Type GetMessageType(string messageBody)
        {
            return _reader.GetMessageType(messageBody);
        }
        
        private void LogMessageRead(MessageBase message)
        {
            Log.Information("Message Received. {Type}, {Guid}, {CorrelationId}", 
                message.MessageType,
                message.MessageId,
                message.CorrelationId);
        }

    }
}