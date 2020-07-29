using System;
using System.Threading.Tasks;
using AzureGems.CosmosDB;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using MagicBus.Providers.HealthCheck;
using MagicBus.Providers.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MagicBus.MessageStore
{
    public class StoreMessages
    {
        private readonly IMessageReader _messageReader;
        private readonly IMessageSender _messageSender;
        private readonly ICosmosDbClient _cosmosClient;
        private readonly IHealthTester _healthTester;

        public StoreMessages(IMessageReader messageReader, IMessageSender messageSender, ICosmosDbClient cosmosClient, IHealthTester healthTester)
        {
            _messageReader = messageReader;
            _messageSender = messageSender;
            _cosmosClient = cosmosClient;
            _healthTester = healthTester;
        }

        [FunctionName(nameof(StoreMessages))]
        public async Task Run([ServiceBusTrigger("%ServiceBusTopic%", "%ServiceBusSubscription%", Connection = "ServiceBusConnectionString")]string messageString, ILogger log)
        {
            MessageBase originalMessage = null;
            try
            {
                originalMessage = _messageReader.ReadMessage(messageString);
                var archivedMessage = new ArchivedMessage(originalMessage);

                if (archivedMessage.MessageTypeName.Equals(nameof(HealthCheckRequest)))
                {
                    await HandleHealthCheckRequest(_messageReader.ReadMessage<HealthCheckRequest>(messageString));
                }

                ICosmosDbContainer cosmosContainer = await _cosmosClient.GetContainer<ArchivedMessage>();
                CosmosDbResponse<ArchivedMessage> cosmosResponse =
                    await cosmosContainer.Add(archivedMessage.Id, archivedMessage);

                if (!cosmosResponse.IsSuccessful)
                {
                    throw new ApplicationException(
                        $"MessageStorage failed to write message {archivedMessage.Message.MessageId} of type {archivedMessage.Message.MessageType}. {cosmosResponse.ErrorMessage}");
                }
            }
            catch (Exception storeException)
            {
                try
                {
                    log.LogError(storeException, "exception saving to MessageStorage. OriginalMessage: "+messageString);
                    await _messageSender.SendException(storeException, originalMessage);
                }
                catch (Exception sendException)
                {
                    log.LogCritical(sendException, "Exception attempting to save an error message to the bus! This is very bad!");
                }
                throw;
            }

        }

        private async Task HandleHealthCheckRequest(HealthCheckRequest message)
        {
            var response = await _healthTester.RunHealthCheck(message);
            await _messageSender.SendMessage(response);
        }
    }

}