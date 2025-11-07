namespace Known.Blazor;

/// <summary>
/// 工具条配置模型信息类。
/// </summary>
public class ToolbarModel
{
    /// <summary>
    /// 取得关联的表格模型。
    /// </summary>
    public TableModel Table { get; internal set; }

    /// <summary>
    /// 取得或设置工具条最大显示按钮数量，默认4个。
    /// </summary>
    public int ShowCount { get; set; } = 4;

    /// <summary>
    /// 取得或设置工具条按钮信息列表。
    /// </summary>
    public List<ActionInfo> Items { get; set; } = [];

    /// <summary>
    /// 取得或设置工具条按钮单击事件委托方法。
    /// </summary>
    public Action<ActionInfo> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置工具条左侧模板。
    /// </summary>
    public RenderFragment Left { get; set; }

    /// <summary>
    /// 取得或设置工具条右侧模板。
    /// </summary>
    public RenderFragment Right { get; set; }

    /// <summary>
    /// 取得或设置工具条刷新委托。
    /// </summary>
    internal Action OnRefresh { get; set; }

    /// <summary>
    /// 取得工具条是否有项目。
    /// </summary>
    public bool HasItem => Items != null && Items.Count > 0;

    /// <summary>
    /// 添加一个有权限的工具条按钮。
    /// </summary>
    /// <typeparam name="T">按钮所在组件类型。</typeparam>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="title">按钮提示信息。</param>
    public void AddAction<T>(string idOrName, string title = "")
    {
        if (Table.Context.HasButton<T>(idOrName))
            Table.Toolbar.AddAction(idOrName, title);
    }

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="title">按钮提示信息。</param>
    public void AddAction(string idOrName, string title = "") => Items.Add(idOrName, title);

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="group">按钮分组。</param>
    /// <param name="title">按钮提示信息。</param>
    public void AddAction(string idOrName, string group, string title = "") => Items.Add(idOrName, group, title);

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="badge">徽章数量。</param>
    /// <param name="title">按钮提示信息。</param>
    public void AddAction(string idOrName, int badge, string title = "") => Items.Add(idOrName, badge, title);

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="id">按钮ID。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="icon">按钮图标。</param>
    /// <param name="title">按钮提示信息。</param>
    public void AddAction(string id, string name, string icon, string title = "") => Items.Add(id, name, icon, title);

    /// <summary>
    /// 刷新工具条。
    /// </summary>
    public void Refresh() => OnRefresh?.Invoke();
}