namespace Known.Components;

/// <summary>
/// 系统用户弹窗选择器组件类。
/// </summary>
public class UserPicker : TablePicker<UserInfo>, ICustomField
{
    /// <summary>
    /// 取得或设置用户角色。
    /// </summary>
    [Parameter] public string Role { get; set; }

    /// <summary>
    /// 取得或设置表格查询数据委托。
    /// </summary>
    [Parameter] public Func<PagingCriteria, Task<PagingResult<UserInfo>>> OnQuery { get; set; }

    /// <inheritdoc />
    protected override Dictionary<string, object> GetPickParameters()
    {
        var parameters = base.GetPickParameters();
        parameters[nameof(Role)] = Role;
        parameters[nameof(OnQuery)] = OnQuery;
        return parameters;
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Title = Language.SelectUser;
        Width = 800;
        AllowClear = true;
        ItemExpression = d => d.UserName == Value?.ToString();
        Table.EnableFilter = false;
        Table.OnQuery = QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(120);
        Table.AddColumn(c => c.Email).Width(150);
        Table.AddColumn(c => c.Role);
    }

    /// <inheritdoc />
    protected override void OnValueChanged(List<UserInfo> items)
    {
        var value = SelectType == TableSelectType.Checkbox
                  ? string.Join(",", items.Select(d => d.UserName))
                  : items?.FirstOrDefault()?.UserName;
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(value);
    }

    private Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(UserInfo.Role), Role);
        if (OnQuery != null)
            return OnQuery.Invoke(criteria);

        return Admin.QueryUsersAsync(criteria);
    }
}