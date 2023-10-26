namespace Known.Pages;

[Route("/users")]
public class SysUserList : KDataGrid<SysUser, SysUserForm>, IPicker
{
    private readonly List<KTreeItem<SysOrganization>> data = new();
    private KTreeItem<SysOrganization> current;
    private List<SysOrganization> datas;
    private bool hasOrg = false;

    public SysUserList() { }

    public SysUserList(string role, string type = "")
    {
        Role = role;
        Type = type;
    }

    #region IPicker
    public string Title => "选择用户";
    public Size Size => new(700, 400);

    public void BuildPick(RenderTreeBuilder builder)
    {
        builder.Component<SysUserList>()
               .Set(c => c.Role, Role)
               .Set(c => c.Type, Type)
               .Set(c => c.OnPicked, OnPicked)
               .Build();
    }

    [Parameter] public string Role { get; set; }
    [Parameter] public string Type { get; set; }

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

        Column(c => c.Department).IsVisible(hasOrg);
        Column(c => c.Type).IsVisible(Config.IsPlatform && !CurrentUser.IsTenant);
        Column(c => c.UserName).Template((b, r) => b.Link(r.UserName, Callback(() => View(r))));
        await base.InitPageAsync();
    }

    protected override Task<PagingResult<SysUser>> OnQueryDataAsync(PagingCriteria criteria)
    {
        if (OnPicked == null && current != null && current.Value.ParentId != "0")
            criteria.SetQuery(nameof(SysUser.OrgNo), QueryType.Equal, current?.Value.Id);
        criteria.SetQuery(nameof(SysUser.Role), Role ?? "");
        criteria.SetQuery(nameof(SysUser.Type), Type ?? "");
        return Platform.User.QueryUsersAsync(criteria);
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (!hasOrg || OnPicked != null)
        {
            base.BuildPage(builder);
            return;
        }

        builder.ViewLR(left =>
        {
            builder.Component<KTree<SysOrganization>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<KTreeItem<SysOrganization>>(OnTreeItemClick))
                   .Build();
        }, right => base.BuildPage(builder));
    }

    public void New() => ShowForm(new SysUser { OrgNo = current?.Value.Id, Enabled = true });
    public void DeleteM() => DeleteRows(Platform.User.DeleteUsersAsync);
    public void ResetPassword() => SelectRow(OnResetPassword);
    public void ChangeDepartment() => SelectRows(OnChangeDepartment);
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

    private void OnChangeDepartment(List<SysUser> models)
    {
        KTreeItem<SysOrganization> node = null;
        UI.Prompt("更换部门", new(300, 300), builder =>
        {
            builder.Component<KTree<SysOrganization>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<KTreeItem<SysOrganization>>(n => node = n))
                   .Build();
        }, async model =>
        {
            models.ForEach(m => m.OrgNo = node.Value.Id);
            var result = await Platform.User.ChangeDepartmentAsync(models);
            UI.Result(result, () =>
            {
                Refresh();
                UI.CloseDialog();
            });
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
        var root = new KTreeItem<SysOrganization> { Value = org, Text = org.Name, IsExpanded = true };
        data.Add(root);
        current = root;
        InitTreeNode(root);
        Refresh();
    }

    private void OnTreeItemClick(KTreeItem<SysOrganization> item)
    {
        current = item;
        Refresh();
    }

    private async void InitTreeNode(KTreeItem<SysOrganization> item, bool reload = false)
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
            var sub = new KTreeItem<SysOrganization> { Value = child, Text = child.Name };
            item.AddChild(sub);
            InitTreeNode(sub);
        }
    }
}