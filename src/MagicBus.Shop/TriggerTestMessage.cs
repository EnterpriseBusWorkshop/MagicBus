using System;
using System.IO;
using System.Threading.Tasks;
using MagicBus.Providers.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MagicBus.Shop
{
    public class TriggerTestMessage
    {
        private readonly IMessageSender _messageSender;

        public TriggerTestMessage(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }


        [FunctionName(nameof(TriggerTestMessage))]
        public  async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _messageSender.SendMessage(new Messages.Common.TestMessage()
            {
                Description = $"Test Message - {DateTime.Now.ToString()}"
            });
            
            return new OkObjectResult("Test message sent");

        }
    }
}