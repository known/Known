namespace Known.Pages;

/// <summary>
/// 同步日志页面组件类。
/// </summary>
[Route("/sys/sync-logs")]
[Menu(Constants.System, "同步日志", "history", 7)]
public class SysSyncLogList : BaseTablePage<SysSyncLog>
{
    private ILogService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        DefaultQuery = new { SyncTime = $"{date} 00:00:00~{date} 23:59:59" };

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ILogService>();

        Table.SetAdminTable();
        Table.ShowIndex = true;
        Table.OnQuery = Service.QuerySyncLogsAsync;
        Table.Column(c => c.SyncType).Tag();
        Table.Column(c => c.SyncResult).Tag();
    }

    /// <summary>
    /// 批量删除同步日志。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteSyncLogsAsync);

    /// <summary>
    /// 删除同步日志。
    /// </summary>
    /// <param name="row"></param>
    [Action] public void Delete(SysSyncLog row) => Table.Delete(Service.DeleteSyncLogsAsync, row);
    
    /// <summary>
    /// 导出同步日志列表。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Export() => Table.ExportDataAsync();
}