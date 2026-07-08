using AntDesign;

namespace Known.Blazor;

/// <summary>
/// Web页面基类，继承组件基类。
/// </summary>
[StreamRendering]
public class BasePage : BaseComponent, IReuseTabsPage
{
    private bool isLogged = false;
    private MenuInfo pageMenu;

    /// <summary>
    /// 取得当前页面菜单信息。
    /// </summary>
    public MenuInfo Menu => pageMenu ??= GetPageMenu();

    /// <summary>
    /// 取得页面模块名称。
    /// </summary>
    public string PageName => Language?.GetString(Menu);

    /// <summary>
    /// 取得或设置页面描述信息。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置是否添加页面访问日志，默认为true。
    /// </summary>
    public bool IsAddLog { get; set; } = true;

    /// <summary>
    /// 获取标签页标题模板。
    /// </summary>
    /// <returns>标签页标题模板。</returns>
    public virtual RenderFragment GetPageTitle()
    {
        return GetPageTitle(Menu?.Icon, PageName);
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        pageMenu ??= GetPageMenu();
        await OnInitPageAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        pageMenu ??= GetPageMenu();
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder) => BuildPage(builder);

    /// <inheritdoc />
    protected override Task OnRenderAsync(bool firstRender)
    {
        if (firstRender && !isLogged && IsAddLog && Menu != null && !Config.IsClient && Context.Url != "/")
        {
            isLogged = true;
            Admin.AddPageLogAsync(Context);
        }
        return base.OnRenderAsync(firstRender);
    }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitPageAsync() => Task.CompletedTask;

    /// <summary>
    /// 设置手机端应用页面菜单信息。
    /// </summary>
    /// <typeparam name="T">页面类型。</typeparam>
    /// <returns></returns>
    protected async Task SetAppPageMenuAsync<T>()
    {
        var type = typeof(T);
        var menu = Context.UserMenus.FirstOrDefault(d => d.Id == type.FullName);
        if (menu != null)
            return;

        var info = new MenuInfo { Id = type.FullName };
        var table = MenuHelper.CreateAutoPage(type);
        info.Plugins.AddPlugin(table);

        var user = CurrentUser;
        if (!user.IsSystemAdmin())
        {
            var moduleIds = await Admin.GetUserModuleIdsAsync(user.Id);
            info.SetPluginPermission(moduleIds);
        }
        Context.UserMenus.Add(info);
    }

    /// <summary>
    /// 构建页面组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card kui-empty-page", () =>
        {
            builder.Div("title", PageName);
            builder.Div("description", Description ?? "页面正在开发中......");
        });
    }

    /// <summary>
    /// 获取页面标题内容。
    /// </summary>
    /// <param name="icon">页面图标。</param>
    /// <param name="name">页面名称。</param>
    /// <returns></returns>
    protected RenderFragment GetPageTitle(string icon, string name)
    {
        return this.BuildTree(b => b.IconName(icon, name));
    }

    private MenuInfo GetPageMenu()
    {
        var type = GetType();
        var menu = Context?.GetMenu(type);
        if (menu != null)
            return type == typeof(AutoPage) ? Context?.Current : menu;

        menu = Context?.Current;
        var route = DataHelper.Routes.FirstOrDefault(m => m.PageType == type);
        if (menu?.PageType == type || menu?.Url == route?.Url)
            return menu;

        return route;
    }
}

/// <summary>
/// 泛型Web页面基类，继承Web页面基类。
/// </summary>
/// <typeparam name="TItem">页面对象类型。</typeparam>
public class BasePage<TItem> : BasePage where TItem : class, new()
{
    /// <summary>
    /// 取得Web页面组件模型实例。
    /// </summary>
    protected PageModel Page { get; } = new();

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<WebPage>().Set(c => c.Model, Page).Build();
    }

    /// <summary>
    /// 查看表单。
    /// </summary>
    /// <param name="type">查看类型。</param>
    /// <param name="row">表单数据。</param>
    public virtual void ViewForm(FormViewType type, TItem row) { }
}

/// <summary>
/// 标签Web页面基类，继承Web页面基类。
/// </summary>
public class BaseTabPage : BasePage
{
    /// <summary>
    /// 构造函数，创建一个标签Web页面实例。
    /// </summary>
    public BaseTabPage()
    {
        Tab = new TabModel(this)
        {
            Left = b => b.FormTitle(PageName)
        };
    }

    /// <summary>
    /// 取得标签Web页面组件模型实例。
    /// </summary>
    protected TabModel Tab { get; }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-card kui-tab").AddClass(Class).BuildClass();
        builder.Div(className, () => builder.Tabs(Tab));
    }
}

/// <summary>
/// 步骤Web页面基类，继承Web页面基类。
/// </summary>
public class BaseStepPage : BasePage
{
    /// <summary>
    /// 构造函数，创建一个步骤Web页面实例。
    /// </summary>
    public BaseStepPage()
    {
        Step = new StepModel(this);
    }

    /// <summary>
    /// 取得步骤Web页面组件模型实例。
    /// </summary>
    protected StepModel Step { get; }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-card kui-step").AddClass(Class).BuildClass();
        builder.Div(className, () => builder.Steps(Step));
    }
}