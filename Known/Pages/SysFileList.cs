namespace Known.Pages;

/// <summary>
/// 系统附件模块页面组件类。
/// </summary>
[Route("/sys/files")]
[Menu(Constants.System, "系统附件", "file", 5)]
public class SysFileList : BaseTablePage<AttachInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.OnQuery = Admin.QueryFilesAsync;
        Table.ActionWidth = "70";
        Table.Column(c => c.Name).Template(BuildFileName);
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(AttachInfo row) => Table.Delete(Admin.DeleteFilesAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteFilesAsync);

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    [Action] public Task Export() => Table.ExportDataAsync();

    private void BuildFileName(RenderTreeBuilder builder, AttachInfo row)
    {
        builder.FileLink(row);
    }

    private void BuildFileSize(RenderTreeBuilder builder, AttachInfo row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}