using System;

namespace AzureGems.CosmosDb
{
    public class CosmosDbException : Exception
    {
        /// <summary>
        /// the entity type we're working with (if known)
        /// </summary>
        public Type EntityType { get; }


        /// <summary>
        /// entity id we're working with (if known)
        /// </summary>
        public string EntityId { get; }


        public CosmosDbException(string message) : base(message) { }

        public CosmosDbException(string message, Exception inner) : base(message, inner) { }

        public CosmosDbException(string message, Type entityType) : base(
            $"{message}; with entityType: {entityType.Name}")
        {
            EntityType = entityType;
        }

        public CosmosDbException(string message, Type entityType, string entityId) : base(
            $"{message}; with entityType: {entityType.Name}, id: {entityId}")
        {
            EntityId = entityId;
            EntityType = entityType;
        }
    }
}
