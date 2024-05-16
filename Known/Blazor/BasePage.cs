namespace Known.Blazor;

public class BasePage : BaseComponent
{
    public string PageName => Language.GetString(Context.Module);

    protected override Task OnInitAsync() => OnPageInitAsync();

    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        //TODO:此次执行两次问题
        //Logger.Info($"TY={GetType().Name},MN={PageName},PUL={PageUrl},orgPageUrl={orgPageUrl}");
        await OnPageChangeAsync();
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
var tabs = $('.kui-table > .ant-tabs').length;
var table = tabs ? 60 : 48;
$('.kui-card .ant-tabs-content-holder').css('height', (body-136)+'px');
$('.kui-table .ant-table-body').not('.form-list .ant-table-body').css('height', (body-182-42-table)+'px');");
    }

    protected virtual void BuildPage(RenderTreeBuilder builder) { }
    protected virtual Task OnPageInitAsync() => Task.CompletedTask;
    protected virtual Task OnPageChangeAsync() => Task.CompletedTask;
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