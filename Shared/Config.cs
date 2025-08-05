namespace Known;

/// <summary>
/// 框架全局配置类。
/// </summary>
public partial class Config
{
    private static readonly List<string> InitAssemblies = [];

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
    /// 自动无代码导入ID前缀。
    /// </summary>
    public const string AutoBizIdPrefix = "AutoImport";

    /// <summary>
    /// 取得是否是客户端。
    /// </summary>
    public static bool IsClient { get; internal set; }

    /// <summary>
    /// 取得或设置是否是Admin插件。
    /// </summary>
    public static bool IsAdmin { get; set; }

    /// <summary>
    /// 取得或设置是否启用Admin账号操作日志，默认启用。
    /// </summary>
    public static bool IsAdminLog { get; set; } = true;

    /// <summary>
    /// 取得或设置是否是调试模式。
    /// </summary>
    public static bool IsDebug { get; set; }

    /// <summary>
    /// 取得或设置是否是显示开发中心。
    /// </summary>
    public static bool IsDevelopment { get; set; }

    /// <summary>
    /// 取得或设置系统是否已经安装。
    /// </summary>
    public static bool IsInstalled { get; set; }

    /// <summary>
    /// 取得或设置是否弃用AI模式，右下角显示AI图标按钮。
    /// </summary>
    public static bool IsAIMode { get; set; }

    /// <summary>
    /// 取得或设置模块是否是数据库存储模式，默认否，代码和数据混合模式。
    /// </summary>
    public static bool IsDbMode { get; set; }

    /// <summary>
    /// 取得或设置系统主机地址或域名。
    /// </summary>
    public static string HostUrl { get; set; }

    /// <summary>
    /// 取得或设置日期格式，默认：yyyy-MM-dd。
    /// </summary>
    public static string DateFormat { get; set; } = "yyyy-MM-dd";

    /// <summary>
    /// 取得或设置日期时间格式，默认：yyyy-MM-dd HH:mm:ss。
    /// </summary>
    public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// 取得或设置当前系统数据库类型。
    /// </summary>
    public static DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 取得或设置系统退出动作，适用于桌面程序。
    /// </summary>
    public static Action OnExit { get; set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    public static SystemInfo System { get; set; }

    /// <summary>
    /// 取得系统配置信息。
    /// </summary>
    public static AppInfo App { get; } = new();

    /// <summary>
    /// 取得系统版本信息。
    /// </summary>
    public static VersionInfo Version { get; private set; }

    /// <summary>
    /// 取得系统前端程序集列表。
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
    /// 取得或设置依赖注入服务提供者。
    /// </summary>
    public static IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// 取得系统启动时间。
    /// </summary>
    public static DateTime StartTime { get; internal set; }

    /// <summary>
    /// 取得自定义扩展字段组件类型字典。
    /// </summary>
    public static Dictionary<string, Type> FieldTypes { get; } = [];

    /// <summary>
    /// 取得框架初始模块信息列表。
    /// </summary>
    public static List<ModuleInfo> Modules { get; } = [];

    /// <summary>
    /// 取得或设置系统安装时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task> OnInstallModules { get; set; }

    /// <summary>
    /// 取得或设置系统登录时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task<List<ModuleInfo>>> OnInitialModules { get; set; }

    /// <summary>
    /// 取得后端导入类字典。
    /// </summary>
    public static Dictionary<string, Type> ImportTypes { get; } = [];

    /// <summary>
    /// 取得后端工作流类字典。
    /// </summary>
    public static Dictionary<string, Type> FlowTypes { get; } = [];

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
    /// <param name="paths">文件夹名称集合。</param>
    /// <returns>文件夹物理路径。</returns>
    public static string GetUploadDirectory(params string[] paths)
    {
        var uploadPath = App.UploadPath;
        if (string.IsNullOrEmpty(uploadPath))
        {
            uploadPath = Path.Combine(App.ContentRoot, "..\\UploadFiles");
            uploadPath = Path.GetFullPath(uploadPath);
        }

        if (paths != null && paths.Length > 0)
        {
            var path = Path.Combine(paths);
            uploadPath = Path.Combine(uploadPath, path);
        }

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        return uploadPath;
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

        return GetUploadDirectory();
    }

    /// <summary>
    /// 获取一个文件物理路径。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <param name="isWeb">是否是wwwroot文件。</param>
    /// <returns>文件物理路径。</returns>
    public static string GetUploadPath(string filePath, bool isWeb = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;

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

    private static void AddApiMethod(Type type, string apiName)
    {
        ApiTypes.Add(type);
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
        if (!method.Name.StartsWith("Get"))
            return HttpMethod.Post;

        foreach (var item in method.GetParameters())
        {
            if (item.ParameterType.IsClass && item.ParameterType != typeof(string))
                return HttpMethod.Post;
        }
        return HttpMethod.Get;
    }

    private static void AddCodeInfo(Type item)
    {
        var codeInfo = item.GetCustomAttribute<CodeInfoAttribute>();
        if (codeInfo != null)
            Cache.AttachCodes(item);
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