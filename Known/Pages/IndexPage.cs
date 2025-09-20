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
            if (Config.System.IsChangePwd)
                ShowUpdatePassword();
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var menu = Context.Current;
        menu ??= new MenuInfo { Id = "Index", Url = "/", Plugins = [] };
        builder.Component<PluginPage>().Set(c => c.Menu, menu).Build();
    }

    private void ShowUpdatePassword()
    {
        var model = new DialogModel
        {
            Title = "修改密码",
            Width = 400,
            //Content = b => b.Component<UpdatePasswordForm>().Build(),
            Closable = false,
            Footer = null
        };
        model.OnClose = model.CloseAsync;
        UI.ShowDialog(model);
    }
}