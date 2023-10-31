namespace Known.Razor;

[Authorize]
public class PageComponent : BaseComponent
{
    private bool isInitialized;

    public PageComponent()
    {
        isInitialized = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await AddVisitLogAsync();
        await InitPageAsync();
        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        if (!Config.IsCheckKey)
        {
            BuildAuthorize(builder);
            return;
        }

        BuildPage(builder);
    }

    protected virtual Task InitPageAsync() => Task.CompletedTask;
    protected virtual void BuildPage(RenderTreeBuilder builder) { }
}