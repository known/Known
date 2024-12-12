namespace Known.Blazor;

partial class TableModel<TItem>
{
    private List<TItem> dataSource = [];
    /// <summary>
    /// 取得或设置表格数据源。
    /// </summary>
    public List<TItem> DataSource
    {
        get { return dataSource; }
        set
        {
            dataSource = value ?? [];
            result = new PagingResult<TItem>(dataSource);
            OnResult?.Invoke();
        }
    }

    private PagingResult<TItem> result = new();
    /// <summary>
    /// 取得或设置表格分页查询结果。
    /// </summary>
    public PagingResult<TItem> Result
    {
        get { return result; }
        set
        {
            result = value ?? new();
            dataSource = value?.PageData;
            OnResult?.Invoke();
        }
    }

    /// <summary>
    /// 取得或设置表格查询数据委托。
    /// </summary>
    public Func<PagingCriteria, Task<PagingResult<TItem>>> OnQuery { get; set; }

    /// <summary>
    /// 取得或设置表格刷新后调用的委托。
    /// </summary>
    public Action OnRefreshed { get; set; }

    /// <summary>
    /// 取得或设置查询出结果后调用的委托。
    /// </summary>
    public Action OnResult { get; set; }

    /// <summary>
    /// 取得或设置表格顶部统计信息模板。
    /// </summary>
    public RenderFragment<PagingResult<TItem>> TopStatis { get; set; }

    internal Func<PagingResult<TItem>, Task> OnRefreshStatis { get; set; }

    /// <summary>
    /// 异步刷新表格数据统计。
    /// </summary>
    /// <returns></returns>
    internal Task RefreshStatisAsync()
    {
        if (OnRefreshStatis == null)
            return Task.CompletedTask;

        return OnRefreshStatis.Invoke(Result);
    }

    internal async Task PageRefreshAsync()
    {
        if (Page != null)
            await Page.RefreshAsync();
        else
            await RefreshAsync();

        OnRefreshed?.Invoke();
    }
}