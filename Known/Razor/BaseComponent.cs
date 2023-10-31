namespace Known.Razor;

public abstract class BaseComponent : ComponentBase, IAsyncDisposable
{
    private readonly Type type;

    public BaseComponent()
    {
        type = GetType();
        Id = type.Name;
    }

    [Parameter] public string Id { get; set; }
    [Parameter] public string Name { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;

    [CascadingParameter] protected Context Context { get; set; }
    [Inject] public UIService UI { get; set; }

    protected UserInfo CurrentUser => Context?.CurrentUser;

    private PlatformService platform;
    protected PlatformService Platform
    {
        get
        {
            platform ??= new PlatformService(CurrentUser);
            return platform;
        }
    }


    protected virtual ValueTask DisposeAsync(bool disposing) => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Refresh() { }
    public EventCallback Callback(Func<Task> callback) => EventCallback.Factory.Create(this, callback);
    public EventCallback Callback(Action callback) => EventCallback.Factory.Create(this, callback);
    protected EventCallback Callback(Action<object> callback) => EventCallback.Factory.Create(this, callback);
    protected EventCallback<T> Callback<T>(Action<T> callback) => EventCallback.Factory.Create(this, callback);

    protected void StateChanged() => InvokeAsync(StateHasChanged);
    protected static RenderFragment<T> BuildTree<T>(Action<RenderTreeBuilder, T> action) => (row) => delegate (RenderTreeBuilder builder) { action(builder, row); };

    protected void BuildDownload(RenderTreeBuilder builder, string fileId)
    {
        builder.Link(Language.Download, Callback(async () =>
        {
            var url = await Platform.File.GetFileUrlAsync(fileId);
            UI.DownloadFile(url.FileName, url.OriginalUrl);
        }));
    }

    protected bool HasButton(ButtonInfo button)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        return IsInMenu(button, Id);
    }

    internal async Task AddVisitLogAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;

        var log = new SysLog { Target = Name, Content = type.FullName };
        if (Context.UserMenus.Exists(p => p.Code == type.Name))
            log.Type = Constants.LogTypePage;

        if (string.IsNullOrWhiteSpace(log.Type))
            return;

        await Platform.System.AddLogAsync(log);
    }

    internal void BuildAuthorize(RenderTreeBuilder builder)
    {
        builder.Component<SysActive>()
               .Set(c => c.OnCheck, isCheck =>
               {
                   Config.IsCheckKey = isCheck;
                   StateChanged();
               })
               .Build();
    }

    internal bool IsInMenu(ButtonInfo button, string id)
    {
        var menu = Context.UserMenus.FirstOrDefault(m => m.Id == id || m.Code == id);
        if (menu == null)
            return false;

        var hasButton = false;
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            hasButton = menu.Buttons.Contains(button.Id) || menu.Buttons.Contains(button.Name);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(button.Id) || menu.Actions.Contains(button.Name);
        return hasButton;
    }
}