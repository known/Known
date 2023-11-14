using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class SysTaskList : BasePage<SysTask>
{
    public SysTaskList()
    {
        //OrderBy = $"{nameof(SysTask.CreateTime)} desc";
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Table.Column(c => c.Status).Template(BuildTaskStatus);
    }

    protected override Task<PagingResult<SysTask>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.System.QueryTasksAsync(criteria);
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