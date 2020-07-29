﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AzureGems.CosmosDb;
using AzureGems.CosmosDB;
using AzureGems.Repository.Abstractions;

namespace AzureGems.Repository.CosmosDB
{
	public class CosmosDbContainerRepository<TDomainEntity> : IRepository<TDomainEntity> where TDomainEntity : BaseEntity
	{
		private readonly IIdValueGenerator<TDomainEntity> _idValueGenerator;

		// TODO: Move to the container level and reuse it there because we have the generic type and container definition,
		// TODO: Because T in IContainer does not implement BaseEntity, set the discriminator via reflection

		private readonly IPartitionKeyResolver _pkvResolver;
		private readonly string _entityType;

		public ICosmosDbContainer Container { get; }

		public CosmosDbContainerRepository(
			ICosmosDbContainer container,
			IEntityTypeNameResolver entityTypeNameResolver,
			IIdValueGenerator<TDomainEntity> idValueGenerator,
			IPartitionKeyResolver pkvResolver
			)
		{
			Container = container;
			_idValueGenerator = idValueGenerator;
			_pkvResolver = pkvResolver;
			_entityType = entityTypeNameResolver.ResolveEntityTypeName<TDomainEntity>();
		}

		private string ResolvePartitionKeyValue(TDomainEntity entity)
		{
			return _pkvResolver.ResolvePartitionKeyValue(Container.Definition.PartitionKeyPath, entity);
		}

		private async Task<IEnumerable<TDomainEntity>> Resolve(IQueryable<TDomainEntity> query)
		{
			CosmosDbResponse<IEnumerable<TDomainEntity>> resolvedQuery = await Container.Resolve(query);
            return resolvedQuery.Result;
		}
		
		public async Task<IEnumerable<TDomainEntity>> Get(Expression<Func<TDomainEntity, bool>> predicate)
		{
			IQueryable<TDomainEntity> query = Container.GetByLinq<TDomainEntity>()
				// add the predicate
				.Where(predicate);

			CosmosDbResponse<IEnumerable<TDomainEntity>> response = await Container.Resolve(query);
			if (!response.IsSuccessful) throw new CosmosDbException($"GET Error: {response.ErrorMessage}", typeof(TDomainEntity));
			return response.Result;
		}

		public async Task<TDomainEntity> GetById(string id)
		{
			CosmosDbResponse<TDomainEntity> response = await Container.Get<TDomainEntity>(id);
            if (!response.IsSuccessful) throw new CosmosDbException($"GetById Error: {response.ErrorMessage}", typeof(TDomainEntity), id);
			return response.Result;
		}

		public async Task<TDomainEntity> Add(TDomainEntity entity)
		{
			// TODO: Move the ID Value Generator and Discriminator Setter to the lower level container

			// always set new ID
			entity.Id = _idValueGenerator.Generate(entity);

			// always set the entity type / Discriminator
			entity.Discriminator = _entityType;

			CosmosDbResponse<TDomainEntity> response = await Container.Add(ResolvePartitionKeyValue(entity), entity);
            if (!response.IsSuccessful) throw new CosmosDbException($"Add Error: {response.ErrorMessage}", typeof(TDomainEntity), entity.Id);
			return response.Result;
		}

		public async Task<TDomainEntity> Delete(TDomainEntity entity)
		{
			CosmosDbResponse<TDomainEntity> deletedEntity = await Container.Delete<TDomainEntity>(ResolvePartitionKeyValue(entity), entity.Id);
            if (!deletedEntity.IsSuccessful) throw new CosmosDbException($"DELETE Error: {deletedEntity.ErrorMessage}", typeof(TDomainEntity), entity.Id);
			return deletedEntity.Result;
		}

		public async Task<TDomainEntity> Update(TDomainEntity entity)
		{
			CosmosDbResponse<TDomainEntity> updatedEntity = await Container.Update(ResolvePartitionKeyValue(entity), entity);
            if (!updatedEntity.IsSuccessful) throw new CosmosDbException($"UPDATE Error: {updatedEntity.ErrorMessage}", typeof(TDomainEntity), entity.Id);
			return updatedEntity.Result;
		}
	}
}