using System;

namespace MagicBus.Providers.ServiceBusReader
{
    public class ServiceBusReaderException : ApplicationException
    {

        public ServiceBusReaderException(string message): base(message) { }

        public ServiceBusReaderException(string message, Exception inner) : base(message, inner) { }
    }
}