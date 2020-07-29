using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages.Common;
using MagicBus.Providers.HealthCheck;
using MagicBus.Providers.Messaging;
using MediatR;

namespace MagicBus.Fulfilment.Handlers
{
   public class HealthCheckRequestHandler: AsyncRequestHandler<HealthCheckRequest>
   {
      private readonly IHealthTester _healthTester;
      private readonly IMessageSender _messageSender;

      public HealthCheckRequestHandler(IHealthTester healthTester, IMessageSender messageSender)
      {
         _healthTester = healthTester;
         _messageSender = messageSender;
      }


      protected override async Task Handle(HealthCheckRequest request, CancellationToken cancellationToken)
      {
         var response = await _healthTester.RunHealthCheck(request);
         await _messageSender.SendMessage(response);
      }
   }
}