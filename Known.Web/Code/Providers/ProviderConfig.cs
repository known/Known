using Known.Serialization;

namespace Known.Providers
{
    public class ProviderConfig
    {
        public static void RegisterProviders()
        {
            Container.Register<IJsonProvider, JsonProvider>();
        }
    }
}