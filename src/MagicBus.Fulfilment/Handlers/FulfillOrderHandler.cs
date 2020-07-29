using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages.Orders;
using MagicBus.Providers.Messaging;
using MediatR;

namespace MagicBus.Fulfilment.Handlers
{
    public class FulfillOrderHandler: AsyncRequestHandler<FulfilOrder>
    {

        private readonly SmtpConfig _smtpConfig;
        private readonly IMessageSender _messageSender;

        public FulfillOrderHandler(SmtpConfig smtpConfig, IMessageSender messageSender)
        {
            _smtpConfig = smtpConfig;
            _messageSender = messageSender;
        }

        protected override async Task Handle(FulfilOrder request, CancellationToken cancellationToken)
        {
            var mailMessage = new MailMessage()
            {
                Body = $@"
{request.Name},

Thank you for your purchase of a premium {request.Sku} cloud. I'm sure you'll agree its a good one!

Please remember us for all your future cloud provision needs.


=======================
BLUE SKY CLOUD PRODUCTS
=======================

Why is the sky blue? 
  'cause we sold all the clouds.

",
                To = { new MailAddress(request.EmailAddress)},
                From = new MailAddress("brendan@nobadthing.com"),
                Subject = "An excellent delivery email with your chosen cloud"
            };
            
            var client = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password)
            }; 
            await client.SendMailAsync(mailMessage);

            await _messageSender.SendMessage(new OrderFulfilled()
            {
                CorrelationId = request.CorrelationId,
                OrderId = request.OrderId
            });

        }
    }
}