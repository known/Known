namespace Known.Razor;

public abstract class BaseComponent : ComponentBase, IDisposable
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

    [Inject] protected HttpClient Http { get; set; }
    [CascadingParameter] protected KRContext Context { get; set; }

    public UIService UI => Context.UI;
    protected UserInfo CurrentUser => Context.CurrentUser;

    private PlatformFactory platform;
    protected PlatformFactory Platform
    {
        get
        {
            Context.Http = Http;
            platform ??= new PlatformFactory(Context);
            return platform;
        }
    }

    protected virtual void Dispose(bool disposing) { }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Refresh() { }
    public EventCallback Callback(Action action) => EventCallback.Factory.Create(this, action);
    protected EventCallback Callback(Action<object> action) => EventCallback.Factory.Create(this, action);
    protected EventCallback<T> Callback<T>(Action<T> action) => EventCallback.Factory.Create(this, action);

    protected void StateChanged() => InvokeAsync(StateHasChanged);
    protected static RenderFragment BuildTree(Action<RenderTreeBuilder> action) => delegate (RenderTreeBuilder builder) { action(builder); };
    protected static RenderFragment<T> BuildTree<T>(Action<RenderTreeBuilder, T> action) => (row) => delegate (RenderTreeBuilder builder) { action(builder, row); };

    protected void BuildDownload(RenderTreeBuilder builder, string fileId)
    {
        builder.Link(Language.Download, Callback(async () =>
        {
            var url = await Platform.File.GetFileUrlAsync(fileId);
            UI.DownloadFile(url.FileName, url.OriginalUrl);
        }));
    }

    internal async Task AddVisitLogAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;

        var log = new SysLog { Target = Name, Content = type.FullName };
        if (KRConfig.UserMenus.Exists(p => p.Code == type.Name))
            log.Type = Constants.LogTypePage;

        if (string.IsNullOrWhiteSpace(log.Type))
            return;

        await Platform.Log.AddLogAsync(log);
    }

    internal void BuildAuthorize(RenderTreeBuilder builder)
    {
        builder.Component<SysActive>()
               .Set(c => c.OnCheck, isCheck =>
               {
                   Context.Check.IsCheckKey = isCheck;
                   StateChanged();
               })
               .Build();
    }
}