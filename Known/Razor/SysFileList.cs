using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class SysFileList : BasePage<SysFile>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Table.Column(c => c.Size).Template(BuildFileSize);
        Page.Table.Column(c => c.CreateTime).DefaultSort("desc");
    }

    protected override Task<PagingResult<SysFile>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.File.QueryFilesAsync(criteria);
    }

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}