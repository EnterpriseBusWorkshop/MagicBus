using System;
using System.Reflection;
using AzureGems.CosmosDB;
using AzureGems.Repository.CosmosDb.ServiceExtensions;
using MagicBus.MappingService;
using MagicBus.MappingService.CosmosDb;
using MagicBus.MappingService.Entities;
using MagicBus.MappingService.ProductMapping;
using MagicBus.Providers.Common;
using MagicBus.Providers.HealthCheck;
using MagicBus.Providers.Logging;
using MagicBus.Providers.MediatrBehaviours;
using MagicBus.Providers.Messaging;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MagicBus.MappingService
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
            builder.Services.AddMagicBusMediatR();
            builder.Services.AddSingleton<IProductLookupStore, CosmosProductLookupStore>();

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
                .UseDatabase("magic-bus-mappings")
                .ContainerConfig(cfg => cfg
                    .AddContainer<CloudOrderMapping>("cloud-orders", "/id")
                    .AddContainer<ProductLookups>("product-lookup", partitionKeyPath:"/id")
                )
            );
            services.AddCosmosContext<MappingServiceCosmosContext>();
            
            return services;
        }

        public static IServiceCollection AddHealthCheck(this IServiceCollection services)
        {
            // register tests
            services.AddTransient<IHealthCheckTest>(c =>
                new CosmosDbTest(c.GetRequiredService<ICosmosDbClient>(), "work-orders"));

            // register test runner
            services.AddHealthCheckTestRunner(typeof(Startup).Namespace, TimeSpan.FromSeconds(10));

            return services;
        }


        public static IServiceCollection AddMagicBusMediatR(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // order of behavior is important - make sure exception behaviour handler wraps the validation behaviour   
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(HandlerExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerBehaviour<,>));
            // this registers the validation pipeline behaviour (same for most apps), but the validators for this app are registered elsewhere (may differ between apps)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            return services;
        } 

    }
    
    
}