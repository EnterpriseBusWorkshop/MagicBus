using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AzureGems.CosmosDb;
using AzureGems.CosmosDB;
using MagicBus.MappingService.Entities;
using MagicBus.Providers.Common;

namespace MagicBus.MappingService.ProductMapping
{
    public interface IProductLookupStore
    {
        Task<ProductLookups> GetProductLookups();

        Task SetProductLookups(ProductLookups lookups);
    }


    public class CosmosProductLookupStore : IProductLookupStore
    {
        private readonly ICosmosDbClient _cosmosDbClient;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CosmosProductLookupStore(ICosmosDbClient cosmosDbClient, IDateTimeProvider dateTimeProvider)
        {
            _cosmosDbClient = cosmosDbClient;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task<ProductLookups> GetProductLookups()
        {
            var cosmosContainer = await _cosmosDbClient.GetContainer<ProductLookups>();
            // get latest lookup config
            IQueryable<ProductLookups> query = cosmosContainer.GetByLinq<ProductLookups>();
            query = query.OrderByDescending(l => l.UpdatedUtc)
                .Take(1);
            
            var cosmosResponse = await cosmosContainer.Resolve(query);
            if (!cosmosResponse.IsSuccessful) throw new CosmosDbException($"Cosmos error fetching product lookup data");

            if (!cosmosResponse.Result.Any())
            {
                var result = await SeedInitialProductMappings();
                return result;
            }
            return cosmosResponse.Result.First();
        }

        private async Task<ProductLookups> SeedInitialProductMappings()
        {
            var cosmosContainer = await _cosmosDbClient.GetContainer<ProductLookups>();
            var lookups = new ProductLookups()
            {
                Products = new List<ProductLookup>()
                {
                    new ProductLookup()
                    {
                        ShopifySku = "50",
                        FulfilmentSku = "HighWispyCloud"
                    } // todo - any more products?
                },
                UpdatedUtc = _dateTimeProvider.UtcNow,
                Id = Guid.NewGuid().ToString()
            };
            var cosmosDbResponse = await cosmosContainer.Add(lookups.Id, lookups);
            if (!cosmosDbResponse.IsSuccessful) throw new CosmosDbException($"Cosmos error product mappings {cosmosDbResponse.ErrorMessage}");
            return lookups;
        }

        public async Task SetProductLookups(ProductLookups lookups)
        {
            var cosmosContainer = await _cosmosDbClient.GetContainer<ProductLookups>();
            lookups.Id = Guid.NewGuid().ToString();
            lookups.UpdatedUtc = _dateTimeProvider.UtcNow;
            var cosmosDbResponse = await cosmosContainer.Add(lookups.Id, lookups);
            if (!cosmosDbResponse.IsSuccessful) throw new CosmosDbException($"Cosmos error product mappings {cosmosDbResponse.ErrorMessage}");
        }
    }
    
}