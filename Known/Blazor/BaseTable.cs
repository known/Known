namespace Known.Blazor;

public class BaseTable<TItem> : BaseComponent where TItem : class, new()
{
    protected TableModel<TItem> Table { get; private set; }

    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(Context);
        Table.OnAction = (info, item) => OnAction(info, [item]);
        Table.Toolbar.OnItemClick = info => OnAction(info, null);
    }

    protected override void BuildRender(RenderTreeBuilder builder) => builder.BuildTable(Table);
    //protected void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    public virtual Task RefreshAsync() => Table.RefreshAsync();
    public virtual void MoveUp(TItem row) => MoveRow(row, true);
    public virtual void MoveDown(TItem row) => MoveRow(row, false);

    protected void DeleteRow(TItem row)
    {
        if (Table.DataSource == null || Table.DataSource.Count == 0)
            return;

        Table.DataSource.Remove(row);
        StateChanged();
    }

    private async void MoveRow(TItem item, bool isMoveUp, Func<TItem, Task<Result>> action = null, Action<TItem, TItem> success = null)
    {
        if (Table.DataSource == null || Table.DataSource.Count == 0)
            return;

        var index = Table.DataSource.IndexOf(item);
        var index1 = isMoveUp ? index - 1 : index + 1;
        if (index1 < 0 || index1 > Table.DataSource.Count - 1)
            return;

        if (action != null)
        {
            var result = await action(item);
            if (result.IsValid)
                OnMoveRow(item, success, index, index1);
        }
        else
        {
            OnMoveRow(item, success, index, index1);
        }
    }

    private void OnMoveRow(TItem item, Action<TItem, TItem> success, int index, int index1)
    {
        if (Table.DataSource == null || Table.DataSource.Count == 0)
            return;

        var temp = Table.DataSource[index1];
        Table.DataSource[index1] = item;
        Table.DataSource[index] = temp;
        success?.Invoke(item, temp);
        StateChanged();
    }
}