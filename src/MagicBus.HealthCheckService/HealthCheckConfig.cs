using System.Collections.Generic;

namespace MagicBus.HealthCheckService
{
    public class HealthCheckConfig
    {
        public IEnumerable<string> ExpectedServices { get; set; } = new List<string>();
    }
}