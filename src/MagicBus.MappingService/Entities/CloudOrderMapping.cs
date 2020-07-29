using AzureGems.Repository.Abstractions;

namespace MagicBus.MappingService.Entities
{
    public class CloudOrderMapping: BaseEntity
    {
        public string ShopifyOrderId { get; set; }

        public string FulfilmentOrderId { get; set; }
        
    }
}