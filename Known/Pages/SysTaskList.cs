﻿namespace Known.Pages;

[Route("/sys/tasks")]
public class SysTaskList : BaseTablePage<SysTask>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.OnQuery = Platform.System.QueryTasksAsync;
        Table.Column(c => c.Status).Template(BuildTaskStatus);
    }

    private void BuildTaskStatus(RenderTreeBuilder builder, SysTask row) => UI.BuildTag(builder, row.Status);
}