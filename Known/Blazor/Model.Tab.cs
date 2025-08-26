namespace Known.Blazor;

/// <summary>
/// 标签配置模型信息类。
/// </summary>
public class TabModel
{
    /// <summary>
    /// 取得或设置步骤CSS类名。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置当前标签。
    /// </summary>
    public string Current { get; set; }

    /// <summary>
    /// 取得或设置标签左侧模板。
    /// </summary>
    public RenderFragment Left { get; set; }

    /// <summary>
    /// 取得或设置标签右侧模板。
    /// </summary>
    public RenderFragment Right { get; set; }

    /// <summary>
    /// 取得或设置标签项目信息列表。
    /// </summary>
    public List<ItemModel> Items { get; } = [];

    /// <summary>
    /// 取得或设置标签改变委托方法。
    /// </summary>
    public Action<string> OnChange { get; set; }

    /// <summary>
    /// 取得或设置标签改变异步委托方法。
    /// </summary>
    public Func<string, Task> OnChangeAsync { get; set; }

    /// <summary>
    /// 取得或设置组件状态改变方法委托。
    /// </summary>
    internal Action OnStateChanged { get; set; }

    /// <summary>
    /// 取得标签是否有项目。
    /// </summary>
    public bool HasItem => Items != null && Items.Count > 0;

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    public void AddTab(string id) => AddTab(id, id);

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    public void AddTab(string id, string title) => Items.Add(new ItemModel(id, title));

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    /// <param name="table">标签表格配置模型。</param>
    public void AddTab(string id, string title, TableModel table) => Items.Add(new ItemModel(id, title) { Table = table });

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="content">标签呈现模板。</param>
    public void AddTab(string id, RenderFragment content) => AddTab(id, id, content);

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    /// <param name="content">标签呈现模板。</param>
    public void AddTab(string id, string title, RenderFragment content) => Items.Add(new ItemModel(id, title) { Content = content });

    /// <summary>
    /// 改变组件状态。
    /// </summary>
    public void StateChanged() => OnStateChanged?.Invoke();

    internal Func<Task> OnRefreshAsync { get; set; }

    internal async Task ChangeAsync(string tab)
    {
        Current = tab;
        OnChange?.Invoke(tab);
        if (OnChangeAsync != null)
            await OnChangeAsync(tab);
        if (OnRefreshAsync != null)
            await OnRefreshAsync();
    }
}