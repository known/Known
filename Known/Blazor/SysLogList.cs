using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysLogList : BaseTablePage<SysLog>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
		Model.OnQuery = Platform.System.QueryLogsAsync;
		Model.AddQueryColumn(c => c.CreateTime);
		Model.Column(c => c.Type).Template(BuildLogType);
		Model.Column(c => c.CreateTime).DefaultDescend();
    }

    private void BuildLogType(RenderTreeBuilder builder, SysLog row)
    {
        var color = row.Type == "登录" ? "success" : "blue";
        UI.BuildTag(builder, row.Type, color);
    }
}