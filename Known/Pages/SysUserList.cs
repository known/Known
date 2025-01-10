namespace Known.Pages;

/// <summary>
/// 系统用户管理页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/users")]
[Menu(Constants.System, "用户管理", "user", 3)]
public class SysUserList : BasePage<SysUser>
{
    private List<SysOrganization> orgs;
    private SysOrganization currentOrg;
    private TreeModel Tree;
    private TableModel<SysUser> Table;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        orgs = await Admin.GetOrganizationsAsync();
        var hasOrg = orgs != null && orgs.Count > 1;
        if (hasOrg)
        {
            Page.Type = PageType.Column;
            Page.Spans = "28";

            currentOrg = orgs.FirstOrDefault(o => o.ParentId == "0");
            Tree = new TreeModel
            {
                ExpandRoot = true,
                Data = orgs.ToMenuItems(),
                OnNodeClick = OnNodeClickAsync,
                SelectedKeys = [currentOrg.Id]
            };

            Page.AddItem("kui-card kui-p10", BuildTree);
        }

        Table = new TableModel<SysUser>(this)
        {
            FormType = typeof(UserForm),
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync
        };
        Table.Toolbar.ShowCount = 6;
        Table.Column(c => c.Gender).Template((b, r) => b.Tag(r.Gender));

        Page.AddItem(BuildTable);
    }

    /// <summary>
    /// 异步刷新页面。
    /// </summary>
    /// <returns></returns>
    public override Task RefreshAsync() => Table.RefreshAsync();

    private void BuildTree(RenderTreeBuilder builder) => builder.Tree(Tree);
    private void BuildTable(RenderTreeBuilder builder) => builder.Table(Table);

    private Task<PagingResult<SysUser>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(SysUser.OrgNo)] = currentOrg?.Id;
        return Admin.QueryUsersAsync(criteria);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Admin.SaveUserAsync, new SysUser { OrgNo = currentOrg?.Id });

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(SysUser row) => Table.EditForm(Admin.SaveUserAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysUser row) => Table.Delete(Admin.DeleteUsersAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Admin.DeleteUsersAsync);

    /// <summary>
    /// 批量重置用户默认密码。
    /// </summary>
    public void ResetPassword() => Table.SelectRows(Admin.SetUserPwdsAsync, Language.Reset);

    /// <summary>
    /// 批量切换用户所属部门。
    /// </summary>
    public void ChangeDepartment() => Table.SelectRows(OnChangeDepartment);

    /// <summary>
    /// 批量启用用户。
    /// </summary>
    public void Enable() => Table.SelectRows(Admin.EnableUsersAsync, Language.Enable);

    /// <summary>
    /// 批量禁用用户。
    /// </summary>
    public void Disable() => Table.SelectRows(Admin.DisableUsersAsync, Language.Disable);

    /// <summary>
    /// 弹出数据导入对话框。
    /// </summary>
    public Task Import() => Table.ShowImportAsync();

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public Task Export() => Table.ExportDataAsync();

    private void OnChangeDepartment(List<SysUser> rows)
    {
        SysOrganization node = null;
        var model = new DialogModel
        {
            Title = Language["Title.ChangeDepartment"],
            Content = builder =>
            {
                builder.Tree(new TreeModel
                {
                    ExpandRoot = true,
                    Data = orgs.ToMenuItems(),
                    OnNodeClick = n =>
                    {
                        node = n.Data as SysOrganization;
                        return Task.CompletedTask;
                    }
                });
            }
        };
        model.OnOk = async () =>
        {
            if (node == null)
            {
                UI.Error(Language["Tip.SelectChangeOrganization"]);
                return;
            }

            rows.ForEach(m => m.OrgNo = node.Id);
            var result = await Admin.ChangeDepartmentAsync(rows);
            UI.Result(result, async () =>
            {
                await model.CloseAsync();
                await Table.RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        currentOrg = item.Data as SysOrganization;
        await Table.RefreshAsync();
    }
}

class UserForm : BaseForm<SysUser>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Model.Initialize();
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var user = await Admin.GetUserDataAsync(Model.Data.Id);
            Model.Data.RoleIds = user.RoleIds;
            Model.Codes["Roles"] = user.Roles;
        }
    }
}