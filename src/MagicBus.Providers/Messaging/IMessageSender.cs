using System;
using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;

namespace MagicBus.Providers.Messaging
{
    public interface IMessageSender
    {
        // <summary>
        /// send a message on the service bus
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessage(MessageBase message);


        /// <summary>
        /// send exception details as an ExceptionMessage on the service bus
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="sourceMessage"></param>
        /// <returns></returns>
        Task SendException(Exception ex, MessageBase sourceMessage);

    }
}