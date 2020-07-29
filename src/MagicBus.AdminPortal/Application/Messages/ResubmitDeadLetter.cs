using System.Threading;
using System.Threading.Tasks;
using MagicBus.Providers.ServiceBusReader;
using MediatR;

namespace MagicBus.AdminPortal.Application.Messages
{
    public class ResubmitDeadLetter : IRequest<bool>
    {
        public string MessageId { get; set; }
        public long SequenceNumber { get; set; }
        public string SbQueue { get; set; }
 
        public ResubmitDeadLetter(string messageId, long sequenceNumber, string sbQueue)
        {
            this.MessageId = messageId;
            this.SequenceNumber = sequenceNumber;
            this.SbQueue = sbQueue;
        }
    }

    public class ResubmitDeadLetterHandler : IRequestHandler<ResubmitDeadLetter, bool>
    {
        private readonly IServiceBusReader _serviceBusReader;

        public ResubmitDeadLetterHandler(IServiceBusReader serviceBusReader)
        {
            _serviceBusReader = serviceBusReader;
        }

        public async Task<bool> Handle(ResubmitDeadLetter request, CancellationToken cancellationToken)
        {
            bool IsSuccessfull = await _serviceBusReader.ResubmitDeadLetterMessageAsync(request.MessageId, request.SbQueue, request.SequenceNumber);
            return IsSuccessfull;
        }
    }
}