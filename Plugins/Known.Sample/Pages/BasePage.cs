namespace Known.Sample.Pages;

public class BizTablePage<TItem> : BaseTablePage<TItem> where TItem : class, new()
{
    [Action] public Task Import() => Table.ShowImportAsync();
    //[Action] public Task Export() => Table.ExportDataAsync();
    [Action(Group = "Export")] public Task ExportSelect() => Table.ExportDataAsync(ExportMode.Select);
    [Action(Group = "Export")] public Task ExportPage() => Table.ExportDataAsync(ExportMode.Page);
    [Action(Group = "Export")] public Task ExportQuery() => Table.ExportDataAsync(ExportMode.Query);
    [Action(Group = "Export")] public Task ExportAll() => Table.ExportDataAsync(ExportMode.All);
}