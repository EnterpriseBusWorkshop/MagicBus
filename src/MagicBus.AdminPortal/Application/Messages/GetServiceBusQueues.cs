using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MagicBus.AdminPortal.Application.Messages
{
    public class GetServiceBusQueues : IRequest<IEnumerable<string>>
    {
    }
    
    public class GetServiceBusQueuesHandler : IRequestHandler<GetServiceBusQueues, IEnumerable<string>>
    {
        public Task<IEnumerable<string>> Handle(GetServiceBusQueues request, CancellationToken cancellationToken)
        {
            IEnumerable<string> result = new List<string>() 
            {
                "fulfillment",
                "healthcheck",
                "mappingservice",
                "messagestore",
                "shop"
            };

            return Task.FromResult(result);
        }
    }
}