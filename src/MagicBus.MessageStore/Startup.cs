using System;
using AzureGems.CosmosDB;
using MagicBus.Messages.Common;
using MagicBus.MessageStore;
using MagicBus.Providers.Common;
using MagicBus.Providers.HealthCheck;
using MagicBus.Providers.Logging;
using MagicBus.Providers.Messaging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MagicBus.MessageStore
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var appName = this.GetType().Namespace;
            builder.Services.AddSerilog(appName);
            builder.Services.AddSingleton<IAppNameProvider>(new AppNameProvider(appName));
            builder.Services.AddMessageHandling(new ServiceBusTopicConnectionDetails()
            {
                TopicName = Environment.GetEnvironmentVariable("ServiceBusTopic"),
                ServiceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString")
            });
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddCosmosPersistence();
            builder.Services.AddHealthCheck();
        }
    }




    public static class ServiceExtensions
    {

        public static IServiceCollection AddCosmosPersistence(this IServiceCollection services)
        {
            services.AddCosmosDb(builder => builder
                .ConnectUsing(new CosmosDbConnectionSettings(
                    Environment.GetEnvironmentVariable("CosmosDbEndpoint"),
                    Environment.GetEnvironmentVariable("CosmosDbAuthKey")
                ))
                .UseDatabase("message-store")
                .ContainerConfig(cfg => cfg.AddContainer<ArchivedMessage>("messages", "/id"))
            );
            return services;
        }


        public static IServiceCollection AddHealthCheck(this IServiceCollection services)
        {
            // register tests
            services.AddTransient<IHealthCheckTest>(c =>
                new CosmosDbTest(c.GetRequiredService<ICosmosDbClient>(), "messages"));
            // register test runner
            services.AddHealthCheckTestRunner(typeof(Startup).Namespace, TimeSpan.FromSeconds(10));

            return services;
        }

    }



}