using System;

namespace MagicBus.Providers.HealthCheck
{
    public class ParallelHealthCheckTesterConfig
    {
        /// <summary>
        /// current application name to return with check responses
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// timeout for running tests
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }
}
