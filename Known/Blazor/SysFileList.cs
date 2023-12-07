using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysFileList : BaseTablePage<SysFile>
{
	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		Model.OnQuery = Platform.File.QueryFilesAsync;
		Model.Column(c => c.Size).Template(BuildFileSize);
		Model.Column(c => c.CreateTime).DefaultDescend();
	}

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}