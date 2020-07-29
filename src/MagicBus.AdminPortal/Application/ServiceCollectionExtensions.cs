using System.Reflection;
using MagicBus.Providers.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MagicBus.AdminPortal.Application
{
	public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            return services;
        }

    }
}
