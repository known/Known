namespace Known.Blazor;

/// <summary>
/// 树组件配置模型信息类。
/// </summary>
public class TreeModel
{
    /// <summary>
    /// 取得或设置树组件是否是查看模式。
    /// </summary>
    public bool IsView { get; set; }

    /// <summary>
    /// 取得或设置树组件节点是否显示勾选框。
    /// </summary>
    public bool Checkable { get; set; }

    /// <summary>
    /// 取得或设置树组件是否展开根节点。
    /// </summary>
    public bool ExpandRoot { get; set; }

    /// <summary>
    /// 取得或设置树组件默认展开节点的主键集合。
    /// </summary>
    public string[] DefaultExpandedKeys { get; set; }

    /// <summary>
    /// 取得或设置树组件选中节点的主键集合。
    /// </summary>
    public string[] SelectedKeys { get; set; }

    /// <summary>
    /// 取得或设置树组件勾选节点的主键集合。
    /// </summary>
    public string[] CheckedKeys { get; set; }

    /// <summary>
    /// 取得或设置树组件禁用节点的主键集合。
    /// </summary>
    public string[] DisableCheckKeys { get; set; }

    /// <summary>
    /// 取得或设置树组件数据源。
    /// </summary>
    public List<MenuInfo> Data { get; set; }

    /// <summary>
    /// 取得或设置树组件节点单击事件委托。
    /// </summary>
    public Func<MenuInfo, Task> OnNodeClick { get; set; }

    /// <summary>
    /// 取得或设置树组件节点勾选事件委托。
    /// </summary>
    public Func<MenuInfo, Task> OnNodeCheck { get; set; }

    /// <summary>
    /// 取得或设置树组件模型改变事件委托。
    /// </summary>
    public Func<Task<TreeModel>> OnModelChanged { get; set; }

    /// <summary>
    /// 取得或设置树组件刷新委托，创建组件时赋值。
    /// </summary>
    internal Func<Task> OnRefresh { get; set; }

    /// <summary>
    /// 异步刷新树组件。
    /// </summary>
    /// <returns></returns>
    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }
}