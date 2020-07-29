using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AzureGems.CosmosDB
{
	public static class CosmosDbServicesExtensions
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, Action<CosmosDbClientBuilder> configure = null)
		{
			services.TryAddSingleton<ICosmosDbContainerFactory, CosmosDbContainerFactory>();

			services.AddSingleton<CosmosDbClientBuilder>(provider =>
            {
                CosmosDbClientBuilder clientBuilder = new CosmosDbClientBuilder(services);

                configure?.Invoke(clientBuilder);

                return clientBuilder;
			});

			services.AddSingleton<ICosmosDbClient>(provider => provider.GetRequiredService<CosmosDbClientBuilder>().Build());

			return services;
		}
	}
}