﻿using AntDesign;

namespace Known.Blazor;

/// <summary>
/// Web页面基类，继承组件基类。
/// </summary>
public class BasePage : BaseComponent, IReuseTabsPage
{
    /// <summary>
    /// 取得当前页面菜单信息。
    /// </summary>
    public MenuInfo Menu { get; private set; }

    /// <summary>
    /// 取得页面模块名称。
    /// </summary>
    public string PageName => Language?.GetString(Menu);

    /// <summary>
    /// 获取标签页标题模板。
    /// </summary>
    /// <returns>标签页标题模板。</returns>
    public virtual RenderFragment GetPageTitle()
    {
        return GetPageTitle(Menu?.Icon, PageName);
    }

    /// <summary>
    /// 异步保存表格模型配置信息。
    /// </summary>
    /// <param name="info">表格模型配置信息。</param>
    /// <returns></returns>
    public virtual Task<Result> SaveSettingAsync(AutoPageInfo info)
    {
        if (Menu.Plugins == null)
            Menu.Plugins = [];
        Menu.Plugins.AddPlugin(info);
        return Platform.SaveMenuAsync(Menu);
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Menu = Context.Current;
        await OnInitPageAsync();
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder) => BuildPage(builder);

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Context.Current != null && !Config.IsClient)
                Admin.AddPageLogAsync(Context);
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitPageAsync() => Task.CompletedTask;

    /// <summary>
    /// 构建页面组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildPage(RenderTreeBuilder builder) { }

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
    /// 取得标签Web页面组件模型实例。
    /// </summary>
    protected TabModel Tab { get; } = new TabModel { IsFillHeight = true };

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Tab.Left = b => b.FormTitle(PageName);
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => builder.Tabs(Tab));
    }
}

/// <summary>
/// 步骤Web页面基类，继承Web页面基类。
/// </summary>
public class BaseStepPage : BasePage
{
    /// <summary>
    /// 取得步骤Web页面组件模型实例。
    /// </summary>
    protected StepModel Step { get; } = new();

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => builder.Steps(Step));
    }
}