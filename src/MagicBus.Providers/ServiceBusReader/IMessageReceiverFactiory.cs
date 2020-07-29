using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace MagicBus.Providers.ServiceBusReader
{
    public interface IMessageReceiverFactory
    {
        IMessageReceiver Create(string queueName, ReceiveMode receiveMode, bool isDeadLetterQueue = false);
    }
}