namespace Known.Components;

/// <summary>
/// 用户下拉表格组件类。
/// </summary>
public class UserDropdownTable : AntDropdownTable<UserInfo>, ICustomField
{
    private IUserService Service;

    /// <summary>
    /// 取得或设置字段组件是否只读。
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置字段关联的栏位配置信息。
    /// </summary>
    [Parameter] public ColumnInfo Column { get; set; }

    /// <summary>
    /// 取得或设置用户角色。
    /// </summary>
    [Parameter] public string Role { get; set; }

    /// <inheritdoc />
    protected override Func<UserInfo, string> OnValue => d => d?.UserName;

    /// <inheritdoc />
    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();
        Service = await CreateServiceAsync<IUserService>();

        Table.Form = new FormInfo { Width = 800 };
        Table.OnQuery = QueryUsersAsync;
        Table.AddColumn(c => c.UserName).ViewLink(false);
        Table.AddColumn(c => c.Name, true);
        Table.AddColumn(c => c.Mobile);
        Table.AddColumn(c => c.Email);
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Disabled = ReadOnly;
    }

    private Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(UserInfo.Role), Role);
        return Service.QueryUsersAsync(criteria);
    }
}