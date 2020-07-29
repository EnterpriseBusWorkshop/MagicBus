using System;
using System.Text;
using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace MagicBus.Providers.Messaging
{
    public class ServiceBusMessageSender: IMessageSender
    {
        private readonly ServiceBusTopicConnectionDetails _config;

        public ServiceBusMessageSender(ServiceBusTopicConnectionDetails config)
        {
            _config = config;
        }

        public async Task SendMessage(MessageBase message)
        {
            if (string.IsNullOrEmpty(_config.ServiceBusConnectionString)) throw new ArgumentException("No Service Bus Connection String is configured");
            if (string.IsNullOrEmpty(_config.TopicName)) throw new ArgumentException("No Service Bus topic Name is configured");

            var topicClient = new TopicClient(_config.ServiceBusConnectionString, _config.TopicName);
            var busMessage = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)))
            {
                Label = message.GetType().Name, // we use label to route messages by body type
                ContentType = "application/json",
                CorrelationId = message.CorrelationId
            };
            await topicClient.SendAsync(busMessage);
            await topicClient.CloseAsync();
        }

        public async Task SendException(Exception ex, MessageBase sourceMessage)
        {
            if (string.IsNullOrEmpty(_config.ServiceBusConnectionString)) throw new ArgumentException("No Service Bus Connection String is configured");
            if (string.IsNullOrEmpty(_config.TopicName)) throw new ArgumentException("No Service Bus topic Name is configured");

            var exceptionMessage = new ExceptionMessage(sourceMessage, ex);
            await SendMessage(exceptionMessage);
        }
    }
}