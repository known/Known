namespace Known;

public sealed class Config
{
    private Config() { }

    static Config()
    {
        Version = new VersionInfo();
        AppId = "KIMS";
        AppName = "Known信息管理系统";
        var version = typeof(Config).Assembly.GetName().Version;
        Version.FrameVersion = $"Known V{version.Major}.{version.Minor}.{version.Build}";
    }

    public static string DateFormat { get; set; } = "yyyy-MM-dd";
    public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
    public static string AppId { get; set; }
    public static string AppName { get; set; }
    public static VersionInfo Version { get; }
    public static bool IsPlatform { get; set; }
    public static bool IsWebApi { get; set; } = true;

    public static InteractiveServerRenderMode InteractiveServer { get; } = new(false);

    public static AppInfo App { get; set; } = new AppInfo();
    public static List<Assembly> Modules { get; } = [];
    public static string RootPath => AppDomain.CurrentDomain.BaseDirectory;
    public static string WebRoot { get; set; }
    public static string ContentRoot { get; set; }

    public static void AddModule(Assembly assembly)
    {
        Modules.Add(assembly);
        Cache.AttachCodes(assembly);
    }

    public static void SetAppVersion(Assembly assembly)
    {
        var version = assembly.GetName().Version;
        Version.AppVersion = $"{AppId} V{version.Major}.{version.Minor}";
        Version.SoftVersion = version.ToString();
    }

    private static string GetSysVersion(Assembly assembly)
    {
        var version = assembly.GetName().Version;
        var date = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        return $"{Version}.{date:yyMMdd}";
    }

    internal static string GetUploadPath(bool isWeb = false)
    {
        if (isWeb)
        {
            var path = WebRoot ?? ContentRoot;
            var filePath = Path.Combine(path, "Files");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return filePath;
        }

        var app = App;
        var uploadPath = app.UploadPath;
        if (string.IsNullOrEmpty(uploadPath))
            uploadPath = Path.Combine(ContentRoot, "UploadFiles");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        return uploadPath;
    }

    internal static string GetUploadPath(string filePath, bool isWeb = false)
    {
        var path = GetUploadPath(isWeb);
        return Path.Combine(path, filePath);
    }

    private static List<Type> modelTypes;
    internal static List<Type> GetModelTypes()
    {
        if (modelTypes != null)
            return modelTypes;

        var types = new List<Type>();
        foreach (var module in Modules)
        {
            types.AddRange(module.GetTypes());
        }

        modelTypes = types.Where(t => t.BaseType == typeof(EntityBase) || t.BaseType == typeof(ModelBase)).ToList();
        return modelTypes;
    }

    internal static bool IsCheckKey { get; set; } = true;
    internal static string AuthStatus { get; set; }
    public static string ProductId { get; set; }
    public static bool IsWeb { get; set; }
    public static bool IsProductKey { get; set; }
    public static bool IsEditCopyright { get; set; } = true;
    public static string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    public static string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";
    public static string AppJsPath { get; set; }
    public static KMenuItem Home { get; set; }
    public static Action<IMyFlow> ShowMyFlow { get; set; }

    internal static Type GetType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        var type = typeof(Config).Assembly.GetType(typeName);
        if (type != null)
            return type;

        if (Home != null && Home.ComType != null)
        {
            type = Home.ComType.Assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        foreach (var module in Modules)
        {
            type = module.GetType(typeName);
            if (type != null)
                return type;
        }

        return null;
    }
}