using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.Providers.Messaging;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MagicBus.Fulfilment
{
    public class ServiceBusReceiver
    {
        private readonly IMessageReader _messageReader;
        private readonly IMediator _mediator;

        public ServiceBusReceiver(IMessageReader messageReader, IMediator mediator)
        {
            _messageReader = messageReader;
            _mediator = mediator;
        }

        [FunctionName("ServiceBusReceiver")]
        public async Task RunAsync(
            [ServiceBusTrigger("%ServiceBusTopic%", "%ServiceBusSubscription%", Connection = "ServiceBusConnectionString")]
            string messageBody, 
            ILogger log, CancellationToken ct)
        {
            if (_messageReader.ReadMessage(messageBody) is IRequest message)
            {
                try
                {
                    await _mediator.Send(message, ct);
                }
                // swallow validation exceptions - no point resubmitting an invalid message - and exception has already been sent on the bus.
                catch (ValidationException)
                {
                    log.LogWarning("message failed validation");
                }
            }
        }
    }
}