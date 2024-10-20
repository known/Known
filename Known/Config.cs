namespace Known;

/// <summary>
/// 框架全局配置类。
/// </summary>
public sealed class Config
{
    private Config() { }

    /// <summary>
    /// 框架官网网址：http://known.org.cn。
    /// </summary>
    public const string SiteUrl = "http://known.org.cn";

    /// <summary>
    /// 框架Gitee项目网址：https://gitee.com/known/Known。
    /// </summary>
    public const string GiteeUrl = "https://gitee.com/known/Known";

    /// <summary>
    /// 框架GitHub项目网址：https://github.com/known/Known"。
    /// </summary>
    public const string GithubUrl = "https://github.com/known/Known";

    /// <summary>
    /// 取得是否是客户端。
    /// </summary>
    public static bool IsClient { get; internal set; }

    /// <summary>
    /// 取得或设置系统主机地址或域名。
    /// </summary>
    public static string HostUrl { get; set; }

    /// <summary>
    /// 取得或设置日期格式，默认：yyyy-MM-dd。
    /// </summary>
    public static string DateFormat { get; set; } = "yyyy-MM-dd";

    /// <summary>
    /// 取得或设置日期时间格式，默认：yyyy-MM-dd HH:mm。
    /// </summary>
    public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm";

    /// <summary>
    /// 取得或设置系统退出动作，适用于桌面程序。
    /// </summary>
    public static Action OnExit { get; set; }

    /// <summary>
    /// 取得系统配置信息。
    /// </summary>
    public static AppInfo App { get; } = new();

    /// <summary>
    /// 取得系统版本信息。
    /// </summary>
    public static VersionInfo Version { get; private set; }

    /// <summary>
    /// 取得系统程序集列表。
    /// </summary>
    public static List<Assembly> Assemblies { get; } = [];

    /// <summary>
    /// 取得数据库表脚本程序集列表。
    /// </summary>
    public static List<Assembly> DbAssemblies { get; } = [];

    /// <summary>
    /// 取得框架自动解析服务接口生成的WebApi类型列表。
    /// </summary>
    public static List<Type> ApiTypes { get; } = [];

    /// <summary>
    /// 取得框架自动解析服务接口生成的WebApi方法信息列表。
    /// </summary>
    public static List<ApiMethodInfo> ApiMethods { get; } = [];

    /// <summary>
    /// 取得或设置系统移动端菜单信息列表。
    /// </summary>
    public static List<MenuInfo> AppMenus { get; set; }

    /// <summary>
    /// 取得或设置依赖注入服务提供者。
    /// </summary>
    public static IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// 取得或设置注入系统模块初始化数据，系统安装时，会调用项目注入的模块信息，自动安装。
    /// </summary>
    public static Action<List<SysModule>> OnAddModule { get; set; }

    /// <summary>
    /// 取得或设置默认系统设置方法委托，用于配置系统默认设置。
    /// </summary>
    public static Action<SettingInfo> OnSetting { get; set; }

