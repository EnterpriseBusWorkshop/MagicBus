using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MagicBus.Fulfilment
{
    public class TestEmailSend
    {

        private readonly SmtpConfig _smtpConfig;

        public TestEmailSend(SmtpConfig smtpConfig)
        {
            _smtpConfig = smtpConfig;
        }


        [FunctionName("TestEmailSend")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var mailMessage = new MailMessage()
            {
                Body = "this is a test message from brendan",
                Subject = "Magic Bus Test Message",
                To = { new MailAddress("brendan@nobadthing.com")},
                From = new MailAddress("brendan@nobadthing.com")
            };

            var client = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password)
            }; 
            await client.SendMailAsync(mailMessage);
            
            return new OkObjectResult("Mail Sent");
        }
    }
}