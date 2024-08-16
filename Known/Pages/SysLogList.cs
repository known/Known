namespace Known.Pages;

[StreamRendering]
[Route("/sys/logs")]
public class SysLogList : BaseTablePage<SysLog>
{
    private ISystemService Service;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<ISystemService>();

        Table.OnQuery = Service.QueryLogsAsync;
        Table.AddQueryColumn(c => c.CreateTime);
        Table.Column(c => c.Type).Template((b, r) => b.Tag(r.Type));
    }

    public async void Export() => await ExportDataAsync();
}