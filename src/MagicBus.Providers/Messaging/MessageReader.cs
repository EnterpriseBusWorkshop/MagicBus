using MagicBus.Messages.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MagicBus.Providers.Messaging
{
	public class MessageReader: IMessageReader
    {
        public MessageBase ReadMessage(string messageBody)
        {
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore
			};

            return JsonConvert.DeserializeObject(messageBody, GetMessageType(messageBody)) as MessageBase;
        }
        public Type GetMessageType(string messageBody)
        {
            JObject j = JObject.Parse(messageBody);
            var typeName = j["MessageType"].ToString();
            return Type.GetType(typeName);
        }
        
        public T ReadMessage<T>(string messageBody) where T : MessageBase
        {
	        return JsonConvert.DeserializeObject<T>(messageBody, new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore
			});
        }
    }
}