using Known.Mapping;

namespace Known.Core
{
    /// <summary>
    /// 平台模块初始化者。
    /// </summary>
    public class CoreInitializer : Initializer
    {
        /// <summary>
        /// 初始化模块。
        /// </summary>
        /// <param name="context">程序上下文对象。</param>
        public override void Initialize(AppContext context)
        {
            InitPlatformRepository(context);
            InitCoreModule(context);
        }

        private static void InitPlatformRepository(AppContext context)
        {
            //先判断外部是否已注册依赖
            var repository = Container.Resolve<IPlatformRepository>();
            if (repository != null)
                return;

            //外部没有注册依赖，则根据环境自动注册
            var baseUrl = Setting.ApiPlatformUrl;
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                repository = new ApiPlatformRepository(baseUrl);
                Container.Register<IPlatformRepository>(repository);
            }
            else
            {
                repository = new PlatformRepository(context.Database);
                Container.Register<IPlatformRepository>(repository);
            }
        }

        private static void InitCoreModule(AppContext context)
        {
            var assembly = typeof(CoreInitializer).Assembly;
            EntityHelper.InitMapper(assembly);

            Container.Register<ServiceBase>(assembly, context);
        }
    }
}
