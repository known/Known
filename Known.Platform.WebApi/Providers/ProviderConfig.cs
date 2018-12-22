using Known.Serialization;

namespace Known.Providers
{
    public class ProviderConfig
    {
        public static void Register()
        {
            Container.Register<IJsonProvider, JsonProvider>();
        }
    }
}