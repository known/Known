using System.Reflection;
using Known.Extensions;
using Known.Razor;
using Microsoft.AspNetCore.Components.Web;

namespace Known;

public sealed class Config
{
    private Config() { }

    //public static string DateFormat { get; set; } = "yyyy-MM-dd";
    //public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

    public static InteractiveServerRenderMode InteractiveServer { get; } = new(false);

    public static AppInfo App { get; private set; }
    public static VersionInfo Version { get; private set; }
    public static CopyrightInfo Copyright { get; } = new();
    public static string RootPath => AppDomain.CurrentDomain.BaseDirectory;
    public static string WebRoot { get; set; }
    public static string ContentRoot { get; set; }
    internal static List<Type> ModelTypes { get; set; } = [];
    internal static Dictionary<string, Type> PageTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];
    //internal static bool IsCheckKey { get; set; } = true;
    //internal static string AuthStatus { get; set; }
    //public static Action<IMyFlow> ShowMyFlow { get; set; }

    public static void AddModule(Assembly assembly)
    {
        foreach (var item in assembly.GetTypes())
        {
            if (item.BaseType == typeof(EntityBase) || item.BaseType == typeof(ModelBase))
                ModelTypes.Add(item);
            else if (item.IsAssignableTo(typeof(BasePage)))
                PageTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseForm)))
                FormTypes[item.Name] = item;

            var attr = item.GetCustomAttributes<CodeTableAttribute>();
            if (attr != null && attr.Any())
                Cache.AttachCodes(item);
        }
    }

    public static void SetApp(AppInfo app)
    {
        App = app;
        Version = new VersionInfo(app.Assembly);
        Database.RegisterProviders(app.Connections);
        AddModule(app.Assembly);
    }

    //private static string GetSysVersion(Assembly assembly)
    //{
    //    var version = assembly.GetName().Version;
    //    var date = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
    //    return $"{Version}.{date:yyMMdd}";
    //}

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

    internal static MenuItem GetHomeMenu()
    {
        return new("首页", "home", PageTypes.GetValue("Home"));
    }
}

public class VersionInfo
{
    internal VersionInfo(Assembly assembly)
    {
        var version = assembly.GetName().Version;
        AppVersion = $"{Config.App.Id} V{version.Major}.{version.Minor}";
        SoftVersion = version.ToString();

        var version1 = typeof(VersionInfo).Assembly.GetName().Version;
        FrameVersion = $"Known V{version1.Major}.{version1.Minor}.{version1.Build}";
    }

    public string AppVersion { get; }
    public string SoftVersion { get; }
    public string FrameVersion { get; }
}

public enum AppType { Web, WinForm }

public class AppInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public AppType Type { get; set; }
    public Assembly Assembly { get; set; }
    public bool IsPlatform { get; set; }
    public string UploadPath { get; set; }
    public string JsPath { get; set; }
    public List<ConnectionInfo> Connections { get; set; }

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

public class CopyrightInfo
{
    public string ProductId { get; set; }
    public bool IsProductKey { get; set; }
    public bool IsEditCopyright { get; set; } = true;
    public string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    public string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";
}