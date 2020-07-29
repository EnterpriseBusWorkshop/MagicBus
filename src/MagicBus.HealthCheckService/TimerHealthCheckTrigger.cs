using System;
using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using MagicBus.Providers.Common;
using MagicBus.Providers.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MagicBus.HealthCheckService
{
    public class TimerHealthCheckTrigger
    {
        private readonly IMessageSender _messageSender;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly HealthCheckConfig _healthCheckConfig; 

        public TimerHealthCheckTrigger(IMessageSender messageSender, IDateTimeProvider dateTimeProvider, HealthCheckConfig healthCheckConfig)
        {
            _messageSender = messageSender;
            _dateTimeProvider = dateTimeProvider;
            _healthCheckConfig = healthCheckConfig;
        }

        [FunctionName(nameof(TimerHealthCheckTrigger))]
        public async Task Run([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"HealthCheck Timer trigger function executed at: {_dateTimeProvider.UtcNow}");

            var request = new HealthCheckRequest()
            {
                CorrelationId = Guid.NewGuid().ToString(),
                MessageDate = _dateTimeProvider.UtcNow, 
                ExpectedServices = _healthCheckConfig.ExpectedServices
            };

            await _messageSender.SendMessage(request);
        }
    }
}