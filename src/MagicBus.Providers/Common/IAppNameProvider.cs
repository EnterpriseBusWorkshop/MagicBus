namespace MagicBus.Providers.Common
{
    public interface IAppNameProvider
    {
        string GetAppName();
    }
    
    public class AppNameProvider : IAppNameProvider
    {
        private readonly string _appName;

        public AppNameProvider(string appName)
        {
            _appName = appName;
        }

        public string GetAppName() => _appName;
    }
}