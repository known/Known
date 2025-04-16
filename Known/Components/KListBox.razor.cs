namespace Known.Components;

/// <summary>
/// 列表框组件类。
/// </summary>
public partial class KListBox
{
    private string curItem;
    private string searchKey;
    private List<CodeInfo> dataSource = [];
    private List<CodeInfo> items = [];

    /// <summary>
    /// 取得或设置是否显示搜索。
    /// </summary>
    [Parameter] public bool ShowSearch { get; set; }

    /// <summary>
    /// 取得或设置列表项数据源。
    /// </summary>
    [Parameter] public List<CodeInfo> DataSource { get; set; }

    /// <summary>
    /// 取得或设置列表项单击事件。
    /// </summary>
    [Parameter] public EventCallback<CodeInfo> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    [Parameter] public RenderFragment<CodeInfo> ItemTemplate { get; set; }

    /// <summary>
    /// 取得或设置添加数据按钮单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnAddClick { get; set; }

    /// <summary>
    /// 设置列表数据。
    /// </summary>
    /// <param name="data">列表数据源。</param>
    /// <param name="current">当前选中项目。</param>
    public void SetListBox(List<CodeInfo> data, string current)
    {
        DataSource = data;
        dataSource = DataSource;
        curItem = current;
        OnSearch("");
        StateChanged();
    }

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        dataSource = DataSource;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            items = dataSource;
    }

    private void OnSearch(string key)
    {
        searchKey = key;
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = dataSource?.Where(c => c.Name.Contains(searchKey)).ToList();
        else
            items = dataSource;
    }

    private async Task OnClick(CodeInfo info)
    {
        if (!Enabled)
            return;

        curItem = info.Code;
        if (OnItemClick.HasDelegate)
            await OnItemClick.InvokeAsync(info);
    }
}