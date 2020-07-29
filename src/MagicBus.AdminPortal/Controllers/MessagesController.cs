using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.AdminPortal.Application.Messages;
using MagicBus.AdminPortal.Application.Models;
using MagicBus.Messages.Common;
using Microsoft.AspNetCore.Mvc;

namespace MagicBus.AdminPortal.Controllers
{
	public class MessagesController : ApiController
	{

		[HttpGet("message-types")]
		public async Task<ActionResult<IEnumerable<MessageTypeDto>>> GetMessageTypes(CancellationToken ct)
		{
			return new OkObjectResult(await Mediator.Send(new GetMessageTypes(), ct));
		}
		
		[HttpPost]
		public async Task<ActionResult<PagedMessages<ArchivedMessage>>> GetMessages([FromBody] MessageFilters messageFilters, CancellationToken ct)
		{
			// if no type specified, then continue...
			Type resolvedMessageType = null;
			if (!string.IsNullOrWhiteSpace(messageFilters.TypeName))
			{
				// if type is specified but does not resolve to a domain type, return error.
				resolvedMessageType = Type.GetType(messageFilters.TypeName);
				if (resolvedMessageType == null)
				{
					return UnprocessableEntity($"The requested MessageType '{messageFilters.TypeName}' does not exist in the Domain.");
				}
			}

			messageFilters.MessageType = resolvedMessageType;

			return Ok(await Mediator.Send(new GetMessages(messageFilters), ct));
		}


        [HttpPost("resubmitMessage")]
        public async Task<ActionResult<bool>> ResubmitMessage([FromQuery] string messageId, CancellationToken ct)
        {
            return await Mediator.Send(new ResubmitMessage(messageId), ct);
        }


		[HttpPost("deadletters")]
		public async Task<ActionResult<PagedMessages<DeadLetter>>> GetDeadLetterMessages([FromBody] MessageFilters messageFilters, CancellationToken ct)
		{
			// if no type specified, then continue...
			Type resolvedMessageType = null;
			if (!string.IsNullOrWhiteSpace(messageFilters.TypeName))
			{
				// if type is specified but does not resolve to a domain type, return error.
				resolvedMessageType = Type.GetType(messageFilters.TypeName);
				if (resolvedMessageType == null)
				{
					return UnprocessableEntity($"The requested MessageType '{messageFilters.TypeName}' does not exist in the Domain.");
				}
			}

			messageFilters.MessageType = resolvedMessageType;
			return new OkObjectResult(await Mediator.Send(new GetDeadLetterMessages(messageFilters), ct));
		}
	}
}
