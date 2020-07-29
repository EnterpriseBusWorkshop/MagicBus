using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MagicBus.Messages.Common;
using MagicBus.Providers.Messaging;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using IMessageSender = MagicBus.Providers.Messaging.IMessageSender;

namespace MagicBus.Providers.ServiceBusReader
{
	// TODO: Brendan - Rename to DeadLetterReader
   public class ServiceBusReader : IServiceBusReader
	{
		private const int MaxMessageCount = 1000;
		private const double CacheExpirationSeconds = 30;

		private readonly IMessageReceiverFactory _messageReceiverFactory;
		private readonly IMessageReader _messageReader;
		private readonly IMessageSender _messageSender;
		private readonly IMemoryCache _memoryCache;
		private readonly ILogger<ServiceBusReader> _log;

		public ServiceBusReader(
			IMessageReceiverFactory messageReceiverFactory, 
			IMessageReader messageReader, 
			IMessageSender messageSender, 
			IMemoryCache memoryCache, 
			ILogger<ServiceBusReader> log)
		{
			_messageReceiverFactory = messageReceiverFactory;
			_messageReader = messageReader;
			_messageSender = messageSender;
			_memoryCache = memoryCache;
			_log = log;
        }

		public async Task<IEnumerable<DeadLetter>> ReadDeadLetterMessagesAsync(string serviceBusQueue)
		{
			var _deadLetters = await _memoryCache.GetOrCreateAsync(serviceBusQueue, 
				entry => {
						entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheExpirationSeconds);
						return ReadDeadLettersFromServiceBusAsync(serviceBusQueue);
					});
			return _deadLetters;
		}

		private async Task<IEnumerable<DeadLetter>> ReadDeadLettersFromServiceBusAsync(string serviceBusQueue)
		{
			ConcurrentBag<DeadLetter> _deadLetters = new ConcurrentBag<DeadLetter>();

			IMessageReceiver messageReceiver = _messageReceiverFactory.Create(serviceBusQueue, ReceiveMode.PeekLock, true);

			IList<Message> messages = await messageReceiver.PeekAsync(MaxMessageCount);
			await messageReceiver.CloseAsync();

			foreach (Message message in messages)
			{
				// Process the message
				var deadletter = new DeadLetter(_messageReader.ReadMessage(Encoding.UTF8.GetString(message.Body)))
				{
					MessageDate = message.SystemProperties.EnqueuedTimeUtc,
					DeadLetterReason = message.UserProperties["DeadLetterReason"].ToString(),
					DeadLetterErrorDescription = message.UserProperties["DeadLetterErrorDescription"].ToString(),
					SequenceNumber = message.SystemProperties.SequenceNumber,
					SubscriptionName = serviceBusQueue
				};

				_deadLetters.Add(deadletter);
			}

			return _deadLetters;
		}

		public async Task<bool> ResubmitDeadLetterMessageAsync(string messageId, string serviceBusQueue, long sequenceNumber)
		{
			bool IsSuccess = false;
			IMessageReceiver messageReceiver = _messageReceiverFactory.Create(serviceBusQueue, ReceiveMode.PeekLock, true);
			Message deadLetter = await messageReceiver.PeekBySequenceNumberAsync(sequenceNumber);

			if (deadLetter != null)
			{
				var newMessage = _messageReader.ReadMessage(Encoding.UTF8.GetString(deadLetter.Body));
			
				//Check that the messageId matches the one that we want to re-enqueue
				if(newMessage.MessageId.Equals(messageId))
				{
					try
					{
						//Attempt to send the message back onto the queue
						// never re-queue a message with a duplicate id - it will fail to save to message archive
                        newMessage.MessageId = Guid.NewGuid().ToString();
						await _messageSender.SendMessage(newMessage);
					}
					catch(Exception ex)
                    {
                        _log.LogError(ex, "ResubmitDeadLetterMessageAsync Exception");
                        throw new ServiceBusReaderException("ResubmitDeadLetterMessageAsync", ex);
					}
					finally
					{
						//Delete message after resubmitting
						var msg = await messageReceiver.ReceiveAsync();
						await messageReceiver.CompleteAsync(msg.SystemProperties.LockToken);
						IsSuccess = true;
					}
				}
			}

			await messageReceiver.CloseAsync();

			return IsSuccess;
		}
	}
}