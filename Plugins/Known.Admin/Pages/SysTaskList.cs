namespace Known.Admin.Pages;

/// <summary>
/// 系统后台任务页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/tasks")]
public class SysTaskList : BaseTablePage<SysTask>
{
    private ITaskService Service;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<ITaskService>();
        Table.OnQuery = Service.QueryTasksAsync;
        Table.Column(c => c.Status).Template((b, r) => b.Tag(r.Status));
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysTask row) => Table.Delete(Service.DeleteTasksAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteTasksAsync);

    /// <summary>
    /// 批量重置后台任务。
    /// </summary>
    public void Reset() => Table.SelectRows(Service.ResetTasksAsync);

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public Task Export() => Table.ExportDataAsync();
}