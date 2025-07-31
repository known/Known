using AntDesign;

namespace Known.Components;

/// <summary>
/// 应用搜索组件类。
/// </summary>
public class AppSearch<TItem> : BaseComponent where TItem : class, new()
{
    private readonly PagingCriteria criteria = new();
    private PagingResult<TItem> result = new();
    private string searchKey;
    private bool isAdd;

    /// <summary>
    /// 取得或设置搜索关键字。
    /// </summary>
    [Parameter] public string Key { get; set; }

    /// <summary>
    /// 取得或设置搜索关键字。
    /// </summary>
    [Parameter] public EventCallback<string> KeyChanged { get; set; }

    /// <summary>
    /// 取得或设置占位符。
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// 取得或设置空数据文本。
    /// </summary>
    [Parameter] public string EmptyText { get; set; }

    /// <summary>
    /// 取得或设置每页查询大小，默认10。
    /// </summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>
    /// 取得或设置查询方法。
    /// </summary>
    [Parameter] public Func<PagingCriteria, Task<PagingResult<TItem>>> OnQuery { get; set; }

    /// <summary>
    /// 取得或设置点击列表项事件。
    /// </summary>
    [Parameter] public Func<TItem, Task> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置搜索内容。
    /// </summary>
    [Parameter] public RenderFragment Search { get; set; }

    /// <summary>
    /// 取得或设置添加内容模板。
    /// </summary>
    [Parameter] public RenderFragment AddTemplate { get; set; }

    /// <summary>
    /// 取得或设置建议内容模板。
    /// </summary>
    [Parameter] public RenderFragment SuggestTemplate { get; set; }

    /// <summary>
    /// 取得或设置子内容模板。
    /// </summary>
    [Parameter] public RenderFragment<TItem> ChildContent { get; set; }

    /// <summary>
    /// 异步搜索。
    /// </summary>
    /// <param name="key">搜索关键字。</param>
    /// <returns></returns>
    public async Task SearchAsync(string key)
    {
        criteria.Clear();
        OnKeyChanged(key);
        await RefreshAsync();
    }

    /// <inheritdoc />
    public override async Task RefreshAsync()
    {
        criteria.Parameters[nameof(Key)] = searchKey;
        result = await OnQuery.Invoke(criteria);
        await StateChangedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        searchKey = Key;
        criteria.PageSize = PageSize;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await RefreshAsync();
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-app-search", () => BuildSearch(builder));
        builder.Div("kui-app-search-result", () => BuildResult(builder));
        builder.Div("kui-app-search-info", () => BuildInfo(builder));
    }

    private void BuildSearch(RenderTreeBuilder builder)
    {
        if (Search != null)
        {
            builder.Fragment(Search);
            return;
        }

        builder.Component<Search>()
               .Set(c => c.Value, searchKey)
               .Set(c => c.Placeholder, Placeholder)
               .Set(c => c.ClassicSearchIcon, true)
               .Set(c => c.OnSearch, this.Callback<string>(OnSearchAsync))
               .Build();

        if (AddTemplate != null)
            builder.Button(Language.Add, this.Callback<MouseEventArgs>(e => isAdd = true));
    }

    private void BuildResult(RenderTreeBuilder builder)
    {
        if (isAdd)
            builder.Div("kui-app-search-add", () => builder.Fragment(AddTemplate));

        if (SuggestTemplate != null)
            builder.Div("kui-app-search-suggest", () => builder.Fragment(SuggestTemplate));

        if (result.TotalCount == 0)
        {
            builder.Component<Empty>().Set(c => c.Description, EmptyText).Build();
            return;
        }

        builder.Component<AntList<TItem>>()
               .Set(c => c.DataSource, result.PageData)
               .Set(c => c.NoResult, EmptyText)
               .Set(c => c.ChildContent, this.BuildTree<TItem>(BuildListItem))
               .Build();
    }

    private void BuildListItem(RenderTreeBuilder builder, TItem item)
    {
        builder.Component<ListItem>()
               .Set(c => c.OnClick, this.Callback(() => OnListItemClick(item)))
               .Set(c => c.ChildContent, b => b.Fragment(ChildContent, item))
               .Build();
    }

    private void BuildInfo(RenderTreeBuilder builder)
    {
        builder.Component<Alert>()
               .Set(c => c.Type, AlertType.Success)
               .Set(c => c.Style, "padding:3px 15px;")
               .Set(c => c.ChildContent, BuildInfoContent)
               .Build();
    }

    private void BuildInfoContent(RenderTreeBuilder builder)
    {
        builder.Div("kui-app-rc2", () =>
        {
            builder.Div("", Language[Language.PageTotal].Replace("{total}", $"{result.TotalCount}"));
            builder.Component<AppPager>()
                   .Set(c => c.Criteria, criteria)
                   .Set(c => c.TotalCount, result.TotalCount)
                   .Set(c => c.OnChanged, OnPageChangedAsync)
                   .Build();
        });
    }

    private Task OnListItemClick(TItem item)
    {
        if (OnItemClick == null)
            return Task.CompletedTask;

        return OnItemClick.Invoke(item);
    }

    private async Task OnSearchAsync(string key)
    {
        criteria.PageIndex = 1;
        OnKeyChanged(key);
        await RefreshAsync();
    }

    private async Task OnPageChangedAsync()
    {
        await RefreshAsync();
    }

    private void OnKeyChanged(string key)
    {
        Key = key;
        searchKey = key;
        if (KeyChanged.HasDelegate)
            KeyChanged.InvokeAsync(key);
    }
}