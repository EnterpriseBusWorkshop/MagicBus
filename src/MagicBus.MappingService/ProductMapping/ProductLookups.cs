using System;
using System.Collections.Generic;
using AzureGems.Repository.Abstractions;
using MagicBus.MappingService.Entities;

namespace MagicBus.MappingService.ProductMapping
{
    public class ProductLookups: BaseEntity
    {
        public DateTime UpdatedUtc { get; set; }
        public IList<ProductLookup> Products { get; set; } = new List<ProductLookup>();
    }
}