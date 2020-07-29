using System;
using System.Threading.Tasks;
using AzureGems.CosmosDB;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MagicBus.MessageStore
{
    public class MessageStoreTest
    {

        private readonly ICosmosDbClient _cosmosClient;

        public MessageStoreTest(ICosmosDbClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }


        [FunctionName("MessageStoreTest")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Test Storing a message!");

            var message = new ArchivedMessage(new TestMessage()
            {
                Description = "This is a test message with a random value of " + new Random().Next(0,int.MaxValue)
            });

            ICosmosDbContainer cosmosContainer = await _cosmosClient.GetContainer<ArchivedMessage>();
            CosmosDbResponse<ArchivedMessage> cosmosResponse = await cosmosContainer.Add(message.Id, message);

            return new OkObjectResult($"Cosmos Response: {cosmosResponse.IsSuccessful}");
        }
    }

}