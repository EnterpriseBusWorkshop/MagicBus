using System;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages.Common;
using MagicBus.Providers.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MagicBus.Providers.MediatrBehaviours
{
    public class HandlerExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {

        private readonly ILogger<TRequest> _logger;
        private readonly IMessageSender _messageSender;


        public HandlerExceptionBehaviour(
            ILogger<TRequest> logger, IMessageSender messageSender)
        {
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var response = await next();
                return response;
            }
            catch (Exception handlerException)
            {
                // log the exception
                _logger.LogError(handlerException, "Exception running handler for message {@Message}", request);

                try
                {
                    await _messageSender.SendException(handlerException, request as MessageBase);
                }
                catch (Exception sendException)
                {
                    _logger.LogCritical(sendException, "Exception attempting to save an error message to the bus! This is very bad!");
                }

                // throw original exception
                throw;
            }
        }

    }
}