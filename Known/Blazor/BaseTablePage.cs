namespace Known.Blazor;

public class BaseTablePage<TItem> : BasePage<TItem> where TItem : class, new()
{
    protected TableModel<TItem> Table { get; set; }

    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    public override Task RefreshAsync() => Table.RefreshAsync();
    internal override void ViewForm(FormViewType type, TItem row) => Table.ViewForm(type, row);

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<TItem>(this);
        Table.OnAction = OnActionClick;
        Table.Toolbar.OnItemClick = OnToolClick;
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.BuildTable(Table);

    protected async void ShowImportForm(string param = null)
    {
        var type = typeof(TItem);
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        if (Table.IsDictionary)
            id += $"_{Context.Current.Id}";
        var info = await Platform.File.GetImportAsync(id);
        info.Name = PageName;
        info.BizName = ImportTitle;
        ImportForm(info);
    }

    private void ImportForm(ImportFormInfo info)
    {
        var model = new DialogModel { Title = ImportTitle };
        model.Content = builder =>
        {
            builder.Component<Importer>()
                   .Set(c => c.Model, info)
                   .Set(c => c.OnSuccess, async () =>
                   {
                       await model.CloseAsync();
                       await RefreshAsync();
                   })
                   .Build();
        };
        UI.ShowDialog(model);
    }

    private string ImportTitle => Language["Title.Import"].Replace("{name}", PageName);

    protected Task ExportDataAsync(ExportMode mode) => ExportDataAsync(PageName, mode);

    protected async Task ExportDataAsync(string name, ExportMode mode)
    {
        try
        {
            await App.ShowSpinAsync();
            Table.Criteria.ExportMode = mode;
            Table.Criteria.ExportColumns = GetExportColumns();
            var result = await Table.OnQuery?.Invoke(Table.Criteria);
            var bytes = result.ExportData;
            if (bytes == null || bytes.Length == 0)
                return;

            var stream = new MemoryStream(bytes);
            await JS.DownloadFileAsync($"{name}.xlsx", stream);
            App.HideSpin();
        }
        catch (Exception ex)
        {
            await Error.HandleAsync(ex);
            App.HideSpin();
        }
    }

    private Dictionary<string, string> GetExportColumns()
    {
        var columns = new Dictionary<string, string>();
        if (Table.Columns == null || Table.Columns.Count == 0)
            return columns;

        foreach (var item in Table.Columns)
        {
            columns.Add(item.Id, item.Name);
        }
        return columns;
    }
}