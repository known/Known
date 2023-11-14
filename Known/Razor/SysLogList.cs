using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class SysLogList : BasePage<SysLog>
{
    public SysLogList()
    {
        //OrderBy = $"{nameof(SysLog.CreateTime)} desc";
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Table.AddQueryColumn(c => c.CreateTime);
        Page.Table.Column(c => c.Type).Template(BuildLogType);
    }

    protected override Task<PagingResult<SysLog>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.System.QueryLogsAsync(criteria);
    }

    private void BuildLogType(RenderTreeBuilder builder, SysLog row)
    {
        var color = row.Type == "登录" ? "success" : "blue";
        UI.BuildTag(builder, row.Type, color);
    }
}