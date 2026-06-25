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

    private string ClassName => CssBuilder.Default("kui-panel-box").AddClass(Class).BuildClass();

    /// <summary>
    /// 设置列表数据源。
    /// </summary>
    /// <param name="data">列表数据源。</param>
    /// <param name="current">当前选中项目。</param>
    public void SetDataSource(List<CodeInfo> data, string current)
    {
        DataSource = data;
        dataSource = DataSource;
        curItem = current;
        OnSearch("");
        if (OnItemClick.HasDelegate)
        {
            var info = dataSource?.FirstOrDefault(d => d.Code == current);
            OnItemClick.InvokeAsync(info);
        }
    }

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        dataSource = DataSource;
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            items = dataSource;
            await OnClick(items?.FirstOrDefault());
        }
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
        if (!Enabled || info == null)
            return;

        curItem = info.Code;
        if (OnItemClick.HasDelegate)
            await OnItemClick.InvokeAsync(info);
    }
}