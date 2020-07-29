using System;
using System.IO;
using System.Threading.Tasks;
using MagicBus.MappingService.CosmosDb;
using MagicBus.MappingService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MagicBus.MappingService
{
    public class CosmosTest
    {

        private readonly MappingServiceCosmosContext _ctx;

        public CosmosTest(MappingServiceCosmosContext ctx)
        {
            _ctx = ctx;
        }

        [FunctionName("CosmosTest")]
        public  async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _ctx.CloudOrders.Add(new CloudOrderMapping()
            {
                Id = Guid.NewGuid().ToString(),
				ShopifyOrderId = Guid.NewGuid().ToString()
            });

            return new OkObjectResult("Saved OK");
        }
    }
}