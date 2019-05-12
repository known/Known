using Known.Data;

namespace Known.Platform
{
    public sealed class Initializer
    {
        public static void Initialize()
        {
            var baseUrl = Setting.Instance.ApiPlatformUrl;
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                var repository = new ApiPlatformRepository(baseUrl);
                Container.Register<IPlatformRepository>(repository);
            }
            else
            {
                var repository = new PlatformRepository(new Database());
                Container.Register<IPlatformRepository>(repository);
            }
        }
    }
}
