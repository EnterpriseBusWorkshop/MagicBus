using System;

namespace MagicBus.Providers.Common
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
    
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
    
    public class TestDateTimeProvider: IDateTimeProvider
    {
        private readonly DateTime _dateTime;

        public TestDateTimeProvider(DateTime dateTime)
        {
            _dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
        }

        public DateTime UtcNow => _dateTime;
    }
}