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
    /// 取得或设置无代码插件数据服务关联数据库委托，用于根据插件获取关联的数据库对象。
    /// </summary>
    public static Func<Database, AutoPageInfo, Task<Database>> OnDatabase { get; set; }

    // 取得路由页面类型，用于权限控制。
    internal static Dictionary<string, Type> RouteTypes { get; } = [];
    internal static Dictionary<string, Type> FormTypes { get; } = [];

    /// <summary>
    /// 添加项目模块程序集，自动解析操作按钮、多语言、自定义组件类、路由、导入类和数据库建表脚本，以及CodeInfo特性的代码表类。
    /// </summary>
    /// <param name="assembly">模块程序集。</param>
    /// <param name="isAdditional">是否路由附加程序集，默认是。</param>
    public static void AddModule(Assembly assembly, bool isAdditional = true)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        if (isAdditional)
            Assemblies.Add(assembly);
        InitHelper.Add(assembly);
    }

    internal static void AddApp(Assembly assembly)
    {
        Version = new VersionInfo(App.Assembly);
        AddModule(assembly);
        AddModule(App.Assembly);
    }
}