namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/tasks")]
public class SysTaskList : BaseTablePage<SysTask>
{
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table.OnQuery = Platform.System.QueryTasksAsync;
    }

    protected override async Task OnPageChangeAsync()
    {
        await base.OnPageChangeAsync();
        Table.Column(c => c.Status).Template(BuildTaskStatus);
    }

    private void BuildTaskStatus(RenderTreeBuilder builder, SysTask row) => UI.BuildTag(builder, row.Status);
}