namespace Known.Components;

/// <summary>
/// 系统用户弹窗选择器组件类。
/// </summary>
public class UserPicker : TablePicker<SysUser>, ICustomField
{
    private IUserService Service;

    /// <summary>
    /// 异步初始化系统用户弹窗选择器组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IUserService>();
        Title = Language["Title.SelectUser"];
        Width = 800;
        AllowClear = true;
        Table.OnQuery = Service.QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(100);
        Table.AddColumn(c => c.Email).Width(100);
        Table.AddColumn(c => c.Role);
    }

    /// <summary>
    /// 选择器选择内容改变时触发的方法。
    /// </summary>
    /// <param name="items">选中的对象列表。</param>
    protected override void OnValueChanged(List<SysUser> items)
    {
        var value = SelectType == TableSelectType.Checkbox
                  ? string.Join(",", items.Select(d => d.UserName))
                  : items?.FirstOrDefault()?.UserName;
        ValueChanged?.Invoke(value);
    }
}