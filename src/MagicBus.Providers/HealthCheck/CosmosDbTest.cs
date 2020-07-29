using System.Threading;
using System.Threading.Tasks;
using AzureGems.CosmosDB;
using MagicBus.Messages;
using MagicBus.Messages.Common;

namespace MagicBus.Providers.HealthCheck
{
   public class CosmosDbTest: IHealthCheckTest
   {

       private readonly ICosmosDbClient _cosmosClient;
       private readonly string _containerName;

       public CosmosDbTest(ICosmosDbClient cosmosClient, string containerName)
       {
           _cosmosClient = cosmosClient;
           _containerName = containerName;
       }


       public string Name { get; set; } = "Cosmos DB Connection";


       public async Task<HealthCheckTestResult> RunTest(CancellationToken ct)
       {
           var c = await _cosmosClient.GetContainer(_containerName);

           return new HealthCheckTestResult()
           {
               Message = $"Successfully connected to container {c.SdkContainer.Id}",
               Name = this.Name,
               ResponseStatus = HealthCheckResponseStatus.Success
           };

       }
    }
}
