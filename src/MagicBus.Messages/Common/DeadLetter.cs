using System;
using Newtonsoft.Json;

namespace MagicBus.Messages.Common
{
    /// <summary>
    /// wrap MessageBase with an ID so it can be sent to CosmosDb
    /// </summary>
    public class DeadLetter
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public MessageBase Message { get; set; }

        public DateTime MessageDate { get; set; } = DateTime.UtcNow;

        public string MessageTypeName => this.Message?.GetType()?.Name;

        public string DeadLetterReason { get; set; }

        public string DeadLetterErrorDescription { get; set; }

        public long SequenceNumber { get; set; }

        public string SubscriptionName { get; set; }

        public DeadLetter()
        {
            
        }

        public DeadLetter(MessageBase message)
        {
            this.Message = message;
            this.Id = message.MessageId;
        }

    }
}