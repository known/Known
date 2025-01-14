namespace Known.Pages;

/// <summary>
/// 系统用户管理页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/users")]
[Menu(Constants.System, "用户管理", "user", 3)]
public class SysUserList : BaseTablePage<UserInfo>
{
    private List<OrganizationInfo> orgs;
    private OrganizationInfo currentOrg;
    private TreeModel Tree;
    private bool HasOrg => orgs != null && orgs.Count > 1;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        orgs = await Admin.GetOrganizationsAsync();
        if (HasOrg)
        {
            currentOrg = orgs.FirstOrDefault(o => o.ParentId == "0");
            Tree = new TreeModel
            {
                ExpandRoot = true,
                Data = orgs.ToMenuItems(),
                OnNodeClick = OnNodeClickAsync,
                SelectedKeys = [currentOrg.Id]
            };
        }

        Table = new TableModel<UserInfo>(this)
        {
            FormType = typeof(UserForm),
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync,
            Form = new FormInfo { Width = 800 }
        };
        Table.Column(c => c.Gender).Template((b, r) => b.Tag(r.Gender));
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (HasOrg)
        {
            builder.Div("kui-row-28", () =>
            {
                builder.Div("kui-card kui-p10", () => builder.Tree(Tree));
                base.BuildPage(builder);
            });
        }
        else
        {
            base.BuildPage(builder);
        }
    }

    /// <inheritdoc />
    public override Task RefreshAsync() => Table.RefreshAsync();

    private Task<PagingResult<UserInfo>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(UserInfo.OrgNo)] = currentOrg?.Id;
        return Admin.QueryUsersAsync(criteria);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    [Action] public void New() => Table.NewForm(Admin.SaveUserAsync, new UserInfo { OrgNo = currentOrg?.Id });

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Edit(UserInfo row) => Table.EditForm(Admin.SaveUserAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(UserInfo row) => Table.Delete(Admin.DeleteUsersAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteUsersAsync);

    /// <summary>
    /// 批量重置用户默认密码。
    /// </summary>
    [Action] public void ResetPassword() => Table.SelectRows(Admin.SetUserPwdsAsync, Language.Reset);

    /// <summary>
    /// 批量切换用户所属部门。
    /// </summary>
    [Action] public void ChangeDepartment() => Table.SelectRows(OnChangeDepartment);

    /// <summary>
    /// 批量启用用户。
    /// </summary>
    [Action] public void Enable() => Table.SelectRows(Admin.EnableUsersAsync, Language.Enable);

    /// <summary>
    /// 批量禁用用户。
    /// </summary>
    [Action] public void Disable() => Table.SelectRows(Admin.DisableUsersAsync, Language.Disable);

    /// <summary>
    /// 弹出数据导入对话框。
    /// </summary>
    [Action] public Task Import() => Table.ShowImportAsync();

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    [Action] public Task Export() => Table.ExportDataAsync();

    private void OnChangeDepartment(List<UserInfo> rows)
    {
        OrganizationInfo node = null;
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
                        node = n.Data as OrganizationInfo;
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
        currentOrg = item.Data as OrganizationInfo;
        await Table.RefreshAsync();
    }
}

class UserForm : BaseForm<UserInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Model.Initialize();
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds, c => c.Type = FieldType.CheckList);
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