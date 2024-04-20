namespace Known.Blazor;

public class BasePage : BaseComponent
{
    private string orgPageUrl;
    private string pageUrl;

    [SupplyParameterFromQuery(Name = "pid")]
    public string PageId { get; set; }

    public string PageName => Language.GetString(Context.Module);

    protected override Task OnInitAsync() => OnInitPageAsync();

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            var baseUrl = Navigation.BaseUri.TrimEnd('/');
            pageUrl = Navigation.Uri.Replace(baseUrl, "");
            //Logger.Info($"TY={GetType()},DIP={IsDisposing},MN={PageName},PID={PageId},PUL={pageUrl}");
            if (!string.IsNullOrWhiteSpace(pageUrl) && pageUrl != "/" && orgPageUrl != pageUrl)
            {
                //Logger.Info($"{Menu?.Name},orgPageUrl={orgPageUrl},pageUrl={pageUrl}");
                orgPageUrl = pageUrl;
                await AddVisitLogAsync();
                await OnPageChangedAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            if (Error != null)
                await Error.HandleAsync(ex);
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildPage).Build();
    }

    protected virtual void BuildPage(RenderTreeBuilder builder) { }
    protected virtual Task OnInitPageAsync() => Task.CompletedTask;
    protected virtual Task OnPageChangedAsync() => Task.CompletedTask;
    public virtual Task RefreshAsync() => Task.CompletedTask;
    public void OnToolClick(ActionInfo info) => OnAction(info, null);
    public void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    private async Task AddVisitLogAsync()
    {
        if (string.IsNullOrWhiteSpace(pageUrl))
            return;

        var type = GetType();
        var log = new SysLog { Target = Name, Content = pageUrl };
        if (Context.UserMenus != null && Context.UserMenus.Exists(p => p.Url == pageUrl))
            log.Type = LogType.Page.ToString();

        if (string.IsNullOrWhiteSpace(log.Type))
            return;

        await Platform.System.AddLogAsync(log);
    }
}

public class BasePage<TItem> : BasePage where TItem : class, new()
{
    protected PageModel Page { get; } = new();

    internal virtual void ViewForm(FormViewType type, TItem row) { }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<WebPage>().Set(c => c.Model, Page).Build();
    }

    protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);
}

public class BaseTabPage : BasePage
{
    protected TabModel Tab { get; } = new();

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Tab.Left = b => b.Component<KTitle>().Set(c => c.Text, PageName).Build();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => UI.BuildTabs(builder, Tab));
    }
}

public class BaseStepPage : BasePage
{
    protected StepModel Step { get; } = new();

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => UI.BuildSteps(builder, Step));
    }
}