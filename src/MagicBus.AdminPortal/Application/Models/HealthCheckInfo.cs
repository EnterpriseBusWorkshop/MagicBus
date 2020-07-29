using System.Collections.Generic;
using MagicBus.Messages.Common;

namespace MagicBus.AdminPortal.Application.Models
{
    public class HealthCheckInfo
    {
        public HealthCheckRequest HealthCheckRequest {get; set;}

        public HealthCheckResponseStatus Status { get; set; }
        
        public IList<HealthCheckResponse> HealthCheckResponses { get; set; } = new List<HealthCheckResponse>();
    }
}
