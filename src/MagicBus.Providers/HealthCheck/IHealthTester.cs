using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;

namespace MagicBus.Providers.HealthCheck
{
    public interface IHealthTester
    {
        Task<HealthCheckResponse> RunHealthCheck(HealthCheckRequest request);
    }
}
