﻿using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureGems.CosmosDB
{
	public class CosmosDbClient : ICosmosDbClient, IDisposable
	{
		private readonly ContainerDefinition[] _containerDefinitions;
		private readonly CosmosClient _sdkClient;
		private readonly AsyncLazy<Database> _lazyDatabase;

		private async Task<Database> GetDatabase()
		{
			return await _lazyDatabase.Value;
		}

		public ContainerDefinition GetContainerDefinition(string containerId)
		{
			ContainerDefinition containerDef = _containerDefinitions.FirstOrDefault(def => def.ContainerId == containerId);
			return containerDef;
		}

		public ContainerDefinition GetContainerDefinitionForType(Type t)
		{
			ContainerDefinition containerDefForT = _containerDefinitions.FirstOrDefault(def => def.EntityType == t);
			return containerDefForT;
		}

		public ContainerDefinition GetContainerDefinitionForType<T>()
		{
			Type t = typeof(T);
			return GetContainerDefinitionForType(t);
		}

		public CosmosDbClient(
			CosmosDbConnectionSettings connectionSettings,
			CosmosDbConfig dbConfig,
			IEnumerable<ContainerDefinition> containerDefinitions)
		{

			IEnumerable<ContainerDefinition> definitions = containerDefinitions as ContainerDefinition[] ?? containerDefinitions.ToArray();
			_containerDefinitions = definitions.ToArray();

			_sdkClient = new CosmosClient(
				connectionSettings.EndPoint,
				connectionSettings.AuthKey,
				new CosmosClientOptions()
				{
					Serializer = new ConfigurableCosmosSerializer(
						new JsonSerializerSettings()
						{
							NullValueHandling = NullValueHandling.Ignore,
							TypeNameHandling = TypeNameHandling.All,
							MissingMemberHandling = MissingMemberHandling.Ignore
						}),
					ConnectionMode = ConnectionMode.Direct
				});

			_lazyDatabase = new AsyncLazy<Database>(async () =>
			{
				DatabaseResponse resp = await _sdkClient.CreateDatabaseIfNotExistsAsync(dbConfig.DatabaseId, dbConfig.Throughput);

				foreach (ContainerDefinition containerDefinition in definitions)
				{
					await Internal_EnsureContainerExists(resp.Database, containerDefinition);
				}

				return resp.Database;
			});
		}

		private static async Task<Container> Internal_EnsureContainerExists(Database db, ContainerDefinition containerDefinition)
		{
			return await Internal_EnsureContainerExists(db, containerDefinition.ContainerId, containerDefinition.PartitionKeyPath, containerDefinition.Throughput);
		}

		private static async Task<Container> Internal_EnsureContainerExists(Database db, string containerId, string partitionKeyPath, int? throughput)
		{
			var containerDefinition = new ContainerProperties(id: containerId, partitionKeyPath: partitionKeyPath);

			ContainerResponse response = await db.CreateContainerIfNotExistsAsync(
				containerProperties: containerDefinition,
				throughput: throughput,
				requestOptions: null);

			return response.Container;
		}

		private async Task<Container> Internal_GetContainer(string containerId)
		{
			Database database = await GetDatabase();
			Container container = database.GetContainer(containerId);
			return container;
		}

		private readonly ConcurrentDictionary<string, ICosmosDbContainer> _containerCache = new ConcurrentDictionary<string, ICosmosDbContainer>();

		public async Task<ICosmosDbContainer> GetContainer(string containerId)
		{
			return await _containerCache.GetOrAddAsync<string, ICosmosDbContainer>(containerId, async id =>
			{
				Container container = await Internal_GetContainer(containerId);
				ContainerDefinition definition = GetContainerDefinition(containerId);
				return new CosmosDbContainer(definition, this, container);
			});
		}

		public async Task<ICosmosDbContainer> GetContainer<TEntity>()
		{
			ContainerDefinition definition = GetContainerDefinitionForType<TEntity>();
			return await GetContainer(definition.ContainerId);
		}

		public void Dispose()
		{
			_sdkClient?.Dispose();
		}
	}
}