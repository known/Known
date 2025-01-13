﻿namespace Known.Pages;

/// <summary>
/// 系统日志模块页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/logs")]
[Menu(Constants.System, "系统日志", "clock-circle", 6)]
public class SysLogList : BaseTablePage<LogInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        DefaultQuery = new { CreateTime = $"{date}~{date}" };

        await base.OnInitPageAsync();

        Table.OnQuery = Admin.QueryLogsAsync;
        Table.Column(c => c.Type).Template((b, r) => b.Tag(r.Type));
    }

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    [Action] public Task Export() => Table.ExportDataAsync();
}