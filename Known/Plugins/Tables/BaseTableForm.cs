namespace Known.Plugins.Tables;

/// <summary>
/// 表格表单基类。
/// </summary>
public class BaseTableForm<TTable, TTableItem, TItem> : BaseForm 
    where TItem : class, new()
    where TTable : IFastTable<TTableItem>
{
    /// <summary>
    /// 表格数据源。
    /// </summary>
    protected internal List<TItem> Items = [];

    /// <summary>
    /// 取得或设置数据改变委托。
    /// </summary>
    [Parameter] public Action OnChange { get; set; }

    /// <summary>
    /// 取得或设置刷新委托。
    /// </summary>
    [Parameter] public Action OnRefresh { get; set; }

    /// <summary>
    /// 设置表格数据源。
    /// </summary>
    /// <param name="items">表格数据源。</param>
    public void SetItems(List<TItem> items)
    {
        Items = items;
        OnChange?.Invoke();
    }

    internal virtual string Query => string.Empty;
    internal virtual List<string> SelectedItems => [];
    internal virtual List<TItem> GetSelectedRows(List<TTableItem> rows) => [];

    internal void OnAddPlus()
    {
        UI.ShowFastTable<TTable, TTableItem>(SelectedItems, rows =>
        {
            var items = GetSelectedRows(rows);
            if (items != null && items.Count > 0)
                Items.AddRange(items);
            OnChange?.Invoke();
        }, Query);
    }

    /// <summary>
    /// 添加数据。
    /// </summary>
    protected void OnAdd()
    {
        Items.Add(new TItem());
        OnChange?.Invoke();
    }

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <param name="row">数据对象。</param>
    protected void OnDelete(TItem row)
    {
        Items.Remove(row);
        OnRefresh?.Invoke();
    }

    /// <summary>
    /// 上移数据。
    /// </summary>
    /// <param name="row">数据对象。</param>
    protected void OnMoveUp(TItem row) => MoveRow(row, true);

    /// <summary>
    /// 下移数据。
    /// </summary>
    /// <param name="row">数据对象。</param>
    protected void OnMoveDown(TItem row) => MoveRow(row, false);

    private void MoveRow(TItem row, bool isMoveUp)
    {
        Items.MoveRow(row, isMoveUp);
        OnRefresh?.Invoke();
    }
}