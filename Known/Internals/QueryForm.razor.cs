﻿namespace Known.Internals;

/// <summary>
/// 查询表单组件类。
/// </summary>
public partial class QueryForm
{
    private bool _expand = false;
    private QueryDataForm form;
    private bool IsAdvSearch => AdvSearch || Model.AdvSearch;

    /// <summary>
    /// 取得或设置表格模型。
    /// </summary>
    [Parameter] public TableModel Model { get; set; }

    /// <summary>
    /// 取得或设置是否显示高级搜索。
    /// </summary>
    [Parameter] public bool AdvSearch { get; set; }

    private async Task OnSearchAsync()
    {
        Model.Criteria.Query = Model.QueryData.Select(d => d.Value).ToList();
        await Model.SearchAsync();
    }

    private void ShowAdvSearch() => Model?.ShowAdvancedSearch(App);

    private void OnReset()
    {
        Model?.SetDefaultQuery();
        StateChanged();
    }
}