    internal static DateTime StartTime { get; set; }
    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }
    internal static List<ActionInfo> Actions { get; set; } = [];
    internal static Dictionary<string, Type> RouteTypes { get; } = [];
    internal static Dictionary<string, Type> ImportTypes { get; } = [];
    internal static Dictionary<string, Type> FlowTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];
    internal static Dictionary<string, Type> FieldTypes { get; } = [];

    /// <summary>
    /// 添加项目模块程序集，自动解析操作按钮、多语言、导入类、工作流类、自定义表单组件类和路由，以及CodeInfo特性的代码表类。
    /// </summary>
    /// <param name="assembly">模块程序集。</param>
    /// <param name="isAdditional">是否附加到路由组件，默认True。</param>
    public static void AddModule(Assembly assembly, bool isAdditional = true)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        if (isAdditional)
            Assemblies.Add(assembly);
        AddActions(assembly);
        Language.Initialize(assembly);

        foreach (var item in assembly.GetTypes())
        {
            var routes = item.GetCustomAttributes<RouteAttribute>();
            if (routes != null && routes.Any())
            {
                foreach (var route in routes)
                {
                    RouteTypes[route.Template] = item;
                }
            }

            if (TypeHelper.IsSubclassOfGeneric(item, typeof(EntityTablePage<>), out var genericArguments))
                AddApiMethod(typeof(IEntityService<>).MakeGenericType(genericArguments), item.Name);
            else if (item.IsInterface && !item.IsGenericTypeDefinition && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(item, item.Name[1..].Replace("Service", ""));
            else if (item.IsAssignableTo(typeof(ImportBase)))
                ImportTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseFlow)))
                FlowTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseForm)))
                FormTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(ICustomField)))
                AddFieldType(item);
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            var attr = item.GetCustomAttributes<CodeInfoAttribute>();
            if (attr != null && attr.Any())
                Cache.AttachCodes(item);
        }
    }

    /// <summary>
    /// 获取依赖注入的对象。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <returns>对象实例。</returns>
    public static T GetScopeService<T>()
    {
        var scope = ServiceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 获取带有版本号的静态文件URL地址（版本号是根据文件修改日期生成）。
    /// </summary>
    /// <param name="url">静态文件URL。</param>
    /// <returns>带版本号的静态文件URL。</returns>
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

    /// <summary>
    /// 获取上传文件夹路径。
    /// </summary>
    /// <param name="isWeb">是否是wwwroot文件。</param>
    /// <returns>文件夹物理路径。</returns>
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

    /// <summary>
    /// 获取一个文件物理路径。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <param name="isWeb">是否是wwwroot文件。</param>
    /// <returns>文件物理路径。</returns>
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

    private static void AddApiMethod(Type type, string apiName)
    {
        ApiTypes.Add(type);
        //Console.WriteLine($"api/{type.Name}");
        var xml = GetAssemblyXml(type.Assembly);
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (method.IsPublic && method.DeclaringType?.Name == type.Name)
            {
                var info = new ApiMethodInfo();
                var name = method.Name.Replace("Async", "");
                info.Id = $"{type.Name}.{method.Name}";
                info.Route = $"/{apiName}/{name}";
                info.Description = GetMethodSummary(xml, method);
                info.HttpMethod = GetHttpMethod(method);
                info.MethodInfo = method;
                info.Parameters = method.GetParameters();
                ApiMethods.Add(info);
            }
        }
    }

    private static HttpMethod GetHttpMethod(MethodInfo method)
    {
        if (method.Name.StartsWith("Get"))
            return HttpMethod.Get;

        return HttpMethod.Post;
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

    private static void AddFieldType(Type item)
    {
        if (item.Name == nameof(ICustomField) || item.Name == nameof(CustomField))
            return;

        FieldTypes[item.Name] = item;
    }

    private static string GetAssemblyXml(Assembly assembly)
    {
        if (IsClient)
            return string.Empty;

        var fileName = assembly.ManifestModule.Name.Replace(".dll", ".xml");
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        return Utils.ReadFile(path);
    }

    private static string GetMethodSummary(string xml, MethodInfo info)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return string.Empty;

        var name = $"{info.DeclaringType.FullName}.{info.Name}";
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var node = doc.SelectSingleNode($"/doc/members/member[@name[starts-with(., 'M:{name}')]]/summary");
        if (node == null)
            return string.Empty;

        return node.InnerText?.Trim('\n').Trim();
    }
}

/// <summary>
/// 系统版本信息类。
/// </summary>
public class VersionInfo
{
    private readonly Assembly assembly;

    /// <summary>
    /// 构造函数，创建一个系统版本信息类的实例。
    /// </summary>
    public VersionInfo() { }

    internal VersionInfo(Assembly assembly)
    {
        this.assembly = assembly;
        if (assembly != null)
        {
            var version = assembly.GetName().Version;
            AppVersion = $"{Config.App.Id} V{version.Major}.{version.Minor}";
            SoftVersion = $"V{version.Major}.{version.Minor}.{version.Build}";
        }

        var version1 = typeof(VersionInfo).Assembly.GetName().Version;
        FrameVersion = $"Known V{version1.Major}.{version1.Minor}.{version1.Build}";
    }

    /// <summary>
    /// 取得或设置系统版本号。
    /// </summary>
    public string AppVersion { get; set; }

    /// <summary>
    /// 取得或设置软件版本号。
    /// </summary>
    public string SoftVersion { get; set; }

    /// <summary>
    /// 取得或设置框架版本号。
    /// </summary>
    public string FrameVersion { get; set; }

    /// <summary>
    /// 取得或设置系统编译时间。
    /// </summary>
    public DateTime BuildTime { get; set; }

    internal void LoadBuildTime()
    {
        var dateTime = GetBuildTime();
        var count = dateTime.Year - 2000 + dateTime.Month + dateTime.Day;
        BuildTime = dateTime;
        SoftVersion = $"{SoftVersion}.{count}";
    }

    private DateTime GetBuildTime()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        var fileName = Directory.GetFiles(path, "*.exe")?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var version = assembly?.GetName().Version;
            //return new DateTime(2000, 1, 1) + TimeSpan.FromDays(version.Revision);
            return new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        }

        var file = new FileInfo(fileName);
        return file.LastWriteTime;
    }
}

