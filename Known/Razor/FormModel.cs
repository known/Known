using Known.Extensions;

namespace Known.Razor;

public class FormModel<TItem> where TItem : class, new()
{
    internal FormModel(TableModel<TItem> table, List<ColumnAttribute> columns)
    {
        Table = table;
        Fields = columns.Where(c => c.IsForm).Select(c => new FieldModel<TItem>(table.UI, this, c));
        Type = Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
    }

    public TableModel<TItem> Table { get; }
    public IEnumerable<FieldModel<TItem>> Fields { get; }
    public bool IsView { get; set; }
    public string Title { get; set; }
    public double? Width { get; set; }
    public TItem Data { get; set; }
    public Type Type { get; set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Func<TItem, Task<Result>> OnSave { get; set; }

    public async Task SaveAsync()
    {
        if (OnValidate != null)
        {
            if (!OnValidate.Invoke()) return;
        }

        var result = await OnSave?.Invoke(Data);
        Table.UI.Result(result, async () =>
        {
            if (result.IsClose)
                await OnClose?.Invoke();
            await Table.RefreshAsync();
        });
    }
}