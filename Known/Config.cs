namespace Known;

public sealed class Config
{
    private Config() { }

    public const string SiteUrl = "http://known.pumantech.com";
    public const string GiteeUrl = "https://gitee.com/known/Known";
    public const string GithubUrl = "https://github.com/known/Known";

    public static bool IsClient { get; internal set; }
    public static string HostUrl { get; set; }
    public static string DateFormat { get; set; } = "yyyy-MM-dd";
    public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:ss";
    public static Action OnExit { get; set; }
    public static AppInfo App { get; } = new();
    public static VersionInfo Version { get; private set; }
    public static List<Assembly> Assemblies { get; } = [];
    public static List<Type> ApiTypes { get; } = [];
    public static List<ApiMethodInfo> ApiMethods { get; } = [];
    public static List<MenuInfo> AppMenus { get; set; }
    public static Action<List<SysModule>> OnAddModule { get; set; }
    internal static DateTime StartTime { get; set; }
    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }
    internal static List<Assembly> CoreAssemblies { get; } = [];
    internal static List<ActionInfo> Actions { get; set; } = [];
    internal static Dictionary<string, Type> ImportTypes { get; } = [];
    internal static Dictionary<string, Type> FlowTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];

    public static void AddModule(Assembly assembly, bool isAdditional = true)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        if (isAdditional)
            Assemblies.Add(assembly);
        else
            CoreAssemblies.Add(assembly);
        AddActions(assembly);
        Language.Initialize(assembly);

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsInterface && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(item);
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

    public static string GetUploadPath(string filePath, bool isWeb = false)
    {
        var path = GetUploadPath(isWeb);
        return Path.Combine(path, filePath);
    }

    internal static string GetFileUrl(string filePath, bool isWeb = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;

        var path = filePath.Replace("\\", "/");
        return isWeb ? $"Files/{path}" : $"UploadFiles/{path}";
    }

    internal static void AddApp()
    {
        Version = new VersionInfo(App.Assembly);
        AddModule(typeof(Config).Assembly);
    }

    private static void AddApiMethod(Type type)
    {
        ApiTypes.Add(type);
        //Console.WriteLine($"api/{type.Name}");
        var controler = type.Name[1..].Replace("Service", "");
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (method.IsPublic && method.DeclaringType?.Name == type.Name)
            {
                var info = new ApiMethodInfo();
                var name = method.Name.Replace("Async", "");
                info.Id = $"{type.Name}.{method.Name}";
                info.Route = $"/{controler}/{name}";
                info.HttpMethod = name.StartsWith("Get") ? HttpMethod.Get : HttpMethod.Post;
                info.MethodInfo = method;
                info.Parameters = method.GetParameters();
                ApiMethods.Add(info);
            }
        }
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
    public VersionInfo() { }

    internal VersionInfo(Assembly assembly)
    {
        if (assembly != null)
        {
            var version = assembly.GetName().Version;
            AppVersion = $"{Config.App.Id} V{version.Major}.{version.Minor}";
            SoftVersion = $"V{version.Major}.{version.Minor}.{version.Build}";
        }

        var version1 = typeof(VersionInfo).Assembly.GetName().Version;
        FrameVersion = $"Known V{version1.Major}.{version1.Minor}.{version1.Build}";
    }

    public string AppVersion { get; set; }
    public string SoftVersion { get; set; }
    public string FrameVersion { get; set; }
    public DateTime BuildTime { get; set; }

    internal void LoadBuildTime()
    {
        var dateTime = GetBuildTime();
        var count = dateTime.Year - 2000 + dateTime.Month + dateTime.Day;
        BuildTime = dateTime;
        SoftVersion = $"{SoftVersion}.{count}";
    }

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
}

public enum AppType { Web, WebApi, Desktop }

public class AppInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public AppType Type { get; set; } = AppType.Web;
    public Assembly Assembly { get; set; }
    public bool IsPlatform { get; set; }
    public bool IsSize { get; set; }
    public bool IsLanguage { get; set; }
    public bool IsTheme { get; set; }
    public string WebRoot { get; set; }
    public string ContentRoot { get; set; }
    public string UploadPath { get; set; }
    public long UploadMaxSize { get; set; } = 1024 * 1024 * 50;
    public string DefaultSize { get; set; } = "Default";
    public int DefaultPageSize { get; set; } = 10;
    public string JsPath { get; set; }
    public string ProductId { get; set; }
    public string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    public string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";
    public List<ConnectionInfo> Connections { get; set; }
    public Func<SystemInfo, Result> CheckSystem { get; set; }
    public Action<CommandInfo> DBLog { get; set; }

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

    internal void SetConnection(List<DatabaseInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return;

        foreach (var info in infos)
        {
            var conn = GetConnection(info.Name);
            if (conn != null)
                conn.ConnectionString = info.ConnectionString;
        }

        AppHelper.SaveConnections(Connections);
    }
}

public class ApiMethodInfo
{
    public string Id { get; set; }
    public string Route { get; set; }
    public HttpMethod HttpMethod { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public ParameterInfo[] Parameters { get; set; }
}

public class ClientInfo
{
    public Func<Type, Type> InterceptorType { get; set; }
    public Func<Type, object, object> InterceptorProvider { get; set; }
}

public class ConnectionInfo
{
    public string Name { get; set; }
    public DatabaseType DatabaseType { get; set; }
    public Type ProviderType { get; set; }
    public string ConnectionString { get; set; }

    internal string GetDefaultConnectionString()
    {
        switch (DatabaseType)
        {
            case DatabaseType.Access:
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password=xxx";
            case DatabaseType.SQLite:
                return "Data Source=..\\Sample.db";
            case DatabaseType.SqlServer:
                return "Data Source=localhost;Initial Catalog=Sample;User Id=xxx;Password=xxx;";
            case DatabaseType.Oracle:
                return "Data Source=localhost:1521/orcl;User Id=xxx;Password=xxx;";
            case DatabaseType.MySql:
                return "Data Source=localhost;port=3306;Initial Catalog=Sample;user id=xxx;password=xxx;Charset=utf8;SslMode=none;AllowZeroDateTime=True;";
            case DatabaseType.PgSql:
                return "Host=localhost;Port=5432;Database=Sample;Username=xxx;Password=xxx;";
            default:
                return string.Empty;
        }
    }
}