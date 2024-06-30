namespace Known.Blazor;

public class BasePage : BaseComponent
{
    public string PageName => Language.GetString(Context.Current);

    protected override Task OnInitAsync() => OnPageInitAsync();

    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        //Logger.Info($"TY={GetType().Name},MN={PageName},PUL={Context.Url}");
        await RefreshAsync();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildPage).Build();
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        await JS.RunVoidAsync(@"
var body = $('body').height();
var header = 64;
var tabs = $('.kui-page > .ant-tabs').length ? 55 : 10;
var query = 62;
var toolbar = $('.kui-table > .ant-tabs').length ? 56 : 52;
var tableHead = 42;
var cardHeight = body-header-tabs-62;
var tableHeight = body-header-tabs-query-toolbar-tableHead-50;
$('.kui-card .ant-tabs-content-holder').css('height', cardHeight+'px');
$('.kui-table .ant-table-body').not('.form-list .ant-table-body').css('height', tableHeight+'px');");
    }

    protected virtual void BuildPage(RenderTreeBuilder builder) { }
    protected virtual Task OnPageInitAsync() => Task.CompletedTask;
    public virtual Task RefreshAsync() => Task.CompletedTask;
    internal void OnToolClick(ActionInfo info) => OnAction(info, null);
    internal void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);
}

public class BasePage<TItem> : BasePage where TItem : class, new()
{
    protected PageModel Page { get; } = new();

    internal virtual void ViewForm(FormViewType type, TItem row) { }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<WebPage>().Set(c => c.Model, Page).Build();
    }
}

public class BaseTabPage : BasePage
{
    protected TabModel Tab { get; } = new();
    protected string CurrentTab {  get; set; }

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Tab.Left = b => b.Component<KTitle>().Set(c => c.Text, PageName).Build();
    }

    protected async void OnTabChange(string tab)
    {
        if (CurrentTab == tab)
            return;

        CurrentTab = tab;
        await OnTabChangeAsync();
        await RefreshAsync();
    }

    protected virtual Task OnTabChangeAsync() => Task.CompletedTask;

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