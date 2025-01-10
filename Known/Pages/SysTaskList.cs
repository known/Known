namespace Known.Pages;

/// <summary>
/// 系统后台任务页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/tasks")]
[Menu(Constants.System, "后台任务", "control", 4)]
public class SysTaskList : BaseTablePage<SysTask>
{
    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.OnQuery = Admin.QueryTasksAsync;
        Table.Column(c => c.Status).Template((b, r) => b.Tag(r.Status));
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysTask row) => Table.Delete(Admin.DeleteTasksAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Admin.DeleteTasksAsync);

    /// <summary>
    /// 批量重置后台任务。
    /// </summary>
    public void Reset() => Table.SelectRows(Admin.ResetTasksAsync);

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public Task Export() => Table.ExportDataAsync();
}