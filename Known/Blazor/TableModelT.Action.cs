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
}