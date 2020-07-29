using AzureGems.CosmosDB;
using MagicBus.Messages.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicBus.AdminPortal
{
	public static class ServiceExtensions
	{

		public static IServiceCollection AddCosmosPersistence(this IServiceCollection services, IConfiguration config)
		{
			string endPoint = config["CosmosDb:CosmosDbEndpoint"];
			string authKey = config["CosmosDb:CosmosDbAuthKey"];

			services.AddCosmosDb(builder => builder
				.ConnectUsing(new CosmosDbConnectionSettings(endPoint, authKey))
				.UseDatabase("message-store")
				.ContainerConfig(cfg  => cfg.AddContainer<ArchivedMessage>("messages", "/id"))
			);
			return services;
		}
	}
}
