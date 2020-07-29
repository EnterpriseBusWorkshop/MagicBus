using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
	
namespace AzureGems.CosmosDB
{
	public class CosmosDbClientBuilder
	{
		public IServiceCollection Services { get; }

		private readonly List<ContainerDefinition> _containerDefinitions = new List<ContainerDefinition>();
		private CosmosDbConnectionSettings _connectionSettings = null;
		private CosmosDbConfig _dbconfig = new CosmosDbConfig(null, null);

		public CosmosDbClientBuilder(IServiceCollection services)
		{
			Services = services;
		}

		public CosmosDbClientBuilder ReadConfiguration(IConfiguration config)
		{
			_connectionSettings = new CosmosDbConnectionSettings(config);
			_dbconfig = new CosmosDbConfig(config);
			return this;
		}


        public CosmosDbClientBuilder WithDbConfig(CosmosDbConfig config)
        {
            _dbconfig = config;
            return this;
        }

		public CosmosDbClientBuilder UseDatabase(string databaseId)
        {
            _dbconfig.DatabaseId = databaseId;
			return this;
		}

		public CosmosDbClientBuilder WithDatabaseThroughput(int throughput)
        {
            _dbconfig.Throughput = throughput;
            return this;
		}

        

		public CosmosDbClientBuilder ConnectUsing(string endPoint, string authKey)
		{
			_connectionSettings = new CosmosDbConnectionSettings(endPoint, authKey);
			return this;
		}


        public CosmosDbClientBuilder ConnectUsing(CosmosDbConnectionSettings connSettings)
        {
            _connectionSettings = connSettings;
            return this;
        }

		/// <summary>
		/// Set up the names, partition key paths, and optional throughput requirements for your CosmosDB containers.
		/// </summary>
		/// <param name="containerConfigBuilder">The Container Config Builder</param>
		/// <returns>The <see cref="CosmosDbClientBuilder"/></returns>
		public CosmosDbClientBuilder ContainerConfig(Action<IContainerConfigBuilder> containerConfigBuilder)
		{
			var builder = new ContainerConfigBuilder();
			containerConfigBuilder(builder);
			_containerDefinitions.AddRange(builder.Build());
			return this;
		}

		public CosmosDbClient Build()
		{
			return new CosmosDbClient(_connectionSettings, _dbconfig, _containerDefinitions);
		}
	}
}