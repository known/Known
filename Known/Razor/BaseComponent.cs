using Known.Entities;
using Microsoft.AspNetCore.Components;

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
    [Inject] public JSService JS { get; set; }
    [Inject] public IUIService UI { get; set; }

    protected bool IsLoaded { get; set; }
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

    public EventCallback Callback(Func<Task> callback) => EventCallback.Factory.Create(this, callback);
    public EventCallback Callback(Action callback) => EventCallback.Factory.Create(this, callback);
    public EventCallback<T> Callback<T>(Action<T> callback) => EventCallback.Factory.Create(this, callback);

    public void StateChanged() => InvokeAsync(StateHasChanged);
    //protected RenderFragment<T> BuildTree<T>(Action<RenderTreeBuilder, T> action) => (row) => delegate (RenderTreeBuilder builder) { action(builder, row); };

    //protected void BuildDownload(RenderTreeBuilder builder, string fileId)
    //{
    //    builder.Link(Language.Download, Callback(async () =>
    //    {
    //        var url = await Platform.File.GetFileUrlAsync(fileId);
    //        UI.DownloadFile(url.FileName, url.OriginalUrl);
    //    }));
    //}

    protected bool HasButton(string buttonId)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        return IsInMenu(Id, buttonId);
    }

    internal async Task AddVisitLogAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;

        var log = new SysLog { Target = Name, Content = type.FullName };
        if (Context.UserMenus != null && Context.UserMenus.Exists(p => p.Code == type.Name))
            log.Type = LogType.Page;

        if (string.IsNullOrWhiteSpace(log.Type))
            return;

        await Platform.System.AddLogAsync(log);
    }

    //internal void BuildAuthorize(RenderTreeBuilder builder)
    //{
    //    builder.Component<SysActive>()
    //           .Set(c => c.OnCheck, isCheck =>
    //           {
    //               Config.IsCheckKey = isCheck;
    //               StateChanged();
    //           })
    //           .Build();
    //}

    private bool IsInMenu(string pageId, string buttonId)
    {
        var menu = Context.UserMenus.FirstOrDefault(m => m.Id == pageId || m.Code == pageId);
        if (menu == null)
            return false;

        var hasButton = false;
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            hasButton = menu.Buttons.Contains(buttonId);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(buttonId);
        return hasButton;
    }
}