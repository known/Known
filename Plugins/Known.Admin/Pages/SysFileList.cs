namespace Known.Pages;

[Route("/sys/files")]
[Menu(Constants.System, "系统附件", "file", 5)]
//[PagePlugin("系统附件", "file", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 8)]
public class SysFileList : BaseTablePage<AttachInfo>
{
    private IFileService Service;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IFileService>();

        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.OnQuery = Service.QueryFilesAsync;
        Table.ActionWidth = "70";
        Table.Column(c => c.Name).Template(BuildFileName);
        Table.Column(c => c.Size).Template(BuildFileSize);
    }

    [Action] public void Delete(AttachInfo row) => Table.Delete(Service.DeleteFilesAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteFilesAsync);
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