using MagicBus.Messages.Orders;
using MagicBus.Providers.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MagicBus.Shop
{
	public class OrderCreated
    {
        private readonly IMessageSender _messageSender;

        public OrderCreated(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }
		
        [FunctionName(nameof(OrderCreated))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string bodyText = await new StreamReader(req.Body).ReadToEndAsync();
                var o = JsonConvert.DeserializeObject<ShopifySharp.Order>(bodyText);
                var id = Guid.NewGuid().ToString();

                log.LogInformation("Sending OrderReceived {messageId} message to the bus!", id);
				await _messageSender.SendMessage(new OrderReceived()
                {
                    MessageId = id,
                    CorrelationId = id,
                    OrderDetails = o
                });
            
                return new OkObjectResult($"message sent: {o.OrderNumber}");
            }
            catch (Exception ex)
            {
				log.LogError(ex, "Could not read/send Shopify order!");
                await _messageSender.SendException(ex, null);
                throw;
            }
        }
    }
}