using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysFileList : BaseTablePage<SysFile>
{
	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		Table.OnQuery = Platform.File.QueryFilesAsync;
		Table.Column(c => c.Size).Template(BuildFileSize);
	}

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}