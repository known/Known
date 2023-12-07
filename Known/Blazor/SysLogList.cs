using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysLogList : BasePage<SysLog>
{
    private TableModel<SysLog> model;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        model = new TableModel<SysLog>(this);
        model.OnQuery = Platform.System.QueryLogsAsync;
		model.AddQueryColumn(c => c.CreateTime);
		model.Column(c => c.Type).Template(BuildLogType);
		model.Column(c => c.CreateTime).DefaultDescend();
    }

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
        UI.BuildTablePage(builder, model);
	}

    private void BuildLogType(RenderTreeBuilder builder, SysLog row)
    {
        var color = row.Type == "登录" ? "success" : "blue";
        UI.BuildTag(builder, row.Type, color);
    }
}