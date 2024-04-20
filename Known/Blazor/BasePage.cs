namespace Known.Blazor;

public class BasePage : BaseComponent
{
    private string orgPageUrl;

    [SupplyParameterFromQuery(Name = "pid")]
    public string PageId { get; set; }

    public string PageName => Language.GetString(Context.Module);

    protected override Task OnInitAsync() => OnInitPageAsync();

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            var baseUrl = Navigation.BaseUri.TrimEnd('/');
            var pageUrl = Navigation.Uri.Replace(baseUrl, "");
            await Context.SetCurrentMenuAsync(Platform, PageId, pageUrl);
            Logger.Info($"TY={GetType().Name},MN={PageName},PID={PageId},PUL={Context.Url},orgPageUrl={orgPageUrl}");
            if (orgPageUrl != Context.Url)
            {
                orgPageUrl = Context.Url;
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
        if (Context.Module == null)
            return;

        var log = new SysLog
        {
            Target = Context.Module.Name,
            Content = Context.Url,
            Type = LogType.Page.ToString()
        };
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