using MagicBus.Providers.Messaging;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace MagicBus.Providers.ServiceBusReader
{
    public class MessageReceiverFactory: IMessageReceiverFactory
    {

        private readonly ServiceBusTopicConnectionDetails _connectionDetails;
        
        public MessageReceiverFactory(ServiceBusTopicConnectionDetails connectionDetails)
        {
            _connectionDetails = connectionDetails;
        }
        
        public IMessageReceiver Create(string queueName, ReceiveMode receiveMode, bool isDeadLetterQueue = false)
        {
            string path = EntityNameHelper.FormatSubscriptionPath(_connectionDetails.TopicName, queueName);
            if (isDeadLetterQueue)
            {
                path = EntityNameHelper.FormatDeadLetterPath(path);
            }
            return new MessageReceiver(_connectionDetails.ServiceBusConnectionString, path, receiveMode, RetryPolicy.NoRetry, 0);
        }
    }

}