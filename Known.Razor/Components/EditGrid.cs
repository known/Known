namespace Known.Razor.Components;

public class EditGrid<TItem> : DataGrid<TItem> where TItem : class, new()
{
    public EditGrid()
    {
        IsEdit = true;
        IsSort = false;
        ShowEmpty = false;
        ShowPager = false;
        ActionHead = b => b.Link(Language.Add, Callback(OnAdd));
        Actions = new List<ButtonInfo> { GridAction.Delete, GridAction.MoveUp, GridAction.MoveDown };
    }

    public void Delete(TItem row) => RemoveData(row);
    public void MoveUp(TItem row) => MoveRow(row, true);
    public void MoveDown(TItem row) => MoveRow(row, false);
    protected virtual void OnAdd() => AddData(new TItem());
    protected virtual void OnInsert() => InsertData(new TItem());

    private void AddData(TItem item)
    {
        Data?.Add(item);
        StateChanged();
    }

    private void InsertData(TItem item)
    {
        var index = CurRow < 0 ? 0 : CurRow;
        Data?.Insert(index, item);
        StateChanged();
    }

    private void RemoveData(TItem item)
    {
        Data?.Remove(item);
        StateChanged();
    }
}