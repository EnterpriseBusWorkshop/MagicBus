using AzureGems.Repository.Abstractions;
using MagicBus.MappingService.Entities;
using MagicBus.MappingService.ProductMapping;

namespace MagicBus.MappingService.CosmosDb
{
    public class MappingServiceCosmosContext: CosmosContext
    {
        public IRepository<CloudOrderMapping> CloudOrders { get; set; }
        
        public IRepository<ProductLookups> ProductLookups { get; set; }
    }
}