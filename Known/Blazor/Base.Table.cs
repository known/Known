namespace Known.Blazor;

/// <summary>
/// 表格组件基类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public class BaseTable<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表格组件模型对象实例。
    /// </summary>
    protected TableModel<TItem> Table { get; set; }

    /// <summary>
    /// 取得表格选中行对象列表。
    /// </summary>
    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    /// <summary>
    /// 取得表格数据总记录输。
    /// </summary>
    public int TotalCount => Table.Result.TotalCount;

    /// <summary>
    /// 取得表格数据列表。
    /// </summary>
    public List<TItem> DataSource => Table.DataSource;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(this);
        Table.OnAction = (info, item) => OnAction(info, [item]);
        Table.Toolbar.OnItemClick = info => OnAction(info, null);

        var type = GetType();
        if (type.HasAttribute<TabRoleAttribute>() || type.HasAttribute<RoleAttribute>())
            Table.Initialize(true);

        Table.AdvSearch = false;
        Table.AutoHeight = false;
        Table.ShowName = false;
        Table.ShowSetting = false;
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
        if (DataSource == null || DataSource.Count == 0)
            return;

        DataSource.Remove(row);
        StateChanged();
    }

    private async Task MoveRowAsync(TItem item, bool isMoveUp, Func<TItem, Task<Result>> action = null, Action<TItem, TItem> success = null)
    {
        if (DataSource == null || DataSource.Count == 0)
            return;

        var index = DataSource.IndexOf(item);
        var index1 = isMoveUp ? index - 1 : index + 1;
        if (index1 < 0 || index1 > DataSource.Count - 1)
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
        if (DataSource == null || DataSource.Count == 0)
            return;

        var temp = DataSource[index1];
        DataSource[index1] = item;
        DataSource[index] = temp;
        success?.Invoke(item, temp);
        StateChanged();
    }
}