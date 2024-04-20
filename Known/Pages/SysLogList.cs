namespace Known.Pages;

[Route("/sys/logs")]
public class SysLogList : BaseTablePage<SysLog>
{
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table.OnQuery = Platform.System.QueryLogsAsync;
        Table.AddQueryColumn(c => c.CreateTime);
        Table.Column(c => c.Type).Template(BuildLogType);
    }

    private void BuildLogType(RenderTreeBuilder builder, SysLog row) => UI.BuildTag(builder, row.Type);
}