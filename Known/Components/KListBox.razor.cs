namespace Known.Components;

/// <summary>
/// 列表框组件类。
/// </summary>
public partial class KListBox
{
    private string curItem;
    private string searchKey;
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

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        items = DataSource;
    }

    private void OnSearch(string key)
    {
        searchKey = key;
        items = DataSource;
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = items?.Where(c => c.Code.Contains(searchKey) || c.Name.Contains(searchKey)).ToList();
        StateChanged();
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