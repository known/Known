namespace Sample.Core;

public sealed class AppCore
{
    private AppCore() { }

    public static void Initialize()
    {
        var assembly = typeof(AppCore).Assembly;
        //注册Imports文件夹下异步导入
        BaseImport.Register(assembly);
        //注册WorkFlows文件夹下流程
        BaseFlow.Register(assembly);
    }

    public static void RegisterServices()
    {
        //注册非WebApi请求服务，适用于单机版程序
        //框架自动解析Client类请求的url，映射到Service层
        
        //注册框架服务
        KCConfig.RegisterServices();
        //注册项目服务
        var assembly = typeof(AppCore).Assembly;
        Container.RegisterType<ServiceBase>(assembly);
    }

    public static void InitDatabase(string path)
    {
        var fileName = "Sample.db";
        if (!File.Exists(path))
        {
            var assembly = typeof(AppCore).Assembly;
            var names = assembly.GetManifestResourceNames();
            var name = names.FirstOrDefault(n => n.Contains(fileName));
            if (string.IsNullOrWhiteSpace(name))
                return;

            Utils.EnsureFile(path);
            using var stream = assembly.GetManifestResourceStream(name);
            using var fs = File.Create(path);
            stream?.CopyTo(fs);
        }
    }
}