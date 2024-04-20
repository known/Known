namespace Known.Blazor;

public class BasePage : BaseComponent
{
    private string orgPageUrl;

    [SupplyParameterFromQuery(Name = "pid")]
    public string PageId { get; set; }

    public string PageName => Language.GetString(Context.Module);

    protected override Task OnInitAsync() => OnPageInitAsync();

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            //TODO:此次执行三次问题
            var baseUrl = Navigation.BaseUri.TrimEnd('/');
            var pageUrl = Navigation.Uri.Replace(baseUrl, "");
            var isChanged = orgPageUrl == Context.Url;
            if (orgPageUrl != Context.Url)
                orgPageUrl = Context.Url;
            if (isChanged)
            {
                await Context.SetCurrentMenuAsync(Platform, PageId, pageUrl);
                await AddVisitLogAsync();
                await OnPageChangeAsync();
                //Logger.Info($"TY={GetType().Name},MN={PageName},PID={PageId},PUL={pageUrl},orgPageUrl={orgPageUrl}");
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
    protected virtual Task OnPageInitAsync() => Task.CompletedTask;
    protected virtual Task OnPageChangeAsync() => Task.CompletedTask;
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

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
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