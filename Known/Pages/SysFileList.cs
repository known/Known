namespace Known.Pages;

[Authorize]
[Route("/sys/files")]
public class SysFileList : BaseTablePage<SysFile>
{
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table.OnQuery = Platform.File.QueryFilesAsync;
    }

    protected override async Task OnPageChangeAsync()
    {
        await base.OnPageChangeAsync();
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}