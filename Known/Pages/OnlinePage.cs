namespace Known.Pages;

/// <summary>
/// 在线用户开发插件页面组件类。
/// </summary>
[Route("/dev/online")]
[DevPlugin("在线用户", "user-add", Sort = 96)]
public class OnlinePage : BaseTablePage<UserInfo>
{
    private IOnlineService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IOnlineService>();

        Table.Name = PageName;
        Table.EnableEdit = false;
        Table.EnableFilter = false;
        Table.EnableSort = false;
        Table.ShowSetting = false;
        Table.ShowPager = true;
        Table.OnQuery = Service.QueryOnlineUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name).Width(100);
        Table.AddColumn(c => c.Status).Width(80).Tag();
        Table.AddColumn(c => c.StartTime).Width(140);
        Table.AddColumn(c => c.LastTime).Width(140);
        Table.AddColumn(c => c.Duration).Width(100);
        Table.AddColumn(c => c.LastPage).Width(140);
        Table.AddColumn(c => c.IPAddress).Width(140);
        Table.AddColumn(c => c.IPLocal).Width(140);
        Table.AddColumn(c => c.OSName).Width(120);
        Table.AddColumn(c => c.Device).Width(100);
        Table.AddColumn(c => c.Browser).Width(120);
        Table.AddColumn(c => c.Agent).Width(300).Ellipsis(true);
        Table.AddColumn(c => c.SessionId).Width(260);
        Table.AddColumn(c => c.ClientId).Width(280);

        Table.Toolbar.AddAction(nameof(Refresh));
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var invoker = DotNetObjectReference.Create(this);
            await JSRuntime.RegisterNotifyAsync(invoker, Constants.KeyOnline, nameof(RefreshUser));
        }
    }

    /// <inheritdoc />
    protected override async Task OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await JSRuntime.CloseNotifyAsync(Constants.KeyOnline);
    }

    /// <summary>
    /// 刷新用户在线信息。
    /// </summary>
    [JSInvokable]
    public void RefreshUser(string message)
    {
        Refresh();
    }

    /// <summary>
    /// 异步刷新表格。
    /// </summary>
    /// <returns></returns>
    public Task Refresh() => Table.RefreshAsync();
}