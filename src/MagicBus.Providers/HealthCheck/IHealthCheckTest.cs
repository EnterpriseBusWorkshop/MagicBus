using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;

namespace MagicBus.Providers.HealthCheck
{
    public interface IHealthCheckTest
    {

        string Name { get; set; }

        Task<HealthCheckTestResult> RunTest(CancellationToken ct);


    }
}
