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
    }

    protected override async Task OnPageChangeAsync()
    {
        await base.OnPageChangeAsync();
        Table.Column(c => c.Type).Template(BuildLogType);
    }

    private void BuildLogType(RenderTreeBuilder builder, SysLog row) => UI.BuildTag(builder, row.Type);
}