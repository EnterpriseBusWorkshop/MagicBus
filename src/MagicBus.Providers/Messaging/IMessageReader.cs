using System;
using MagicBus.Messages;
using MagicBus.Messages.Common;

namespace MagicBus.Providers.Messaging
{
    public interface IMessageReader
    {
        MessageBase ReadMessage(string messageBody);

        T ReadMessage<T>(string messageBody) where T : MessageBase;

        Type GetMessageType(string messageBody);
    }
}