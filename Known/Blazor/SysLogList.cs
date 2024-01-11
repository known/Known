using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysLogList : BaseTablePage<SysLog>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
		Table.OnQuery = Platform.System.QueryLogsAsync;
		Table.AddQueryColumn(c => c.CreateTime);
		Table.Column(c => c.Type).Template(BuildLogType);
    }

    private void BuildLogType(RenderTreeBuilder builder, SysLog row) => UI.BuildTag(builder, row.Type);
}