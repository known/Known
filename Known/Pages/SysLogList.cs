namespace Known.Pages;

/// <summary>
/// 系统日志页面组件类。
/// </summary>
[Route("/sys/logs")]
[Menu(Constants.System, "系统日志", "clock-circle", 6)]
//[PagePlugin("系统日志", "clock-circle", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 9)]
public class SysLogList : BaseTablePage<LogInfo>
{
    private ILogService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        DefaultQuery = new { CreateTime = $"{date}~{date}" };

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ILogService>();

        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.ShowIndex = true;
        Table.SelectType = TableSelectType.None;
        Table.OnQuery = Service.QueryLogsAsync;
        Table.Column(c => c.Type).Tag();
        Table.Column(c => c.Target).Width(200);
        Table.Column(c => c.CreateTime).Type(FieldType.DateTime);
    }

    /// <summary>
    /// 导出日志列表。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Export() => Table.ExportDataAsync();
}