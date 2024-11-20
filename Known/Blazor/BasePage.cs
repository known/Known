namespace Known.Blazor;

/// <summary>
/// Web页面基类，继承组件基类。
/// </summary>
public class BasePage : BaseComponent
{
    /// <summary>
    /// 取得页面模块名称。
    /// </summary>
    public string PageName => Language.GetString(Context.Current);

    /// <summary>
    /// 异步初始化页面组件。
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitAsync() => OnPageInitAsync();

    /// <summary>
    /// 呈现页面组件内容，如果系统启用授权功能，则会先判断授权，再呈现页面。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildPage).Build();
    }

    /// <summary>
    /// 页面呈现后异步方法，此处会调用页面内容高度自适应计算脚本。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (Context.Current != null && !Config.IsClient)
            {
                await Platform.AddLogAsync(new LogInfo
                {
                    Type = LogType.Page,
                    Target = Context.Current.Name,
                    Content = Context.Url
                });
            }
        }
        await JSRuntime.FillPageHeightAsync();
    }

    /// <summary>
    /// 异步初始化页面虚方法，子页面应覆写该方法。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnPageInitAsync() => Task.CompletedTask;

    /// <summary>
    /// 构建页面组件虚方法，子页面应覆写该方法。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildPage(RenderTreeBuilder builder) { }
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

    /// <summary>
    /// 构建Web页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<WebPage>().Set(c => c.Model, Page).Build();
    }

    /// <summary>
    /// 查看表单虚方法。
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
    protected TabModel Tab { get; } = new();

    /// <summary>
    /// 异步初始化标签Web页面组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Tab.Left = b => b.FormTitle(PageName);
    }

    /// <summary>
    /// 构建标签Web页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
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

    /// <summary>
    /// 构建步骤Web页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => builder.Steps(Step));
    }
}