using Known.Serialization;

namespace Known.Providers
{
    /// <summary>
    /// 提供者配置。
    /// </summary>
    public class ProviderConfig
    {
        /// <summary>
        /// 注册所有自定义提供者。
        /// </summary>
        public static void RegisterProviders()
        {
            Container.Register<IJsonProvider, JsonProvider>();
        }
    }
}