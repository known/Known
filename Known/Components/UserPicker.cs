namespace Known.Components;

/// <summary>
/// 系统用户弹窗选择器组件类。
/// </summary>
public class UserPicker : TablePicker<UserInfo>, ICustomField
{
    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Title = Language["Title.SelectUser"];
        Width = 800;
        AllowClear = true;
        ItemExpression = d => d.UserName == Value?.ToString();
        Table.OnQuery = Admin.QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(100);
        Table.AddColumn(c => c.Email).Width(100);
        Table.AddColumn(c => c.Role);
    }

    /// <inheritdoc />
    protected override void OnValueChanged(List<UserInfo> items)
    {
        var value = SelectType == TableSelectType.Checkbox
                  ? string.Join(",", items.Select(d => d.UserName))
                  : items?.FirstOrDefault()?.UserName;
        ValueChanged?.Invoke(value);
    }
}