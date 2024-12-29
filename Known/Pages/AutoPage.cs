namespace Known.Pages;

/// <summary>
/// 无代码页面组件类。
/// </summary>
[StreamRendering]
[Route("/page/{*PageRoute}")]
public class AutoPage : BasePage
{
    private IAutoPage page;
    private string pageRoute;
    private string PageId { get; set; }

    /// <summary>
    /// 取得或设置页面路由。
    /// </summary>
    [Parameter] public string PageRoute { get; set; }

    /// <summary>
    /// 异步设置页面参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        if (pageRoute != PageRoute)
        {
            pageRoute = PageRoute;
            PageId = Context.Current?.Id;
            if (page != null)
            {
                await page.InitializeAsync();
                await Admin.AddPageLogAsync(Context);
            }
        }
    }

    /// <summary>
    /// 页面呈现后异步方法。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (Context.Current == null)
        {
            UI.Page404(builder, PageId);
            return;
        }

        if (Context.Current.Target == nameof(ModuleType.IFrame))
        {
            builder.IFrame(Context.Current.Url);
            return;
        }

        if (Context.Current.Type == nameof(MenuType.Page))
        {
            BuildPluginPage(builder);
            return;
        }

        if (Context.Current.Target == nameof(ModuleType.Page))
        {
            BuildAutoTablePage(builder);
        }
    }

    private void BuildPluginPage(RenderTreeBuilder builder)
    {
        builder.Component<PluginPage>()
               .Set(c => c.Menu, Context.Current)
               .Build(value => page = value);
    }

    private void BuildAutoTablePage(RenderTreeBuilder builder)
    {
        builder.Component<AutoTablePage>()
               .Set(c => c.PageId, PageId)
               .Build(value => page = value);
    }
}