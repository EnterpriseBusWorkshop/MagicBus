using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureGems.CosmosDB;
using MagicBus.AdminPortal.Application.Models;
using MagicBus.Messages.Common;
using MediatR;

namespace MagicBus.AdminPortal.Application.HealthService
{
	public class GetHealthCheckInfo : IRequest<IEnumerable<HealthCheckInfo>>
	{

	}

	public class GetHealthCheckInfoHandler : IRequestHandler<GetHealthCheckInfo, IEnumerable<HealthCheckInfo>>
	{
		private const int _expectHealthResponseInSeconds = 30;
		private readonly ICosmosDbClient _cosmosDbClient;

		public GetHealthCheckInfoHandler(ICosmosDbClient cosmosDbClient)
		{
			_cosmosDbClient = cosmosDbClient;
		}

		public async Task<IEnumerable<HealthCheckInfo>> Handle(GetHealthCheckInfo request, CancellationToken cancellationToken)
		{
			IList<HealthCheckInfo> healthCheckInfoList = new List<HealthCheckInfo>();

			//Get the latest HealthCheck Requests from Cosmos DB
			ICosmosDbContainer messagesContainer = await _cosmosDbClient.GetContainer("messages");
			IQueryable<ArchivedMessage> query = messagesContainer.GetByLinq<ArchivedMessage>();
			query = query.Where(m => m.Message.MessageType == typeof(HealthCheckRequest).AssemblyQualifiedName)
					.OrderByDescending(m => m.MessageDate)
					.Take(12); // last hour

			CosmosDbResponse<IEnumerable<ArchivedMessage>> messages = await messagesContainer.ResolveWithStreamIterator(query);

			if (!messages.IsSuccessful)
			{
				throw new ApplicationException($"Error fetching messages: {messages.ErrorMessage}");
			}

			foreach(var message in messages.Result)
			{
				var heathCheckRequest = message.Message as HealthCheckRequest;
				var healthCheckInfo = new HealthCheckInfo
				{
					HealthCheckRequest = heathCheckRequest
				};

				//Search the HealthCheck Responses using the correlationId
				IEnumerable<ArchivedMessage> healthCheckResponses = await GetHealthCheckResponsesByCorrelationId(message.Message.CorrelationId);
				foreach(var response in healthCheckResponses)
				{
					HealthCheckResponse healthCheckResponse = (HealthCheckResponse) response.Message;
					healthCheckResponse.ResponseStatus = healthCheckResponse.AggregateTestResults();
					MarkDelayedHealthyResponseAsWarning(healthCheckResponse, heathCheckRequest.MessageDate);
					healthCheckInfo.HealthCheckResponses.Add(healthCheckResponse);
				}

				AddErrorsForMissingServices(healthCheckInfo);
				AggregateResult(healthCheckInfo);
				
				healthCheckInfoList.Add(healthCheckInfo);
			}
			

			return healthCheckInfoList;
		}

		private void AggregateResult(HealthCheckInfo healthCheckInfo)
		{
			healthCheckInfo.Status =
				healthCheckInfo.HealthCheckResponses.Any(r => r.ResponseStatus == HealthCheckResponseStatus.Error)
					? HealthCheckResponseStatus.Error
					: healthCheckInfo.HealthCheckResponses.Any(r =>
						r.ResponseStatus == HealthCheckResponseStatus.Warning)
						? HealthCheckResponseStatus.Warning
						: HealthCheckResponseStatus.Success;
		}

		private void MarkDelayedHealthyResponseAsWarning(HealthCheckResponse healthCheckResponse, DateTime healthCheckRequestDate)
		{
			if (healthCheckResponse.ResponseStatus.Equals(HealthCheckResponseStatus.Success)
				&& (healthCheckResponse.MessageDate.Subtract(healthCheckRequestDate).TotalSeconds > _expectHealthResponseInSeconds))
			{
				healthCheckResponse.ResponseStatus = HealthCheckResponseStatus.Warning;
			}
		}

		private void AddErrorsForMissingServices(HealthCheckInfo healthCheckInfo)
		{
			foreach (var appName in healthCheckInfo.HealthCheckRequest.ExpectedServices)
			{
				if (healthCheckInfo.HealthCheckResponses.All(i => i.AppName != appName))
				{
					healthCheckInfo.HealthCheckResponses.Add(new HealthCheckResponse()
					{
						MessageDate = healthCheckInfo.HealthCheckRequest.MessageDate,
						AppName = appName,
						CorrelationId = healthCheckInfo.HealthCheckRequest.CorrelationId,
						Description = "No response",
						ResponseStatus = HealthCheckResponseStatus.Error
					});
				}
			}
		}

		private async Task<IEnumerable<ArchivedMessage>> GetHealthCheckResponsesByCorrelationId(string correlationId)
		{
			ICosmosDbContainer messagesContainer = await _cosmosDbClient.GetContainer("messages");
			IQueryable<ArchivedMessage> queryHealthCheckResponse = messagesContainer.GetByLinq<ArchivedMessage>();
			queryHealthCheckResponse = queryHealthCheckResponse.Where(m =>
				m.Message.MessageType == typeof(HealthCheckResponse).AssemblyQualifiedName
				&& m.Message.CorrelationId == correlationId);

			CosmosDbResponse<IEnumerable<ArchivedMessage>> messages = await messagesContainer.ResolveWithStreamIterator(queryHealthCheckResponse);

			if (!messages.IsSuccessful)
			{
				throw new ApplicationException($"Error fetching messages: {messages.ErrorMessage}");
			}

			return messages.Result;
		}
	}
}
