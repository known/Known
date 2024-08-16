namespace Known.Pages;

[StreamRendering]
[Route("/sys/files")]
public class SysFileList : BaseTablePage<SysFile>
{
    private IFileService Service;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IFileService>();
        Table.OnQuery = Service.QueryFilesAsync;
        Table.Column(c => c.Name).Template(BuildFileName);
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    public void Delete(SysFile row) => Table.Delete(Service.DeleteFilesAsync, row);
    public void DeleteM() => Table.DeleteM(Service.DeleteFilesAsync);
    public async void Export() => await ExportDataAsync();

    private void BuildFileName(RenderTreeBuilder builder, SysFile row)
    {
        builder.Component<FileLink>().Set(c => c.Item, row).Set(c => c.OpenFile, true).Build();
    }

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}