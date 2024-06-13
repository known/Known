namespace Known.Blazor;

public class BaseTablePage<TItem> : BasePage<TItem> where TItem : class, new()
{
    protected TableModel<TItem> Table { get; set; }

    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    public override Task RefreshAsync() => Table.RefreshAsync();
    internal override void ViewForm(FormViewType type, TItem row) => Table.ViewForm(type, row);

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table = new TableModel<TItem>(this);
    }

    protected override async Task OnPageChangeAsync()
    {
        await base.OnPageChangeAsync();
        Table.Initialize(this);
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
        var fileService = await CreateServiceAsync<IFileService>();
        var info = await fileService.GetImportAsync(id);
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

    protected Task ExportDataAsync(ExportMode mode = ExportMode.Query) => ExportDataAsync(PageName, mode);
    protected Task ExportDataAsync(string name, ExportMode mode = ExportMode.Query) => App?.ExportDataAsync(Table, name, mode);
}