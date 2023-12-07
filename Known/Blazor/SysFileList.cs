using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysFileList : BasePage<SysFile>
{
	private TableModel<SysFile> model;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		model = new TableModel<SysFile>(this);
		model.OnQuery = Platform.File.QueryFilesAsync;
		model.Column(c => c.Size).Template(BuildFileSize);
		model.Column(c => c.CreateTime).DefaultDescend();
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		UI.BuildTablePage(builder, model);
	}

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}