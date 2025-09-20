namespace Known.Pages;

/// <summary>
/// 首页组件类。
/// </summary>
public class IndexPage : BasePage
{
    /// <inheritdoc />
    public override RenderFragment GetPageTitle()
    {
        return GetPageTitle("home", Language.Home);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            //var isChangePwd = await Admin.
            //UI.Alert("欢迎使用 Known！", () =>
            //{

            //});
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var menu = Context.Current;
        menu ??= new MenuInfo { Id = "Index", Url = "/", Plugins = [] };
        builder.Component<PluginPage>().Set(c => c.Menu, menu).Build();
    }
}