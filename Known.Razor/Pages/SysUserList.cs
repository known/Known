using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysUserList : DataGrid<SysUser, SysUserForm>, IPicker
{
    #region IPicker
    public string Title => "选择用户";
    public Size Size => new(700, 400);

    public void BuildPick(RenderTreeBuilder builder)
    {
        builder.Component<SysUserList>()
               .Set(c => c.Role, Role)
               .Set(c => c.OnPicked, OnPicked)
               .Build();
    }

    [Parameter] public string Role { get; set; }

    public void SetRole(string role) => Role = role;

    public override void OnRowDoubleClick(int row, SysUser item)
    {
        OnPicked?.Invoke($"{item.UserName}-{item.Name}");
        UI.CloseDialog();
    }
    #endregion

    protected override Task<PagingResult<SysUser>> OnQueryData(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysUser.Role), Role ?? "");
        return Platform.User.QueryUsersAsync(criteria);
    }

    public override bool CheckAction(ButtonInfo action, SysUser item)
    {
        var isAdmin = item.UserName == "admin" || item.UserName == item.CompNo;
        if (isAdmin && !action.Is(GridAction.View))
            return false;

        return base.CheckAction(action, item);
    }

    public void New() => ShowForm(new SysUser { Enabled = true });
    public void DeleteM() => DeleteRows(Platform.User.DeleteUsersAsync);
    public void ResetPassword() => SelectRow(OnResetPassword);
    public void Enable() => SelectRows(OnEnableUsers);
    public void Disable() => SelectRows(OnDisableUsers);
    public void Edit(SysUser row) => ShowForm(row);
    public void Delete(SysUser row) => DeleteRow(row, Platform.User.DeleteUsersAsync);
    public override void View(SysUser row) => UI.ShowForm<SysUserForm>("查看用户", row);

    protected override void ShowForm(SysUser model = null)
    {
        var action = model == null || model.IsNew ? "新增" : "编辑";
        ShowForm<SysUserForm>($"{action}用户", model);
    }

    private void OnResetPassword(SysUser model)
    {
        UI.Confirm($"确定要重置{model.UserName}的密码？", async () =>
        {
            var result = await Platform.User.SetUserPwdsAsync(new List<SysUser> { model });
            UI.Result(result);
        });
    }

    private void OnEnableUsers(List<SysUser> models)
    {
        UI.Confirm("确定要启用选中的用户？", async () =>
        {
            var result = await Platform.User.EnableUsersAsync(models);
            UI.Result(result, Refresh);
        });
    }

    private void OnDisableUsers(List<SysUser> models)
    {
        UI.Confirm("确定要禁用选中的用户？", async () =>
        {
            var result = await Platform.User.DisableUsersAsync(models);
            UI.Result(result, Refresh);
        });
    }
}