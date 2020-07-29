using System.Collections.Generic;
using System.Threading.Tasks;
using MagicBus.Messages.Common;

namespace MagicBus.Providers.ServiceBusReader
{
	// TODO: Brendan - Rename to DeadLetterReader
	public interface IServiceBusReader
    {
        Task<IEnumerable<DeadLetter>> ReadDeadLetterMessagesAsync(string serviceBusQueue);

        Task<bool> ResubmitDeadLetterMessageAsync(string messageId, string serviceBusQueue, long sequenceNumber);
    }
}