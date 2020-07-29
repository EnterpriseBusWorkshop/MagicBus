using MagicBus.MappingService.CosmosDb;
using MagicBus.MappingService.Entities;
using MagicBus.MappingService.ProductMapping;
using MagicBus.Messages.Orders;
using MagicBus.Providers.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MagicBus.MappingService.Handlers
{
	public class OrderReceivedHandler : AsyncRequestHandler<OrderReceived>
    {

        private readonly MappingServiceCosmosContext _cosmos;
        private readonly IMessageSender _messageSender;
        private readonly IProductLookupStore _productLookupStore;
        private readonly ILogger<OrderReceivedHandler> _logger;

        public OrderReceivedHandler(MappingServiceCosmosContext cosmos, IMessageSender messageSender, IProductLookupStore productLookupStore, ILogger<OrderReceivedHandler> logger)
        {
            _cosmos = cosmos;
            _messageSender = messageSender;
            _productLookupStore = productLookupStore;
            _logger = logger;
        }

        protected override async Task Handle(OrderReceived request, CancellationToken cancellationToken)
        {
            var isAlreadySubmitted = (await _cosmos.CloudOrders
                .Get(o => o.ShopifyOrderId == request.OrderDetails.Id.Value.ToString())).Any();

            if (isAlreadySubmitted)
            {
                _logger.LogWarning("Order {OrderId} already processed", request.OrderDetails.Id);
                return;
            }

            var mapEntity = new CloudOrderMapping()
            {
                ShopifyOrderId = request.OrderDetails.Id?.ToString(),
                FulfilmentOrderId = Guid.NewGuid().ToString()
            };
            await _cosmos.CloudOrders.Add(mapEntity);
            
            var nextMessage = new FulfilOrder()
            {
                OrderId = mapEntity.FulfilmentOrderId,
                EmailAddress = request.OrderDetails.Email,
                Name = request.OrderDetails.Name,
                Sku = await GetFulfilmentSku(request.OrderDetails.LineItems.First().SKU),
                CorrelationId = request.CorrelationId
            };
            
            await _messageSender.SendMessage(nextMessage);
            
        }

        

        private async Task<string> GetFulfilmentSku(string sku)
        {
            var lookups = await _productLookupStore.GetProductLookups();
            return lookups.Products
                .Where(p => p.ShopifySku == sku)
                .Select(p => p.FulfilmentSku)
                .FirstOrDefault();
        }
    }
}