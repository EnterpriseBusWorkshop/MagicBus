using System.Collections.Generic;

namespace MagicBus.Messages.Common
{
    /// <summary>
    /// HealthCheck request message
    /// </summary>
    public class HealthCheckRequest : MessageBase
    {
        // services we're expecting to receive a reply from
        public IEnumerable<string> ExpectedServices { get; set; } = new List<string>();
    }
}