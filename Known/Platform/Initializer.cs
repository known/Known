using Known.Data;

namespace Known.Platform
{
    public sealed class Initializer
    {
        public static void Initialize(IPlatformRepository repository = null)
        {
            Container.Register<IJson, JsonProvider>();
            InitPlatformRepository(repository);
        }

        private static void InitPlatformRepository(IPlatformRepository repository)
        {
            if (repository != null)
            {
                Container.Register<IPlatformRepository>(repository);
                return;
            }

            var baseUrl = Setting.Instance.ApiPlatformUrl;
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                repository = new ApiPlatformRepository(baseUrl);
                Container.Register<IPlatformRepository>(repository);
            }
            else
            {
                repository = new PlatformRepository(new Database());
                Container.Register<IPlatformRepository>(repository);
            }
        }
    }
}
