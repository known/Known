namespace Known.Admin.Pages;

/// <summary>
/// 系统附件模块页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/files")]
public class SysFileList : BaseTablePage<SysFile>
{
    private IFileService Service;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IFileService>();
        Table.OnQuery = Service.QueryFilesAsync;
        Table.Column(c => c.Name).Template(BuildFileName);
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysFile row) => Table.Delete(Service.DeleteFilesAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteFilesAsync);

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public Task Export() => Table.ExportDataAsync();

    private void BuildFileName(RenderTreeBuilder builder, SysFile row)
    {
        var info = Utils.MapTo<AttachInfo>(row);
        builder.FileLink(info);
    }

    private void BuildFileSize(RenderTreeBuilder builder, SysFile row)
    {
        var size = Utils.Round(row.Size / 1024M, 0);
        builder.Span($"{size} KB");
    }
}