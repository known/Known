namespace Known;

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
    /// 取得或设置系统是否已经安装。
    /// </summary>
    public static bool IsInstalled { get; set; }

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
    /// 取得或设置系统是否授权，默认true。
    /// </summary>
    public static bool IsAuth { get; set; } = true;

    /// <summary>
    /// 取得或设置系统授权状态提示信息。
    /// </summary>
    public static string AuthStatus { get; set; }

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

    /// <summary>
    /// 取得框架初始模块信息列表。
    /// </summary>
    public static List<ModuleInfo> Modules { get; } = [];

    /// <summary>
    /// 取得框架初始模块菜单信息列表。
    /// </summary>
    public static List<MenuAttribute> Menus { get; } = [];

    /// <summary>
    /// 取得或设置系统安装时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task> OnInstallModules { get; set; }

    /// <summary>
    /// 取得或设置系统登录时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task<List<ModuleInfo>>> OnInitialModules { get; set; }

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

    internal static void AddApp(Assembly assembly)
    {
        // 添加默认一级模块
        if (App.IsModule)
        {
            Modules.AddItem("0", Constants.BaseData, "基础数据", "database", 1);
            Modules.AddItem("0", Constants.System, "系统管理", "setting", 99);
        }

        Version = new VersionInfo(App.Assembly);
        InitAssembly(App.Assembly);
        AddModule(assembly);
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

    private static readonly List<string> InitAssemblies = [];

    private static void InitAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (InitAssemblies.Contains(assembly.FullName))
            return;

        InitAssemblies.Add(assembly.FullName);
        AddActions(assembly);
        Language.Initialize(assembly);

        foreach (var item in assembly.GetTypes())
        {
            if (TypeHelper.IsSubclassOfGeneric(item, typeof(EntityTablePage<>), out var typeArguments))
                AddApiMethod(typeof(IEntityService<>).MakeGenericType(typeArguments), item.Name);
            else if (item.IsInterface && !item.IsGenericTypeDefinition && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(item, item.Name[1..].Replace("Service", ""));
            else if (item.IsAssignableTo(typeof(ICustomField)))
                AddFieldType(item);
            else if (item.IsAssignableTo(typeof(BaseForm)))
                FormTypes[item.Name] = item;
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            var routes = GetRoutes(item);
            AddPlugin(item, routes);
            AddMenu(item, routes);
            AddCodeInfo(item);
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

    private static IEnumerable<RouteAttribute> GetRoutes(Type item)
    {
        var routes = item.GetCustomAttributes<RouteAttribute>();
        if (routes != null && routes.Any())
        {
            foreach (var route in routes)
            {
                RouteTypes[route.Template] = item;
            }
        }
        return routes;
    }

    private static void AddPlugin(Type item, IEnumerable<RouteAttribute> routes)
    {
        var plugin = item.GetCustomAttribute<PluginAttribute>();
        if (plugin != null)
        {
            Plugins.Add(new PluginMenuInfo(item, plugin)
            {
                Url = routes?.FirstOrDefault()?.Template
            });
        }
    }

    private static void AddMenu(Type item, IEnumerable<RouteAttribute> routes)
    {
        var menu = item.GetCustomAttribute<MenuAttribute>();
        if (menu != null)
        {
            menu.Page = item;
            menu.Url = routes?.FirstOrDefault()?.Template;
            Menus.Add(menu);
        }
    }

    private static void AddCodeInfo(Type item)
    {
        var codeInfo = item.GetCustomAttribute<CodeInfoAttribute>();
        if (codeInfo != null)
            Cache.AttachCodes(item);
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