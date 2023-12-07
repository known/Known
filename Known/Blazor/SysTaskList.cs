using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysTaskList : BasePage<SysTask>
{
	private TableModel<SysTask> model;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		model = new TableModel<SysTask>(this);
		model.OnQuery = Platform.System.QueryTasksAsync;
		model.Column(c => c.Status).Template(BuildTaskStatus);
		model.Column(c => c.CreateTime).DefaultDescend();
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		UI.BuildTablePage(builder, model);
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