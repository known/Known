namespace Known.Internals;

/// <summary>
/// 查询表单组件类。
/// </summary>
public partial class QueryForm
{
    private bool _expand = false;
    private bool IsAdvSearch => AdvSearch || Model.AdvSearch;
    private const string TipReset = "重置为默认查询条件";

    /// <summary>
    /// 取得或设置表格模型。
    /// </summary>
    [Parameter] public TableModel Model { get; set; }

    /// <summary>
    /// 取得或设置是否显示高级搜索。
    /// </summary>
    [Parameter] public bool AdvSearch { get; set; }

    private async Task OnItemSearchAsync(List<QueryInfo> query)
    {
        Model.Criteria.Query = query;
        await Model.SearchAsync();
    }

    private async Task OnSearchAsync()
    {
        Model.Criteria.Query = [.. Model.QueryData.Select(d => d.Value)];
        await Model.SearchAsync();
    }

    private void ShowAdvSearch() => Model?.ShowAdvancedSearch(App);

    private void OnReset()
    {
        Model?.SetDefaultQuery();
        StateChanged();
    }
}