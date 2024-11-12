namespace Known.Admin.Pages;

/// <summary>
/// 系统日志模块页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/logs")]
public class SysLogList : BaseTablePage<SysLog>
{
    private ILogService Service;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        DefaultQuery = new { CreateTime = $"{date}~{date}" };
        
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<ILogService>();

        Table.OnQuery = Service.QueryLogsAsync;
        Table.Column(c => c.Type).Template((b, r) => b.Tag(r.Type));
    }

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public Task Export() => Table.ExportDataAsync();
}