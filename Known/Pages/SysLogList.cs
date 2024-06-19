namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/logs")]
public class SysLogList : BaseTablePage<SysLog>
{
    private ISystemService systemService;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        systemService = await CreateServiceAsync<ISystemService>();

        Table.OnQuery = systemService.QueryLogsAsync;
        Table.AddQueryColumn(c => c.CreateTime);
        Table.Column(c => c.Type).Template((b, r) => b.Tag(r.Type));
    }
}