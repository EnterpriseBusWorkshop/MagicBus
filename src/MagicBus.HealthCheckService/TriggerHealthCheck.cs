using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using MagicBus.Providers.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MagicBus.HealthCheckService
{
    /// <summary>
    /// test http trigger function that sends a healthcheck request 
    /// </summary>
    public class TriggerHealthCheck
    {
        private readonly IMessageSender _messageSender;

        public TriggerHealthCheck(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }


        [FunctionName(nameof(TriggerHealthCheck))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req)
        {
            await _messageSender.SendMessage(new HealthCheckRequest()
            {
				// TODO: Inject health check config
                ExpectedServices = new List<string>(){"shopify"}
            });
            return new OkObjectResult("OK");
        }
    }
}