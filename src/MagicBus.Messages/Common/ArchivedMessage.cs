using System;
using Newtonsoft.Json;

namespace MagicBus.Messages.Common
{
    /// <summary>
    /// wrap MessageBase with an ID so it can be sent to CosmosDb
    /// </summary>
    public class ArchivedMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public MessageBase Message { get; set; }

        public DateTime MessageDate { get; set; } = DateTime.UtcNow;

        public string MessageTypeName => this.Message?.GetType()?.Name;

        public ArchivedMessage()
        {
            
        }

        public ArchivedMessage(MessageBase message)
        {
            this.Message = message;
            this.Id = message.MessageId;
        }

    }

}