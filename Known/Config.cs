using System.Reflection;
using Known.Extensions;
using Known.Razor;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Web;

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
    public static string RootPath => AppDomain.CurrentDomain.BaseDirectory;
    public static string WebRoot { get; set; }
    public static string ContentRoot { get; set; }

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

    public static void SetAppVersion(Assembly assembly)
    {
        var version = assembly.GetName().Version;
        Version.AppVersion = $"{AppId} V{version.Major}.{version.Minor}";
        Version.SoftVersion = version.ToString();
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

    internal static List<Type> ModelTypes { get; set; } = [];
    internal static Dictionary<string, Type> PageTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];
    internal static bool IsCheckKey { get; set; } = true;
    internal static string AuthStatus { get; set; }
    public static string ProductId { get; set; }
    public static bool IsWeb { get; set; }
    public static bool IsProductKey { get; set; }
    public static bool IsEditCopyright { get; set; } = true;
    public static string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    public static string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";
    public static string AppJsPath { get; set; }
    public static Action<IMyFlow> ShowMyFlow { get; set; }

    internal static MenuItem GetHomeMenu()
    {
        return new("首页", "home", PageTypes.GetValue("Home"));
    }
}

public class VersionInfo
{
    public string AppVersion { get; internal set; }
    public string SoftVersion { get; internal set; }
    public string FrameVersion { get; internal set; }
}

public class AppInfo
{
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string UploadPath { get; set; }
    public string Templates { get; set; }
    public string ExportTemplate { get; set; }
    public string ExportPath { get; set; }
    public Dictionary<string, object> Params { get; set; }
    public List<ConnectionInfo> Connections { get; set; }

    public ConnectionInfo GetConnection(string name)
    {
        if (Connections == null || Connections.Count == 0)
            return null;

        return Connections.FirstOrDefault(c => c.Name == name);
    }

    public T Param<T>(string key, T defaultValue = default)
    {
        if (Params == null || Params.Count == 0)
            return defaultValue;

        if (!Params.ContainsKey(key))
            return defaultValue;

        var value = Params[key];
        if (typeof(T).IsClass)
            return Utils.MapTo<T>(value);

        return Utils.ConvertTo(Params[key], defaultValue);
    }
}

public class ConnectionInfo
{
    public string Name { get; set; }
    public string ProviderName { get; set; }
    public string ProviderType { get; set; }
    public string ConnectionString { get; set; }
}