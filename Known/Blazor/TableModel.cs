namespace Known.Blazor;

/// <summary>
/// 表格组件模型信息类。
/// </summary>
/// <param name="context">UI上下文对象。</param>
public class TableModel(UIContext context) : BaseModel(context)
{
    /// <summary>
    /// 取得或设置表格是否显示高级搜索。
    /// </summary>
    public bool AdvSearch { get; set; }

    /// <summary>
    /// 取得或设置表格默认查询条件匿名对象，对象属性名应与查询实体对应。
    /// </summary>
    public object DefaultQuery { get; set; }

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
    /// 取得表格栏位信息列表。
    /// </summary>
    public List<ColumnInfo> Columns { get; internal set; } = [];

    /// <summary>
    /// 取得或设置表格刷新委托，创建抽象表格时赋值。
    /// </summary>
    public Func<Task> OnRefresh { get; set; }

    /// <summary>
    /// 取得表格工具条配置模型对象。
    /// </summary>
    public ToolbarModel Toolbar { get; } = new();

    /// <summary>
    /// 取得表格是否有工具条按钮。
    /// </summary>
    public bool HasToolbar => Toolbar != null && Toolbar.HasItem;

    internal List<ColumnInfo> AllColumns { get; set; }

    /// <summary>
    /// 刷新表格数据。
    /// </summary>
    /// <returns></returns>
    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }

    /// <summary>
    /// 显示高级搜索对话框。
    /// </summary>
    /// <param name="app">系统模板对象。</param>
    public void ShowAdvancedSearch(BaseLayout app)
    {
        AdvancedSearch search = null;
        var columns = AllColumns;
        if (columns == null || columns.Count == 0)
            columns = Columns;
        var model = new DialogModel
        {
            Title = Language.AdvSearch,
            Width = 700,
            Content = b => b.Component<AdvancedSearch>()
                            .Set(c => c.Columns, columns)
                            .Build(value => search = value)
        };
        model.OnOk = async () =>
        {
            await app.QueryDataAsync(async () =>
            {
                Criteria.Query = await search?.SaveQueryAsync();
                await RefreshAsync();
            });
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 设置默认查询条件数据。
    /// </summary>
    public void SetDefaultQuery()
    {
        QueryData.Clear();
        if (QueryColumns == null || QueryColumns.Count == 0)
            return;

        foreach (var item in QueryColumns)
        {
            var info = new QueryInfo(item);
            info.Value = TypeHelper.GetPropertyValue<string>(DefaultQuery, item.Id);
            QueryData[item.Id] = info;
        }

        Criteria.Query = QueryData.Select(d => d.Value).ToList();
    }
}