using System;
using MediatR;

namespace MagicBus.Messages.Common
{
    public abstract class MessageBase: IRequest 
    {
        protected MessageBase()
        {
            MessageType = this.GetType().AssemblyQualifiedName;
        }

        public string CorrelationId { get; set; }

        public DateTime MessageDate { get; set; } = DateTime.UtcNow;

        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        public string MessageType { get; protected set; }
    }
}