using Known.Providers;
using Known.Serialization;

namespace Known.Platform.WebApi
{
    public class AppInitializer
    {
        public static void Initialize(Context context)
        {
            Container.Register<IJsonProvider, JsonProvider>();
            Container.Register<ServiceBase>(typeof(Module).Assembly, context);
        }
    }
}