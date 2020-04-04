using Known.Core.Datas;

namespace Known.Core
{
    sealed class Initializer
    {
        public static void Initialize()
        {
            Container.Register<IPlatformRepository, PlatformRepository>();
        }
    }
}
