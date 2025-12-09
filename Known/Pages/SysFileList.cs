namespace Known.Pages;

/// <summary>
/// 系统附件页面组件类。
/// </summary>
[Route("/sys/files")]
[Menu(Constants.System, "系统附件", "file", 5)]
//[PagePlugin("系统附件", "file", PagePluginType.Module, Language.SystemManage, Sort = 8)]
public class SysFileList : BaseTablePage<AttachInfo>
{
    private IFileService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IFileService>();

        Table.SetAdminTable();
        Table.OnQuery = Service.QueryFilesAsync;
        Table.ActionWidth = "70";
        Table.Column(c => c.Name).Template(BuildFileName);
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    /// <summary>
    /// 删除附件。
    /// </summary>
    /// <param name="row">附件信息。</param>
    [Action] public void Delete(AttachInfo row) => Table.Delete(Service.DeleteFilesAsync, row);

    /// <summary>
    /// 批量删除附件。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteFilesAsync);

    /// <summary>
    /// 导出附件列表。
    /// </summary>
    /// <returns></returns>
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