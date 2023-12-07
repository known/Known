using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysTaskList : BaseTablePage<SysTask>
{
	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		Model.OnQuery = Platform.System.QueryTasksAsync;
		Model.Column(c => c.Status).Template(BuildTaskStatus);
		Model.Column(c => c.CreateTime).DefaultDescend();
	}

    private void BuildTaskStatus(RenderTreeBuilder builder, SysTask row)
    {
        var color = "default";
        switch (row.Status)
        {
            case TaskStatus.Pending:
                color = "default";
                break;
            case TaskStatus.Running:
                color = "processing";
                break;
            case TaskStatus.Success:
                color = "success";
                break;
            case TaskStatus.Failed:
                color = "error";
                break;
        }
        UI.BuildTag(builder, row.Status, color);
    }
}