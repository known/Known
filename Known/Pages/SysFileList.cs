using Known.Extensions;
using Known.Razor;

namespace Known.Pages;

[Route("/files")]
public class SysFileList : KDataGrid<SysFile>
{
    public SysFileList()
    {
        OrderBy = $"{nameof(SysTask.CreateTime)} desc";
    }

    protected override Task InitPageAsync()
    {
        Column(c => c.Size).Template(BuildFileSize);
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<SysFile>> OnQueryDataAsync(PagingCriteria criteria)
    {
        return Platform.File.QueryFilesAsync(criteria);
    }

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}