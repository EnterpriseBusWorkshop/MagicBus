using Microsoft.Extensions.DependencyInjection;

namespace MagicBus.Providers.Messaging
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// configure both send and receive
        /// </summary>
        /// <param name="services">Service Collection we're building</param>
        /// <param name="sbConnection">connection details</param>
        /// <param name="appName">app  name string - used when logging messages</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageHandling(this IServiceCollection services, 
            ServiceBusTopicConnectionDetails sbConnection)
        {
            services.AddMessageReader();
            services.AddMessageSender(sbConnection);
            return services;
        }


        /// <summary>
        /// configure reader only 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appName">current application name - used when logging messages</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageReader(this IServiceCollection services)
        {
            services.AddSingleton<IMessageReader>(c =>
                new LoggingMessageReader(new MessageReader())
            );
            return services;
        }


        /// <summary>
        /// configure sender only
        /// </summary>
        /// <param name="services"></param>
        /// <param name="sbConnection"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageSender(this IServiceCollection services, ServiceBusTopicConnectionDetails sbConnection)
        { 
            services.AddSingleton(sbConnection);
            services.AddSingleton<IMessageSender, ServiceBusMessageSender>();
            return services;
        }
    }
}