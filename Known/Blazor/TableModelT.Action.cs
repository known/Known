namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 取得表格操作列信息列表。
    /// </summary>
    public List<ActionInfo> Actions { get; private set; } = [];

    /// <summary>
    /// 取得或设置表格操作列事件委托。
    /// </summary>
    public Func<TItem, List<ActionInfo>> RowActions { get; set; }

    /// <summary>
    /// 取得或设置表格操作列根据数据更新按钮是否显示的委托。
    /// </summary>
    public Action<TItem, List<ActionInfo>> UpdateRowActions { get; set; }

    /// <summary>
    /// 取得或设置表格行单击事件委托。
    /// </summary>
    public Func<TItem, Task> OnRowClick { get; set; }

    /// <summary>
    /// 取得或设置表格行双击事件委托。
    /// </summary>
    public Func<TItem, Task> OnRowDoubleClick { get; set; }

    /// <summary>
    /// 取得或设置表格操作列按钮单击事件委托。
    /// </summary>
    public Action<ActionInfo, TItem> OnAction { get; set; }

    /// <summary>
    /// 添加操作列按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    public void AddAction(string idOrName) => Actions?.Add(new ActionInfo(idOrName));

    /// <summary>
    /// 添加操作列按钮。
    /// </summary>
    /// <param name="id">按钮ID。</param>
    /// <param name="name">按钮名称。</param>
    public void AddAction(string id, string name) => Actions?.Add(new ActionInfo { Id = id, Name = name });

    /// <summary>
    /// 异步操作表格行数据。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    /// <param name="row">操作对象。</param>
    /// <param name="buttonId">确认操作按钮ID。</param>
    public async Task ActionAsync(Func<TItem, Task<Result>> action, TItem row, string buttonId = null)
    {
        if (!string.IsNullOrWhiteSpace(buttonId))
        {
            UI.Confirm(GetConfirmText(buttonId), async () =>
            {
                var result = await action?.Invoke(row);
                UI.Result(result, PageRefreshAsync);
            });
        }
        else
        {
            var result = await action?.Invoke(row);
            UI.Result(result, PageRefreshAsync);
        }
    }
}