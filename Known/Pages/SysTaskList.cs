namespace Known.Pages;

[StreamRendering]
[Route("/sys/tasks")]
public class SysTaskList : BaseTablePage<SysTask>
{
    private ISystemService systemService;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        systemService = await CreateServiceAsync<ISystemService>();
        Table.OnQuery = systemService.QueryTasksAsync;
        Table.Column(c => c.Status).Template((b, r) => b.Tag(r.Status));
    }
}