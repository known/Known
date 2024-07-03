namespace Known.Pages;

[StreamRendering]
[Route("/sys/files")]
public class SysFileList : BaseTablePage<SysFile>
{
    private IFileService fileService;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        fileService = await CreateServiceAsync<IFileService>();
        Table.OnQuery = fileService.QueryFilesAsync;
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}