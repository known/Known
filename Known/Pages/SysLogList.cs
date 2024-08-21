namespace Known.Pages;

[StreamRendering]
[Route("/sys/logs")]
public class SysLogList : BaseTablePage<SysLog>
{
    private ISystemService Service;

    protected override async Task OnPageInitAsync()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        DefaultQuery = new { CreateTime = $"{date}~{date}" };
        
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<ISystemService>();

        Table.OnQuery = Service.QueryLogsAsync;
        Table.Column(c => c.Type).Template((b, r) => b.Tag(r.Type));
    }

    public async void Export() => await ExportDataAsync();
}