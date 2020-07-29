using System;
using Microsoft.Extensions.DependencyInjection;

namespace MagicBus.Providers.HealthCheck
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddHealthCheckTestRunner(this IServiceCollection services, string appName, TimeSpan timeout)
        {
            services.AddSingleton<ParallelHealthCheckTesterConfig>(new ParallelHealthCheckTesterConfig()
            {
                AppName = appName,
                Timeout = timeout
            });
            services.AddTransient<IHealthTester, ParallelHealthCheckTester>();
            return services;
        }

    }
}
