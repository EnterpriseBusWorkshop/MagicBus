using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.MappingService.CosmosDb;
using MagicBus.Messages.Orders;
using MagicBus.Providers.Messaging;
using MediatR;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;

namespace MagicBus.MappingService.Handlers
{
    public class OrderFulfilledHandler: AsyncRequestHandler<OrderFulfilled>
    {
        private readonly MappingServiceCosmosContext _cosmos;
        private readonly IMessageSender _messageSender;

        public OrderFulfilledHandler(MappingServiceCosmosContext cosmos, IMessageSender messageSender)
        {
            _cosmos = cosmos;
            _messageSender = messageSender;
        }

        protected override async Task Handle(OrderFulfilled request, CancellationToken cancellationToken)
        {
            var mapEntity = (await _cosmos.CloudOrders
                    .Get(c => c.FulfilmentOrderId == request.OrderId))
                .FirstOrDefault();

            if (mapEntity == null) throw new ApplicationException($"Failed to find order record for fulfilment record {request.OrderId}");

            await _messageSender.SendMessage(new ShopifyOrderFulfilled()
            {
                ShopifyOrderId = Convert.ToInt64(mapEntity.ShopifyOrderId),
				CorrelationId = request.CorrelationId
            });

        }
    }
}