using System;
using System.Threading;
using System.Threading.Tasks;
using AzureGems.CosmosDB;
using MagicBus.Messages.Common;
using MagicBus.Providers.Common;
using MagicBus.Providers.Messaging;
using MediatR;

namespace MagicBus.AdminPortal.Application.Messages
{
    public class ResubmitMessage: IRequest<bool>
    {
        public string MessageId { get; }


        public ResubmitMessage(string messageId)
        {
            MessageId = messageId;
        }
    }



    public class ResubmitMessageHandler : IRequestHandler<ResubmitMessage, bool>
    {
        private readonly ICosmosDbClient _cosmosDbClient;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMessageSender _messageSender;

        public ResubmitMessageHandler(ICosmosDbClient cosmosDbClient, IDateTimeProvider dateTimeProvider, IMessageSender messageSender)
        {
            _cosmosDbClient = cosmosDbClient;
            _dateTimeProvider = dateTimeProvider;
            _messageSender = messageSender;
        }

        public async Task<bool> Handle(ResubmitMessage request, CancellationToken cancellationToken)
        {
            ICosmosDbContainer messagesContainer = await _cosmosDbClient.GetContainer("messages");
            var cosmosResponse = await messagesContainer.Get<ArchivedMessage>(request.MessageId, request.MessageId);
            if (!cosmosResponse.IsSuccessful) throw new ApplicationException("Cosmos error: "+cosmosResponse.ErrorMessage);

            var archivedMessage = cosmosResponse.Result;
            if (archivedMessage != null && archivedMessage.Message != null)
            {
                var message = archivedMessage.Message;
                message.MessageId = Guid.NewGuid().ToString();
                message.MessageDate = _dateTimeProvider.UtcNow;
                await _messageSender.SendMessage(message);
                return true;
            }
            return false;
        }
    }

}
