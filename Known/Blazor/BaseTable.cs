namespace Known.Blazor;

/// <summary>
/// 表格组件基类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public class BaseTable<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得表格组件模型对象实例。
    /// </summary>
    protected TableModel<TItem> Table { get; private set; }

    /// <summary>
    /// 取得表格选中行对象列表。
    /// </summary>
    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(this);
        Table.AdvSearch = false;
        Table.OnAction = (info, item) => OnAction(info, [item]);
        Table.ShowSetting = false;
        Table.Toolbar.OnItemClick = info => OnAction(info, null);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && !IsServerMode)
            await Table.RefreshAsync();
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder) => builder.FormTable(Table);
    //protected void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    /// <inheritdoc />
    public override Task RefreshAsync() => Table.RefreshAsync();

    /// <summary>
    /// 表格行异步上移。
    /// </summary>
    /// <param name="row">行对象。</param>
    public virtual Task MoveUp(TItem row) => MoveRowAsync(row, true);

    /// <summary>
    /// 表格行异步下移。
    /// </summary>
    /// <param name="row">行对象。</param>
    public virtual Task MoveDown(TItem row) => MoveRowAsync(row, false);

    /// <summary>
    /// 删除表格行。
    /// </summary>
    /// <param name="row">行对象。</param>
    protected void DeleteRow(TItem row)
    {
        if (Table.DataSource == null || Table.DataSource.Count == 0)
            return;

        Table.DataSource.Remove(row);
        StateChanged();
    }

    private async Task MoveRowAsync(TItem item, bool isMoveUp, Func<TItem, Task<Result>> action = null, Action<TItem, TItem> success = null)
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