/// <summary>
/// 项目类型枚举。
/// </summary>
public enum AppType
{
    /// <summary>
    /// Web项目。
    /// </summary>
    Web,
    /// <summary>
    /// WebApi项目。
    /// </summary>
    WebApi,
    /// <summary>
    /// 桌面项目。
    /// </summary>
    Desktop
}

/// <summary>
/// 系统配置信息类。
/// </summary>
public class AppInfo
{
    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置系统类型，默认Web。
    /// </summary>
    public AppType Type { get; set; } = AppType.Web;

    /// <summary>
    /// 取得或设置系统入口程序集，用于获取软件版本号。
    /// </summary>
    public Assembly Assembly { get; set; }

    /// <summary>
    /// 取得或设置系统是否为多租户平台。
    /// </summary>
    public bool IsPlatform { get; set; }

    /// <summary>
    /// 取得或设置系统是否启用移动端页面。
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// 取得或设置系统主页顶部菜单是否显示字体大小切换。
    /// </summary>
    public bool IsSize { get; set; }

    /// <summary>
    /// 取得或设置系统主页顶部菜单是否显示语言切换。
    /// </summary>
    public bool IsLanguage { get; set; }

    /// <summary>
    /// 取得或设置系统主页顶部菜单是否显示主题切换。
    /// </summary>
    public bool IsTheme { get; set; }

    /// <summary>
    /// 取得或设置系统Web根目录。
    /// </summary>
    public string WebRoot { get; set; }

    /// <summary>
    /// 取得或设置系统内容根目录。
    /// </summary>
    public string ContentRoot { get; set; }

    /// <summary>
    /// 取得或设置系统附件上传位置，默认为根目录上级文件夹内的UploadFiles文件夹。
    /// </summary>
    public string UploadPath { get; set; }

    /// <summary>
    /// 取得或设置系统附件上传最大长度，默认50M。
    /// </summary>
    public long UploadMaxSize { get; set; } = 1024 * 1024 * 50;

    /// <summary>
    /// 取得或设置系统默认字体大小，默认为Default。
    /// </summary>
    public string DefaultSize { get; set; } = "Default";

    /// <summary>
    /// 取得或设置系统表格默认分页大小，默认10。
    /// </summary>
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// 取得或设置系统JS脚本文件路径，该文件中的JS方法，可通过JSService的InvokeAppAsync和InvokeAppVoidAsync调用。
    /// </summary>
    public string JsPath { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的产品ID。
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的版权信息。
    /// </summary>
    public string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";

    /// <summary>
    /// 取得或设置【关于系统】模块显示的软件许可信息。
    /// </summary>
    public string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";

    /// <summary>
    /// 取得或设置系统数据库连接信息列表，注意：Default为框架默认数据库连接名称，不要修改。
    /// </summary>
    public List<ConnectionInfo> Connections { get; set; }

    /// <summary>
    /// 取得或设置系统授权验证方法，如果设置，则页面会先校验系统License，不通过，则显示框架内置的未授权面板。
    /// </summary>
    public Func<SystemInfo, Result> CheckSystem { get; set; }

    /// <summary>
    /// 取得或设置系统数据库SQL监听器委托。
    /// </summary>
    public Action<CommandInfo> SqlMonitor { get; set; }

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


/// <summary>
/// WebApi方法信息类。
/// </summary>
public class ApiMethodInfo
{
    /// <summary>
    /// 取得或设置方法ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置方法路由地址。
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// 取得或设置方法描述。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置方法HTTP请求方式，默认方法名以Get开头的方法为GET请求，其他为POST请求。
    /// </summary>
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// 取得或设置方法信息。
    /// </summary>
    public MethodInfo MethodInfo { get; set; }

    /// <summary>
    /// 取得或设置方法参数集合。
    /// </summary>
    public ParameterInfo[] Parameters { get; set; }
}

/// <summary>
/// 客户端信息类。
/// </summary>
public class ClientInfo
{
    /// <summary>
    /// 取得或设置客户端动态代理请求Api拦截器类型。
    /// </summary>
    public Func<Type, Type> InterceptorType { get; set; }

    /// <summary>
    /// 取得或设置客户端动态代理请求拦截器提供者。
    /// </summary>
    public Func<Type, object, object> InterceptorProvider { get; set; }
}

/// <summary>
/// 数据库连接信息类。
/// </summary>
public class ConnectionInfo
{
    /// <summary>
    /// 取得或设置数据库连接名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置数据库类型。
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 取得或设置数据库访问的ADO.NET提供者类型。
    /// </summary>
    public Type ProviderType { get; set; }

    /// <summary>
    /// 取得或设置数据库连接字符串。
    /// </summary>
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
            case DatabaseType.DM:
                return "Server=localhost;Schema=Sample;DATABASE=Sample;uid=xxx;pwd=xxx;";
            default:
                return string.Empty;
        }
    }
}