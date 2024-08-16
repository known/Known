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
        Table.Initialize(this);
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Table(Table);

    protected async void ShowImportForm(string param = null) => await Table.ShowImportFormAsync(param);
    protected Task ExportDataAsync(ExportMode mode = ExportMode.Query) => Table.ExportDataAsync(mode);
    protected Task ExportDataAsync(string name, ExportMode mode = ExportMode.Query) => Table.ExportDataAsync(name, mode);
}