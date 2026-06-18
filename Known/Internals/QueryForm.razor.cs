namespace Known.Internals;

/// <summary>
/// 查询表单组件类。
/// </summary>
public partial class QueryForm
{
    private bool _expand = false;
    private bool IsAdvSearch => AdvSearch || Model.AdvSearch;
    private List<SearchScheme> _schemes = [];
    private string _activeSchemeId;
    private string SchemeKey => $"UserSearchScheme_{Context.Current?.Id}_{Model.TableId}";

    /// <summary>
    /// 取得或设置表格模型。
    /// </summary>
    [Parameter] public TableModel Model { get; set; }

    /// <summary>
    /// 取得或设置是否显示高级搜索。
    /// </summary>
    [Parameter] public bool AdvSearch { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        if (IsAdvSearch)
            await LoadSchemesAsync();
    }

    private async Task LoadSchemesAsync()
    {
        var json = await Admin.GetUserSettingAsync(SchemeKey);
        if (!string.IsNullOrWhiteSpace(json))
        {
            var items = Utils.FromJson<List<SearchScheme>>(json);
            if (items != null)
                _schemes = items;
        }
    }

    private async Task OnSchemeClick(SearchScheme scheme)
    {
        _activeSchemeId = scheme.Id;
        if (scheme.Mode == SearchMode.Normal && scheme.Conditions != null)
        {
            Model.Criteria.Query = scheme.Conditions;
        }
        else if (scheme.Mode == SearchMode.Advanced && scheme.Groups != null)
        {
            var query = new List<QueryInfo>();
            foreach (var group in scheme.Groups)
            {
                var groupId = group.Id ?? Utils.GetGuid();
                foreach (var condition in group.Conditions)
                {
                    condition.GroupId = groupId;
                    query.Add(condition);
                }
            }
            Model.Criteria.Query = query;
        }
        await Model.SearchAsync();
    }

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

    private async void ShowAdvSearch()
    {
        Model?.ShowAdvancedSearch(App, async () =>
        {
            await LoadSchemesAsync();
            await StateChangedAsync();
        });
    }

    private void OnReset()
    {
        Model?.SetDefaultQuery();
        Model?.Reload();
        StateChanged();
    }
}