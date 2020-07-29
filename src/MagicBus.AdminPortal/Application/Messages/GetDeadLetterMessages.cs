using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.AdminPortal.Application.Models;
using MagicBus.Messages.Common;
using MagicBus.Providers.ServiceBusReader;
using MediatR;

namespace MagicBus.AdminPortal.Application.Messages
{
    public class GetDeadLetterMessages : IRequest<PagedMessages<DeadLetter>>
    {
        public MessageFilters MessageFilters { get; set; }

        public GetDeadLetterMessages(MessageFilters filters)
        {
            this.MessageFilters = filters;
        }
    }

    public class GetDeadLetterMessagesHandler : IRequestHandler<GetDeadLetterMessages, PagedMessages<DeadLetter>>
    {
        private readonly IServiceBusReader _serviceBusReader;

        public GetDeadLetterMessagesHandler(IServiceBusReader serviceBusReader)
        {
            _serviceBusReader = serviceBusReader;
        }

        public async Task<PagedMessages<DeadLetter>> Handle(GetDeadLetterMessages request, CancellationToken cancellationToken)
        {
            IEnumerable<DeadLetter> messages = await _serviceBusReader.ReadDeadLetterMessagesAsync(request.MessageFilters.SbQueue);

            messages = ApplyFilters(messages, request);
            messages = messages.OrderByDescending(m => m.MessageDate);
            int count = messages.Count();
            messages = ApplyPaging(messages, request);

            return new PagedMessages<DeadLetter>()
            {
                Messages = messages,
                PageIndex = request.MessageFilters.PageIndex,
                PageSize = request.MessageFilters.PageSize,
                TotalRecords = count
            };
        }

        private IEnumerable<DeadLetter> ApplyPaging(IEnumerable<DeadLetter> query, GetDeadLetterMessages request)
        {
            query = query
                .Skip(request.MessageFilters.PageIndex * request.MessageFilters.PageSize)
                .Take(request.MessageFilters.PageSize);
            return query;
        }

        private IEnumerable<DeadLetter> ApplyFilters(IEnumerable<DeadLetter> query, GetDeadLetterMessages request)
        {
            if (request.MessageFilters.MessageType != null)
            {
                string messageType = request.MessageFilters.MessageType.AssemblyQualifiedName;
                query = query.Where(m => m.Message.MessageType == messageType);
            }
            if (request.MessageFilters.DateFrom != null)
            {
                query = query.Where(m => m.MessageDate >= request.MessageFilters.DateFrom);
            }
            if (request.MessageFilters.DateTo != null)
            {
                query = query.Where(m => m.MessageDate <= request.MessageFilters.DateTo);
            }
            if (!string.IsNullOrWhiteSpace(request.MessageFilters.MessageId))
            {
                query = query.Where(m => m.Message.MessageId == request.MessageFilters.MessageId);
            }
            if (!string.IsNullOrWhiteSpace(request.MessageFilters.CorrelationId))
            {
                query = query.Where(m => m.Message.CorrelationId == request.MessageFilters.CorrelationId);
            }
            return query;
        }
    }
}