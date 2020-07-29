using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureGems.CosmosDB;
using MagicBus.AdminPortal.Application.Models;
using MagicBus.Messages.Common;
using MediatR;

namespace MagicBus.AdminPortal.Application.Messages
{
	public class GetMessages : IRequest<PagedMessages<ArchivedMessage>>
	{
		public MessageFilters MessageFilters { get; set; }

		public GetMessages(MessageFilters filters)
		{
			this.MessageFilters = filters;
		}
	}

	public class GetMessagesHandler : IRequestHandler<GetMessages, PagedMessages<ArchivedMessage>>
	{
		private readonly ICosmosDbClient _cosmosDbClient;

		public GetMessagesHandler(ICosmosDbClient cosmosDbClient)
		{
			_cosmosDbClient = cosmosDbClient;
		}

		public async Task<PagedMessages<ArchivedMessage>> Handle(GetMessages request, CancellationToken cancellationToken)
		{
			ICosmosDbContainer messagesContainer = await _cosmosDbClient.GetContainer("messages");

			IQueryable<ArchivedMessage> query = messagesContainer.GetByLinq<ArchivedMessage>();

			query = ApplyFilters(query, request);
			query = query.OrderByDescending(m => m.MessageDate);
			int count = await messagesContainer.ResolveCount(query);
			query = ApplyPaging(query, request);
			CosmosDbResponse<IEnumerable<ArchivedMessage>> messages = await messagesContainer.ResolveWithStreamIterator(query);
			
			if (!messages.IsSuccessful)
			{
				throw new ApplicationException($"Error fetching messages: {messages.ErrorMessage}");
			}

			return new PagedMessages<ArchivedMessage>()
			{
				Messages = messages.Result,
				PageIndex = request.MessageFilters.PageIndex,
				PageSize = request.MessageFilters.PageSize,
				TotalRecords = count
			};
		}

		private IQueryable<ArchivedMessage> ApplyPaging(IQueryable<ArchivedMessage> query, GetMessages request)
		{
			query = query
				.Skip(request.MessageFilters.PageIndex * request.MessageFilters.PageSize)
				.Take(request.MessageFilters.PageSize);
			return query;
		}

		private IQueryable<ArchivedMessage> ApplyFilters(IQueryable<ArchivedMessage> query, GetMessages request)
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
