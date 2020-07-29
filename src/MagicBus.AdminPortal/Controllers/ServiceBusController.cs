using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.AdminPortal.Application.Messages;
using Microsoft.AspNetCore.Mvc;

namespace MagicBus.AdminPortal.Controllers
{
	public class ServiceBusController : ApiController
	{
		[HttpGet("servicebus-queues")]
		public async Task<ActionResult<IEnumerable<string>>> GetServiceBusQueues(CancellationToken ct)
		{
			return new OkObjectResult(await Mediator.Send(new GetServiceBusQueues(), ct));
		}

		[HttpPost("resubmitDeadLetter")]
		public async Task<ActionResult<bool>> ResubmitDeadLetter([FromQuery] string messageId, [FromQuery] long sequenceNumber, [FromQuery] string sbQueue, CancellationToken ct)
		{
			return await Mediator.Send(new ResubmitDeadLetter(messageId, sequenceNumber, sbQueue), ct);
		}
	}
}