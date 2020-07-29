using System;
using System.Linq;
using MagicBus.HealthCheckService;
using MagicBus.Providers.Common;
using MagicBus.Providers.Logging;
using MagicBus.Providers.Messaging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MagicBus.HealthCheckService
{
    public class Startup : FunctionsStartup
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
            
            builder.Services.AddSingleton<IDateTimeProvider>(new DateTimeProvider());
            builder.Services.AddSingleton<HealthCheckConfig>(new HealthCheckConfig()
            {
                ExpectedServices = Environment.GetEnvironmentVariable("ExpectedServices")
                    ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    ?.ToList()
            });
        }
    }
}