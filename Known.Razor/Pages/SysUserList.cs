using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysUserList : DataGrid<SysUser, SysUserForm>, IPicker
{
    private readonly List<TreeItem<SysOrganization>> data = new();
    private TreeItem<SysOrganization> current;
    private List<SysOrganization> datas;
    private bool hasOrg = false;

    public SysUserList() { }

    public SysUserList(string role)
    {
        Role = role;
    }

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

    protected override async Task InitPageAsync()
    {
        if (OnPicked != null)
        {
            Tools = null;
            var builder = new ColumnBuilder<SysUser>();
            builder.Field(r => r.UserName);
            builder.Field(r => r.Name, true);
            builder.Field(r => r.Phone);
            builder.Field(r => r.Mobile);
            builder.Field(r => r.Email);
            builder.Field(r => r.Role);
            Columns = builder.ToColumns();
        }

        datas = await Platform.Company.GetOrganizationsAsync();
        hasOrg = datas != null && datas.Count > 1;
        InitTreeNode();
        await base.InitPageAsync();
    }

    protected override Task<PagingResult<SysUser>> OnQueryData(PagingCriteria criteria)
    {
        if (OnPicked == null && current != null && current.Value.ParentId != "0")
            criteria.SetQuery(nameof(SysUser.OrgNo), current?.Value.Id);
        criteria.SetQuery(nameof(SysUser.Role), Role ?? "");
        return Platform.User.QueryUsersAsync(criteria);
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (!hasOrg || OnPicked != null)
        {
            base.BuildPage(builder);
            return;
        }

        builder.Div("lr-view", attr =>
        {
            builder.Div("left-view", attr =>
            {
                builder.Component<Tree<SysOrganization>>()
                       .Set(c => c.Data, data)
                       .Set(c => c.OnItemClick, Callback<TreeItem<SysOrganization>>(OnTreeItemClick))
                       .Build();
            });
            builder.Div("right-view", attr => base.BuildPage(builder));
        });
    }

    public void New() => ShowForm(new SysUser { OrgNo = current?.Value.Id, Enabled = true });
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

    private void InitTreeNode()
    {
        if (!hasOrg)
            return;

        data.Clear();
        var org = datas.FirstOrDefault(d => d.ParentId == "0");
        var root = new TreeItem<SysOrganization> { Value = org, Text = org.Name, IsExpanded = true };
        data.Add(root);
        current = root;
        InitTreeNode(root);
        Refresh();
    }

    private void OnTreeItemClick(TreeItem<SysOrganization> item)
    {
        current = item;
        Refresh();
    }

    private async void InitTreeNode(TreeItem<SysOrganization> item, bool reload = false)
    {
        if (reload)
            datas = await Platform.Company.GetOrganizationsAsync();

        item.Children.Clear();
        if (datas == null || datas.Count == 0)
            return;

        var children = datas.Where(m => m.ParentId == item.Value.Id).ToList();
        if (children == null || children.Count == 0)
            return;

        foreach (var child in children)
        {
            var sub = new TreeItem<SysOrganization> { Value = child, Text = child.Name };
            item.AddChild(sub);
            InitTreeNode(sub);
        }
    }
}