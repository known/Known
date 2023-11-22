using System.Reflection;
using Known.Razor;
using Microsoft.AspNetCore.Components.Web;

namespace Known;

public sealed class Config
{
    private Config() { }

    public static AppInfo App { get; } = new();
    public static VersionInfo Version { get; private set; }
    internal static List<ActionInfo> Actions { get; set; } = [];
    internal static List<Type> ModelTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];
    internal static Dictionary<string, Type> PageTypes { get; } = [];
    internal static Dictionary<string, List<string>> PageButtons { get; } = [];
    internal static Dictionary<string, List<string>> PageActions { get; } = [];

    public static void AddModule(Assembly assembly)
    {
        AddActions(assembly);

        foreach (var item in assembly.GetTypes())
        {
            if (item.BaseType == typeof(EntityBase) || item.BaseType == typeof(ModelBase))
            {
                ModelTypes.Add(item);
            }
            else if (item.IsAssignableTo(typeof(BasePage)))
            {
                PageTypes[item.Name] = item;
                AddActions(item);
            }
            else if (item.IsAssignableTo(typeof(BaseForm)))
            {
                FormTypes[item.Name] = item;
            }
            else if (item.IsEnum)
            {
                Cache.AttachEnumCodes(item);
            }

            var attr = item.GetCustomAttributes<CodeInfoAttribute>();
            if (attr != null && attr.Any())
                Cache.AttachCodes(item);
        }
    }

    internal static void AddApp()
    {
        Version = new VersionInfo(App.Assembly);
        Database.RegisterProviders(App.Connections);

        AddModule(typeof(Config).Assembly);
        AddModule(App.Assembly);
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
            var path = App.WebRoot ?? App.ContentRoot;
            var filePath = Path.Combine(path, "Files");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return filePath;
        }

        var uploadPath = App.UploadPath;
        if (string.IsNullOrEmpty(uploadPath))
            uploadPath = Path.Combine(App.ContentRoot, "UploadFiles");

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
        return new("首页", "home", PageTypes.GetValueOrDefault("Home"));
    }

    private static void AddActions(Assembly assembly)
    {
        var content = Utils.GetResource(assembly, "actions");
        if (string.IsNullOrWhiteSpace(content))
            return;

        var lines = content.Split(Environment.NewLine);
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

    private static void AddActions(Type item)
    {
        PageButtons[item.Name] = [];
        PageActions[item.Name] = [];
        var methods = item.GetMethods();
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<ActionAttribute>();
            if (attr != null)
            {
                if (method.GetParameters().Length > 0)
                    PageActions[item.Name].Add(method.Name);
                else
                    PageButtons[item.Name].Add(method.Name);
            }
        }
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
    public bool IsDevelopment { get; set; }
    public string WebRoot { get; set; }
    public string ContentRoot { get; set; }
    public string UploadPath { get; set; }
    public long UploadMaxSize { get; set; } = 1024 * 1024 * 50;
    public int DefaultPageSize { get; set; } = 10;
    public string JsPath { get; set; }
    public string ProductId { get; set; }
    public List<ConnectionInfo> Connections { get; set; }
    public InteractiveServerRenderMode InteractiveServer { get; set; } = new(false);

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