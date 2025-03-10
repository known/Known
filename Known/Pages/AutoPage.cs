namespace Known.Pages;

/// <summary>
/// 无代码页面组件类。
/// </summary>
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (Context.Current == null)
            UI.Page404(builder, PageId);
        else if (Context.Current.Target == nameof(ModuleType.IFrame))
            builder.IFrame(Context.Current.Url);
        else if (Context.Current.Target == nameof(ModuleType.Page))
            BuildAutoTablePage(builder);
        else if (Context.Current.Type == nameof(MenuType.Page))
            BuildPluginPage(builder);
    }

    private void BuildPluginPage(RenderTreeBuilder builder)
    {
        builder.Component<PluginPage>()
               .Set(c => c.Menu, Context.Current)
               .Set(c => c.Page, this)
               .Build(value => page = value);
    }

    private void BuildAutoTablePage(RenderTreeBuilder builder)
    {
        builder.Component<AutoTablePage>()
               .Set(c => c.PageId, PageId)
               .Build(value => page = value);
    }
}