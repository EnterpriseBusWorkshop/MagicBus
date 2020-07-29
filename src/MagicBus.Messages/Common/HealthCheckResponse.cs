using System.Collections.Generic;
using System.Linq;

namespace MagicBus.Messages.Common
{
    /// <summary>
    /// healthcheck response message
    /// </summary>
    public class HealthCheckResponse: MessageBase
    {
        public string AppName { get; set; }

        public HealthCheckResponseStatus ResponseStatus { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// can be left empty if there are no additional dependencies to be tested.
        /// </summary>
        public IList<HealthCheckTestResult> TestResults { get; set; } = new List<HealthCheckTestResult>();
        
        public HealthCheckResponseStatus AggregateTestResults()
        {
            return TestResults.Any(t => t.ResponseStatus == HealthCheckResponseStatus.Error)
                ? HealthCheckResponseStatus.Error
                : TestResults.Any(t => t.ResponseStatus == HealthCheckResponseStatus.Warning)
                    ? HealthCheckResponseStatus.Warning
                    : HealthCheckResponseStatus.Success;
        }

    }

    public enum HealthCheckResponseStatus
    {
        Success = 1,
        Warning = 2,
        Error = 3
    }
    
    public class HealthCheckTestResult
    {
        public string Name { get; set; }
        public HealthCheckResponseStatus ResponseStatus  { get; set; }
        public string Message { get; set; }
    }
}