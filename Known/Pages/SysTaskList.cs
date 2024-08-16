namespace Known.Pages;

[StreamRendering]
[Route("/sys/tasks")]
public class SysTaskList : BaseTablePage<SysTask>
{
    private ISystemService Service;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<ISystemService>();
        Table.OnQuery = Service.QueryTasksAsync;
        Table.Column(c => c.Status).Template((b, r) => b.Tag(r.Status));
    }

    public void Delete(SysTask row) => Table.Delete(Service.DeleteTasksAsync, row);
    public void DeleteM() => Table.DeleteM(Service.DeleteTasksAsync);
    public void Reset() => Table.SelectRows(Service.ResetTasksAsync);
    public async void Export() => await ExportDataAsync();
}