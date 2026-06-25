namespace Known.Components;

/// <summary>
/// 树型框组件类。
/// </summary>
public partial class KTreeBox
{
    private string searchKey;
    private List<MenuInfo> dataSource = [];
    private List<MenuInfo> items = [];
    private List<MenuInfo> treeItems = [];
    private readonly TreeModel model = new();

    private string ClassName => CssBuilder.Default("kui-box").AddClass(Class).BuildClass();

    /// <summary>
    /// 设置数据源。
    /// </summary>
    /// <param name="data">数据源。</param>
    /// <param name="current">当前选中项目。</param>
    public void SetDataSource(List<MenuInfo> data, string current)
    {
        DataSource = data;
        dataSource = DataSource;
        OnSearch("");
        if (OnItemClick.HasDelegate)
        {
            var info = dataSource?.FirstOrDefault(d => d.Code == current);
            OnItemClick.InvokeAsync(info);
        }
    }

    /// <inheritdoc />
    protected override Task OnInitAsync()
    {
        model.OnNodeClick = item => OnItemClick.InvokeAsync(item);
        return base.OnInitAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        dataSource = DataSource;
    }

    private void OnSearch(string key)
    {
        searchKey = key;
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = dataSource?.Where(c => c.Name.Contains(searchKey)).ToList();
        else
            items = dataSource;
        treeItems = items.ToMenuItems(false);
        StateChanged();
    }
}