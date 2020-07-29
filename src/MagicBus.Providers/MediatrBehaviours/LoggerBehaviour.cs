using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MagicBus.Providers.MediatrBehaviours
{
    public class LoggerBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        
        public LoggerBehaviour(
            ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var message = request as MessageBase;
            if (message != null)
            {
                _logger.LogInformation("Start Handler message {MessageType} {MessageId}", typeof(TRequest).Name,
                    message.MessageId);
            }
            
            var response = await next();

            if (message != null)
            {
                _logger.LogInformation("Finish Handler for message {MessageType} {MessageId}", typeof(TRequest).Name,
                    message.MessageId);
            }
            return response;
        }
    }
}