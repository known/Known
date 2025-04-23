namespace Known.Blazor;

/// <summary>
/// 表格组件模型信息类。
/// </summary>
public class TableModel : BaseModel
{
    private object defaultQuery;

    /// <summary>
    /// 构造函数，创建一个表格组件模型实例。
    /// </summary>
    /// <param name="page">表格关联的页面组件。</param>
    /// <param name="id">表格关联的页面组件。</param>
    public TableModel(BaseComponent page, string id = null) : base(page)
    {
        Id = id ?? page?.Context?.Current?.Id;
        Toolbar = new ToolbarModel { Table = this };
    }

    /// <summary>
    /// 取得或设置表格ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置表格名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置表格提示信息。
    /// </summary>
    public string Tips { get; set; }

    /// <summary>
    /// 取得或设置表格是否显示列设置，默认显示。
    /// </summary>
    public bool ShowSetting { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否显示高级搜索。
    /// </summary>
    public bool AdvSearch { get; set; }

    /// <summary>
    /// 取得或设置是否启用在线编辑，默认启用。
    /// </summary>
    public bool EnableEdit { get; set; } = true;

    /// <summary>
    /// 取得或设置表格默认查询条件匿名对象，对象属性名应与查询实体对应。
    /// </summary>
    public object DefaultQuery
    {
        get { return defaultQuery; }
        set
        {
            defaultQuery = value;
            SetDefaultQuery();
        }
    }

    /// <summary>
    /// 取得表格查询栏位信息列表。
    /// </summary>
    public List<ColumnInfo> QueryColumns { get; } = [];

    /// <summary>
    /// 取得表格查询数据信息字典。
    /// </summary>
    public Dictionary<string, QueryInfo> QueryData { get; } = [];

    /// <summary>
    /// 取得表的查询条件对象。
    /// </summary>
    public PagingCriteria Criteria { get; } = new();

    /// <summary>
    /// 取得或设置表格栏位信息列表。
    /// </summary>
    public List<ColumnInfo> Columns { get; set; } = [];

    /// <summary>
    /// 取得或设置表格刷新委托，创建抽象表格时赋值。
    /// </summary>
    internal Func<bool, Task> OnRefresh { get; set; }

    /// <summary>
    /// 取得表格工具条配置模型对象。
    /// </summary>
    public ToolbarModel Toolbar { get; }

    /// <summary>
    /// 取得表格是否有工具条按钮。
    /// </summary>
    public bool HasToolbar => Toolbar != null && Toolbar.HasItem;

    internal virtual string TableId { get; }
    internal virtual Type DataType { get; }
    internal List<ColumnInfo> AllColumns { get; set; } = [];

    internal Task SearchAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke(true);
    }

    /// <summary>
    /// 刷新表格数据。
    /// </summary>
    /// <returns></returns>
    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke(false);
    }

    /// <summary>
    /// 显示高级搜索对话框。
    /// </summary>
    /// <param name="app">系统模板对象。</param>
    public void ShowAdvancedSearch(BaseLayout app)
    {
        AdvancedSearch search = null;
        var isAutoClose = true;
        var model = new DialogModel
        {
            Title = Language.AdvSearch,
            Width = 700,
            Content = b => b.Component<AdvancedSearch>()
                            .Set(c => c.TableId, TableId)
                            .Set(c => c.Columns, AllColumns)
                            .Build(value => search = value)
        };
        model.FooterLeft = b => b.CheckBox(new InputModel<bool>
        {
            Label = "关闭高级搜索框",
            Value = isAutoClose,
            ValueChanged = Component.Callback<bool>(value => isAutoClose = value)
        });
        model.OnOk = async () =>
        {
            await app.QueryDataAsync(async () =>
            {
                Criteria.Query = await search?.SaveQueryAsync();
                await RefreshAsync();
                if (isAutoClose)
                    await model.CloseAsync();
            });
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 设置查询条件栏位。
    /// </summary>
    public void SetQueryColumns()
    {
        QueryColumns.Clear();
        if (AllColumns != null && AllColumns.Count > 0)
            QueryColumns.AddRange(AllColumns.Where(c => c.IsQuery));

        SetDefaultQuery();
    }

    /// <summary>
    /// 设置默认查询条件数据。
    /// </summary>
    public void SetDefaultQuery()
    {
        SetDefaultQuery(defaultQuery);
    }

    private void SetDefaultQuery(object query)
    {
        QueryData.Clear();
        if (QueryColumns == null || QueryColumns.Count == 0)
            return;

        var user = CurrentUser;
        foreach (var item in QueryColumns)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                continue;

            var info = new QueryInfo(item);
            info.Value = item.GetDefaultValue(query, user);
            QueryData[item.Id] = info;
        }

        Criteria.Query = [.. QueryData.Select(d => d.Value)];
    }
}