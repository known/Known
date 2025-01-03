﻿namespace Known;

/// <summary>
/// 框架全局配置类。
/// </summary>
public sealed class Config
{
    private Config() { }

    /// <summary>
    /// 框架官网网址：https://known.org.cn。
    /// </summary>
    public const string SiteUrl = "https://known.org.cn";

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
    /// 取得或设置是否是调试模式。
    /// </summary>
    public static bool IsDebug { get; set; }

    /// <summary>
    /// 取得或设置是否是显示开发中心。
    /// </summary>
    public static bool IsDevelopment { get; set; }

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
    /// 取得或设置默认系统设置方法委托，用于配置系统默认设置。
    /// </summary>
    public static Action<UserSettingInfo> OnSetting { get; set; }

    /// <summary>
    /// 取得系统启动时间。
    /// </summary>
    public static DateTime StartTime { get; internal set; }

    /// <summary>
    /// 取得操作按钮信息列表。
    /// </summary>
    public static List<ActionInfo> Actions { get; } = [];

    /// <summary>
    /// 取得自定义扩展字段组件类型字典。
    /// </summary>
    public static Dictionary<string, Type> FieldTypes { get; } = [];

    /// <summary>
    /// 取得框架插件信息列表。
    /// </summary>
    public static List<PluginMenuInfo> Plugins { get; } = [];

    // 取得路由页面类型，用于权限控制。
    internal static Dictionary<string, Type> RouteTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];

    /// <summary>
    /// 添加项目模块程序集，自动解析操作按钮、多语言、自定义组件类、路由、导入类和数据库建表脚本，以及CodeInfo特性的代码表类。
    /// </summary>
    /// <param name="assembly">模块程序集。</param>
    public static void AddModule(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        Assemblies.Add(assembly);
        InitAssembly(assembly);
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

    /// <summary>
    /// 获取文件URL。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <param name="isWeb">是否Web文件，即wwwroot文件。</param>
    /// <returns>文件URL。</returns>
    public static string GetFileUrl(string filePath, bool isWeb = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;

        var path = filePath.Replace("\\", "/");
        return isWeb ? $"Files/{path}" : $"UploadFiles/{path}";
    }

    internal static void AddApp()
    {
        Version = new VersionInfo(App.Assembly);
        InitAssembly(App.Assembly);
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

    private static void InitAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        AddActions(assembly);
        Language.Initialize(assembly);

        foreach (var item in assembly.GetTypes())
        {
            if (TypeHelper.IsSubclassOfGeneric(item, typeof(EntityTablePage<>), out var genericArguments))
                AddApiMethod(typeof(IEntityService<>).MakeGenericType(genericArguments), item.Name);
            else if (item.IsInterface && !item.IsGenericTypeDefinition && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(item, item.Name[1..].Replace("Service", ""));
            else if (item.IsAssignableTo(typeof(ImportBase)))
                ImportHelper.ImportTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseForm)))
                FormTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(ICustomField)))
                AddFieldType(item);
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            var routes = item.GetCustomAttributes<RouteAttribute>();
            if (routes != null && routes.Any())
            {
                foreach (var route in routes)
                {
                    RouteTypes[route.Template] = item;
                }
            }

            var plugin = item.GetCustomAttribute<PluginAttribute>();
            if (plugin != null)
            {
                var info = new PluginMenuInfo(item, plugin);
                info.Url = routes?.FirstOrDefault()?.Template;
                Plugins.Add(info);
            }

            var codeInfo = item.GetCustomAttribute<CodeInfoAttribute>();
            if (codeInfo != null)
                Cache.AttachCodes(item);
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
            if (string.IsNullOrWhiteSpace(item) || item.StartsWith("按钮编码"))
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
            if (values.Length > 4)
                info.Position = values[4].Trim();
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
    //private readonly Assembly assembly;

    /// <summary>
    /// 构造函数，创建一个系统版本信息类的实例。
    /// </summary>
    public VersionInfo() { }

    internal VersionInfo(Assembly assembly)
    {
        //this.assembly = assembly;
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