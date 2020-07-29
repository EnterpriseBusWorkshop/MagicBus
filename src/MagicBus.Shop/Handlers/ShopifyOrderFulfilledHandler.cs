using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages.Orders;
using MediatR;
using Microsoft.Extensions.Logging;
using ShopifySharp;
using ILogger = Microsoft.Build.Framework.ILogger;

namespace MagicBus.Shop.Handlers
{
	public class ShopifyConfig
	{
		public string ShopUrl { get; set; } 
		
		public string AccessToken { get; set; }
	}

	public class ShopifyOrderFulfilledHandler : AsyncRequestHandler<ShopifyOrderFulfilled>
	{
		private readonly ILogger<ShopifyOrderFulfilledHandler> _logger;
		private readonly ShopifyConfig _config;

		public ShopifyOrderFulfilledHandler(ILogger<ShopifyOrderFulfilledHandler> logger, ShopifyConfig config)
		{
			_logger = logger;
			_config = config;
		}

		protected override async Task Handle(ShopifyOrderFulfilled request, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Fulfilling Order in Shopify: {orderid}", request.ShopifyOrderId);

			try
			{
				var locationService = new LocationService(_config.ShopUrl, _config.AccessToken);
				IEnumerable<Location> locations = await locationService.ListAsync(cancellationToken);
				Location defaultLocation = locations.FirstOrDefault();

				var fulfillmentService = new FulfillmentService(_config.ShopUrl, _config.AccessToken);
				var fulfillment = new Fulfillment()
				{
					TrackingCompany = "Willards Shipping Services",
					TrackingUrl = $"https://willards.com/{request.ShopifyOrderId}",
					TrackingNumber = request.ShopifyOrderId.ToString(),
					LocationId = defaultLocation.Id
				};

				Fulfillment fulfillmentResult = await fulfillmentService.CreateAsync(request.ShopifyOrderId, fulfillment, cancellationToken);
			}
			catch (ShopifyException e)
			{
				_logger.LogError(e, "Couldn't complete the fulfillment");
				throw;
			}
		}
	}
}