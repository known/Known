namespace Known.Blazor;

public class BasePage : BaseComponent
{
    private string pageId;
    internal MenuInfo Menu { get; set; }

    [SupplyParameterFromQuery(Name = "pid")]
    public string PageId { get; set; }

    public string PageName => Language.GetString(Context.Module);

    public virtual Task RefreshAsync() => Task.CompletedTask;

    protected override async Task OnInitAsync()
    {
        InitMenu();
        await base.OnInitAsync();
        await OnInitPageAsync();
        await AddVisitLogAsync();
    }

    protected virtual Task OnInitPageAsync() => Task.CompletedTask;

    protected override async Task OnSetParametersAsync()
    {
        await base.OnSetParametersAsync();
        InitMenu();
        if (!string.IsNullOrWhiteSpace(PageId) && pageId != PageId)
        {
            pageId = PageId;
            await OnPageChangedAsync();
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildPage).Build();
    }

    protected virtual void BuildPage(RenderTreeBuilder builder) { }
    protected virtual Task OnPageChangedAsync() => Task.CompletedTask;

    protected void OnToolClick(ActionInfo info) => OnAction(info, null);
    protected void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    internal void InitMenu()
    {
        if (Context == null || Context.UserMenus == null)
            return;

        var baseUrl = Navigation.BaseUri;
        var url = Navigation.Uri.Replace(baseUrl, "");
        Menu = Context.UserMenus.FirstOrDefault(m => m.Id == PageId || (!string.IsNullOrWhiteSpace(m.Url) && m.Url == $"/{url}"));
        if (Menu == null)
            return;

        Id = Menu.Id;
        Name = Menu.Name;
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