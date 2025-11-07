namespace Known.Blazor;

/// <summary>
/// 框架UI全局配置类。
/// </summary>
public class UIConfig
{
    private UIConfig() { }

    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }

    internal static void Set(InitialInfo info)
    {
        if (info.Settings != null)
        {
            info.Settings[nameof(IsAuth)] = IsAuth;
            info.Settings[nameof(AuthStatus)] = AuthStatus;
        }
    }

    internal static void Load(InitialInfo info)
    {
        if (info.Settings != null)
        {
            IsAuth = info.Settings.GetValue<bool>(nameof(IsAuth));
            AuthStatus = info.Settings.GetValue<string>(nameof(AuthStatus));
        }
    }

    internal static void Load(AdminInfo info)
    {
    }

    /// <summary>
    /// 取得或设置菜单栏宽度，默认200。
    /// </summary>
    public static int SideWidth { get; set; } = 200;

    /// <summary>
    /// 取得或设置是否启用编辑模式，临时使用。
    /// </summary>
    public static bool EnableEdit { get; set; }

    /// <summary>
    /// 取得或设置是否启用编辑表格。
    /// </summary>
    public static bool IsEditTable { get; set; }

    /// <summary>
    /// 取得或设置是否启用系统管理高级功能，默认开启。
    /// </summary>
    public static bool IsAdvAdmin { get; set; } = true;

    /// <summary>
    /// 取得或设置表格全局栏位文本超出宽度是否显示省略号，显示则文本不换行，默认不显示。
    /// </summary>
    public static bool IsEllipsisTable { get; set; }

    /// <summary>
    /// 取得或设置用户表单是否显示底部按钮。
    /// </summary>
    public static bool UserFormShowFooter { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的版权信息。
    /// </summary>
    public static string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} {Constants.CompName}。保留所有权利。";

    /// <summary>
    /// 取得或设置【关于系统】模块显示的软件许可信息。
    /// </summary>
    public static string SoftTerms { get; set; } = $"您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从{Constants.CompName}或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";

    /// <summary>
    /// 取得或设置系统字体大小下拉项列表。
    /// </summary>
    public static List<ActionInfo> Sizes { get; set; } = [];

    /// <summary>
    /// 取得或设置页面底部内容组件。
    /// </summary>
    public static RenderFragment Footer { get; set; }

    /// <summary>
    /// 取得系统图标字典。
    /// </summary>
    public static Dictionary<string, List<IconMetaInfo>> Icons { get; } = [];

    /// <summary>
    /// 取得错误页面配置字典。
    /// </summary>
    public static Dictionary<string, ErrorConfigInfo> Errors { get; } = [];

    /// <summary>
    /// 取得或设置顶部导航组件类型。
    /// </summary>
    public static Type TopNavType { get; set; }

    /// <summary>
    /// 取得或设置用户中心用户信息组件类型。
    /// </summary>
    public static Type UserProfileType { get; set; }

    /// <summary>
    /// 取得或设置模块页面组件类型。
    /// </summary>
    public static Type ModulePageType { get; set; }

    /// <summary>
    /// 取得企业信息表单组件自定义标签字典。
    /// </summary>
    public static Dictionary<string, ComponentInfo> CompanyTabs { get; } = [];

    /// <summary>
    /// 取得模块页面组件自定义标签字典。
    /// </summary>
    public static Dictionary<string, ComponentInfo> ModuleTabs { get; } = [];

    /// <summary>
    /// 取得模块表单组件自定义标签字典。
    /// </summary>
    public static Dictionary<string, ComponentInfo> ModuleFormTabs { get; } = [];

    /// <summary>
    /// 取得用户中心页面自定义标签字典。
    /// </summary>
    public static Dictionary<string, ComponentInfo> UserTabs { get; } = [];

    /// <summary>
    /// 取得用户表单组件自定义标签字典。
    /// </summary>
    public static Dictionary<string, ComponentInfo> UserFormTabs { get; } = [];

    /// <summary>
    /// 取得关于系统页面自定义标签字典。
    /// </summary>
    public static Dictionary<string, ComponentInfo> SystemTabs { get; } = [];

    /// <summary>
    /// 取得或设置手机模板内容委托。
    /// </summary>
    public static Action<RenderTreeBuilder, RenderFragment> AppBody { get; set; }

    /// <summary>
    /// 取得或设置管理模板内容委托。
    /// </summary>
    public static Action<RenderTreeBuilder, RenderFragment> AdminBody { get; set; }

    /// <summary>
    /// 取得或设置自定义自动表格页面委托。
    /// </summary>
    public static Action<RenderTreeBuilder, TableModel<Dictionary<string, object>>> AutoTablePage { get; set; }

    /// <summary>
    /// 取得或设置显示导入窗体委托。
    /// </summary>
    public static Action<RenderTreeBuilder, ImportInfo> ImportForm { get; set; }

    /// <summary>
    /// 取得或设置原型模拟数据委托。
    /// </summary>
    public static Func<MenuInfo, PageColumnInfo, object> OnMockData { get; set; }

    /// <summary>
    /// 取得或设置模板页初始化委托。
    /// </summary>
    public static Func<UIContext, bool> OnInitLayout { get; set; }

    /// <summary>
    /// 取得或设置任务设置委托。
    /// </summary>
    public static Action<UIService> OnTaskSetting { get; set; }

    /// <summary>
    /// 取得或设置选择模型字段表单委托。
    /// </summary>
    public static Action<UIService, Action<CodeModelInfo>> OnSelectModel { get; set; }

    /// <summary>
    /// 取得或设置快速添加字段表单委托。
    /// </summary>
    public static Action<UIService, List<string>, Action<List<FieldInfo>>> OnFastAddField { get; set; }

    /// <summary>
    /// 取得忽略URL鉴权的路由列表。
    /// </summary>
    public static List<string> IgnoreRoutes { get; } = [];

    /// <summary>
    /// 取得或设置页面标签颜色委托。
    /// </summary>
    public static Func<string, string> TagColor { get; set; }
}

/// <summary>
/// 图标元数据信息类。
/// </summary>
public class IconMetaInfo
{
    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置图标列表。
    /// </summary>
    public List<string> Icons { get; set; } = [];

    internal bool IsActive { get; set; }
}

/// <summary>
/// 错误页面配置信息类。
/// </summary>
public class ErrorConfigInfo
{
    /// <summary>
    /// 取得或设置错误代码描述文本。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置错误代码自定义模板。
    /// </summary>
    public RenderFragment Template { get; set; }
}