using System.Net;
using MagicBus.Providers.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MagicBus.Providers.Logging
{
    public static  class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerilog(this IServiceCollection services, 
            string appName,
            string seqUrl = "http://localhost:5341")
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AppName", appName)
                .WriteTo.Seq(seqUrl)
                //.WriteTo.File($@"C:\Temp\{appName}.log")
                .CreateLogger();
            
            services.AddLogging(lb => 
                lb.AddSerilog(Log.Logger, true)
            );
            return services;
        }
    }
}