using System;
using System.Reflection;
using AzureGems.CosmosDB;
using MagicBus.Fulfilment;
using MagicBus.Providers.Common;
using MagicBus.Providers.HealthCheck;
using MagicBus.Providers.Logging;
using MagicBus.Providers.MediatrBehaviours;
using MagicBus.Providers.Messaging;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MagicBus.Fulfilment
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
            builder.Services.AddHealthCheck();
            builder.Services.AddMagicBusMediatR();

            builder.Services.AddSingleton(new SmtpConfig()
            {
                Host = Environment.GetEnvironmentVariable("SmtpHost"),
                Port = Int32.Parse(Environment.GetEnvironmentVariable("SmtpPort") ?? "587"),
                UserName = Environment.GetEnvironmentVariable("SmtpUsername"),
                Password = Environment.GetEnvironmentVariable("SmtpPassword")
            });

        }
    }
    
    
    public static class ServiceExtensions
    {
        

        public static IServiceCollection AddHealthCheck(this IServiceCollection services)
        {
            // register tests
            //services.AddTransient<IHealthCheckTest>(c =>
            //    new CosmosDbTest(c.GetRequiredService<ICosmosDbClient>(), "work-orders"));

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