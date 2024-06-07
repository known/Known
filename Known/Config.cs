namespace Known;

public sealed class Config
{
    private Config() { }

    public const string SiteUrl = "http://known.pumantech.com";
    public const string GiteeUrl = "https://gitee.com/known/Known";
    public const string GithubUrl = "https://github.com/known/Known";

    public static bool IsClient { get; set; }
    public static string HostUrl { get; set; }
    public static string DateFormat { get; set; } = "yyyy-MM-dd";
    public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:ss";
    public static Action OnExit { get; set; }
    public static AppInfo App { get; } = new();
    public static VersionInfo Version { get; private set; }
    public static List<Assembly> Assemblies { get; set; } = [];
    public static List<MenuInfo> AppMenus { get; set; }
    public static SystemInfo System { get; set; }
    internal static InstallInfo Install { get; set; }
    internal static DateTime StartTime { get; set; }
    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }
    internal static List<ActionInfo> Actions { get; set; } = [];
    internal static Dictionary<string, Type> ServiceTypes { get; } = [];
    internal static Dictionary<string, Type> ImportTypes { get; } = [];
    internal static Dictionary<string, Type> FlowTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];
    internal static List<MenuInfo> Menus { get; } = [];

    public static void AddModule(Assembly assembly, bool isAdditional = true)
    {
        if (assembly == null)
            return;

        if (isAdditional && !Assemblies.Exists(a => a.FullName == assembly.FullName))
            Assemblies.Add(assembly);
        AddActions(assembly);

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsInterface || item.IsAbstract)
                continue;

            if (item.IsAssignableTo(typeof(IService)))
                ServiceTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(ImportBase)))
                ImportTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseFlow)))
                FlowTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseForm)))
                FormTypes[item.Name] = item;
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            var attr = item.GetCustomAttributes<CodeInfoAttribute>();
            if (attr != null && attr.Any())
                Cache.AttachCodes(item);
        }
    }

    internal static async void AddApp()
    {
        Version = new VersionInfo(App.Assembly);
        AddModule(typeof(Config).Assembly);
        AddModule(App.Assembly, App.AssemblyAdditional);

        var platform = new PlatformService(new Context());
        Install = await platform.System.GetInstallAsync();
    }

    internal static void SetMenu(MenuInfo info)
    {
        if (!Menus.Exists(m => m.Url == info.Url))
            Menus.Add(info);
    }

    public static string GetUploadPath(bool isWeb = false)
    {
        if (isWeb)
        {
            var path = App.WebRoot ?? App.ContentRoot;
            var filePath = Path.Combine(path, "Files");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return filePath;
        }

        var uploadPath = App.UploadPath;
        if (string.IsNullOrEmpty(uploadPath))
        {
            uploadPath = Path.Combine(App.ContentRoot, "..\\UploadFiles");
            uploadPath = Path.GetFullPath(uploadPath);
        }

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        return uploadPath;
    }

    public static string GetStaticFileUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(App.WebRoot))
            return url;

        var fileName = Path.Combine(App.WebRoot, url);
        if (!File.Exists(fileName))
            return url;

        var fileInfo = new FileInfo(fileName);
        var time = fileInfo.LastWriteTime.ToString("yyMMddHHmmss");
        return $"{url}?v={time}";
    }

    public static string GetUploadPath(string filePath, bool isWeb = false)
    {
        var path = GetUploadPath(isWeb);
        return Path.Combine(path, filePath);
    }

    private static void AddActions(Assembly assembly)
    {
        var content = Utils.GetResource(assembly, "actions");
        if (string.IsNullOrWhiteSpace(content))
            return;

        var lines = content.Split([.. Environment.NewLine]);
        if (lines == null || lines.Length == 0)
            return;

        foreach (var item in lines)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            var values = item.Split('|');
            if (values.Length < 2)
                continue;

            var id = values[0].Trim();
            var info = Actions.FirstOrDefault(i => i.Id == id);
            if (info == null)
            {
                info = new ActionInfo { Id = id };
                Actions.Add(info);
            }
            if (values.Length > 1)
                info.Name = values[1].Trim();
            if (values.Length > 2)
                info.Icon = values[2].Trim();
            if (values.Length > 3)
                info.Style = values[3].Trim();
        }
    }
}

public class VersionInfo
{
    internal VersionInfo(Assembly assembly)
    {
        if (!Config.IsClient)
            BuildTime = GetBuildTime();
        if (assembly != null)
        {
            var version = assembly.GetName().Version;
            AppVersion = $"{Config.App.Id} V{version.Major}.{version.Minor}";
            SoftVersion = GetSoftVersion(version, BuildTime);
        }

        var version1 = typeof(VersionInfo).Assembly.GetName().Version;
        FrameVersion = $"Known V{version1.Major}.{version1.Minor}.{version1.Build}";
    }

    public string AppVersion { get; }
    public string SoftVersion { get; }
    public string FrameVersion { get; }
    public DateTime BuildTime { get; }

    private static DateTime GetBuildTime()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        var fileName = Directory.GetFiles(path, "*.exe").FirstOrDefault();
        var file = new FileInfo(fileName);
        return file.LastWriteTime;
        //var version = assembly.GetName().Version;
        //return new DateTime(2000, 1, 1) + TimeSpan.FromDays(version.Revision);
        //return new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
    }

    private static string GetSoftVersion(Version version, DateTime date)
    {
        var count = date.Year - 2000 + date.Month + date.Day;
        return $"V{version.Major}.{version.Minor}.{version.Build}.{count}";
    }
}

public enum AppType { Web, Desktop }

public class AppInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public AppType Type { get; set; }
    public Assembly Assembly { get; set; }
    public bool AssemblyAdditional { get; set; }
    public bool IsPlatform { get; set; }
    public bool IsLanguage { get; set; }
    public bool IsTheme { get; set; }
    public bool IsDevelopment { get; set; }
    public string WebRoot { get; set; }
    public string ContentRoot { get; set; }
    public string UploadPath { get; set; }
    public long UploadMaxSize { get; set; } = 1024 * 1024 * 50;
    public int DefaultPageSize { get; set; } = 10;
    public string JsPath { get; set; }
    public string ProductId { get; set; }
    public string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    public string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";
    public List<ConnectionInfo> Connections { get; set; }
    public Func<SystemInfo, Result> CheckSystem { get; set; }

    internal Result CheckSystemInfo(SystemInfo info)
    {
        if (CheckSystem == null)
            return Result.Success("");

        var result = CheckSystem.Invoke(info);
        Config.IsAuth = result.IsValid;
        Config.AuthStatus = result.Message;
        return result;
    }

    internal ConnectionInfo GetConnection(string name)
    {
        if (Connections == null || Connections.Count == 0)
            return null;

        return Connections.FirstOrDefault(c => c.Name == name);
    }
}

public class ConnectionInfo
{
    public string Name { get; set; }
    public DatabaseType DatabaseType { get; set; }
    public Type ProviderType { get; set; }
    public string ConnectionString { get; set; }
}