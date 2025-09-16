namespace Known;

public partial class Config
{
    private static RenderType renderMode = RenderType.Server;
    /// <summary>
    /// 取得或设置呈现类型，默认Server。
    /// </summary>
    public static RenderType RenderMode
    {
        get { return renderMode; }
        set
        {
            renderMode = value;
            CurrentMode = value;
        }
    }

    /// <summary>
    /// 取得或设置当前呈现类型，默认Server。
    /// </summary>
    public static RenderType CurrentMode { get; set; } = RenderType.Server;

    /// <summary>
    /// 取得或设置系统移动端菜单信息列表。
    /// </summary>
    public static List<MenuInfo> AppMenus { get; set; } = [];

    /// <summary>
    /// 取得框架初始模块菜单信息列表。
    /// </summary>
    public static List<MenuAttribute> Menus { get; } = [];

    /// <summary>
    /// 取得或设置操作按钮信息列表。
    /// </summary>
    public static List<ActionInfo> Actions { get; set; } = [];

    /// <summary>
    /// 取得或设置获取初始化信息后附加操作委托。
    /// </summary>
    public static Action<InitialInfo> OnInitial { get; set; }

    /// <summary>
    /// 取得或设置获取Admin信息后附加操作委托。
    /// </summary>
    public static Action<AdminInfo> OnAdmin { get; set; }

    /// <summary>
    /// 取得或设置获取用户角色模块ID列表委托。
    /// </summary>
    public static Func<Database, string, Task<List<string>>> OnRoleModule { get; set; }

    /// <summary>
    /// 取得或设置系统安装后附加操作委托。
    /// </summary>
    public static Func<Database, InstallInfo, SystemInfo, Task> OnInstall { get; set; }

    /// <summary>
    /// 取得或设置获取代码表信息列表委托。
    /// </summary>
    public static Func<Database, Task<List<CodeInfo>>> OnCodeTable { get; set; }

    /// <summary>
    /// 取得或设置自动导入数据委托。
    /// </summary>
    public static Func<ImportContext, ImportBase> OnAutoImport { get; set; }

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
        InitHelper.Load(assembly);
    }

    internal static void AddApp(Assembly assembly)
    {
        Version = new VersionInfo(App.Assembly);
        InitHelper.Load(App.Assembly);
        AddModule(assembly);
    }
}