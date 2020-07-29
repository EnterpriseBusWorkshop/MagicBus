using System.Threading.Tasks;
using MagicBus.Deploy;
using Pulumi;

class Program
{
    static Task<int> Main() => Deployment.RunAsync<MyStack>();
}
