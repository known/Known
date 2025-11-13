namespace Known.Blazor;

partial class TableModel<TItem>
{
    private bool isFirstChange = true;
    internal Dictionary<string, (RenderFragment, RenderFragment)> TabTemplates { get; } = [];
    internal TablePage<TItem> PageComponent { get; set; }

    /// <summary>
    /// 取得表格标签配置对象。
    /// </summary>
    public TabModel Tab { get; private set; }

    /// <summary>
    /// 取得或设置表格当前标签。
    /// </summary>
    public string CurrentTab
    {
        get { return Tab.Current; }
        set { Tab.Current = value; }
    }

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="name">标签名称。</param>
    public void AddTab<T>(string id, string name)
    {
        if (Context.HasButton<T>(id))
            AddTab(name, name);
    }

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
    public void AddTab(string id, string title) => Tab.AddTab(id, title);

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="query">查询条件模板。</param>
    /// <param name="table">表格模板。</param>
    public void AddTab(string id, RenderFragment query, RenderFragment table) => AddTab(id, id, query, table);

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    /// <param name="query">查询条件模板。</param>
    /// <param name="table">表格模板。</param>
    public void AddTab(string id, string title, RenderFragment query, RenderFragment table)
    {
        AddTab(id, title);
        TabTemplates[id] = (query, table);
    }

    private void InitializeTab()
    {
        Tab = new TabModel(Component)
        {
            OnChangeAsync = tab =>
            {
                ChangeAction(tab);
                if (TabTemplates.Count > 0)
                {
                    if (!isFirstChange)
                    {
                        PageComponent?.StateChangedAsync();
                        return RefreshAsync();
                    }
                    isFirstChange = false;
                    return RefreshAsync();
                }
                else
                {
                    return RefreshAsync();
                }
            }
        };
    }
}