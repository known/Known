namespace Sample;

public sealed class AppConfig
{
    private AppConfig() { }

    public static void Initialize(bool isPlatform = true)
    {
        var assembly = typeof(AppConfig).Assembly;
        //设置项目ID和名称
        Config.AppId = "KIMS";
        Config.AppName = "Known信息管理系统";
        //设置是否是SasS平台模式
        Config.IsPlatform = isPlatform;
        //注入项目程序集
        //框架关于系统自动获取软件版本号
        //框架模块管理配置自动反射Entity和Model
        Config.SetAppAssembly(assembly);

        //设置默认分页大小
        PagingCriteria.DefaultPageSize = 20;
        //注入项目数据字典类别
        DicCategory.AddCategories<AppDictionary>();
        //附加项目CodeTable特性类字典到缓存
        Cache.AttachCodes(assembly);
    }
}