namespace Known.Pages;

/// <summary>
/// 系统用户管理页面组件类。
/// </summary>
[Route("/sys/users")]
[Menu(Constants.System, "用户管理", "user", 3)]
public class SysUserList : BaseTablePage<UserDataInfo>
{
    private List<OrganizationInfo> orgs;
    private OrganizationInfo currentOrg;
    private TreeModel Tree;
    private bool HasOrg => orgs != null && orgs.Count > 1;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync
        };

        Table = new TableModel<UserDataInfo>(this)
        {
            FormType = UIConfig.UserFormTabs.Count > 0 ? typeof(UserTabForm) : typeof(UserForm),
            Form = new FormInfo { Width = 800, SmallLabel = true, ShowFooter = UIConfig.UserFormShowFooter },
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync
        };
        Table.Column(c => c.Gender).Tag();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            orgs = await Admin.GetOrganizationsAsync();
            if (HasOrg)
            {
                currentOrg = orgs.FirstOrDefault(o => o.ParentId == "0");
                Tree.Data = orgs.ToMenuItems();
                Tree.SelectedKeys = [currentOrg.Id];
                await StateChangedAsync();
            }
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (HasOrg)
        {
            builder.Component<KTreeTable<UserDataInfo>>()
                   .Set(c => c.Tree, Tree)
                   .Set(c => c.Table, Table)
                   .Build();
        }
        else
        {
            base.BuildPage(builder);
        }
    }

    private Task<PagingResult<UserDataInfo>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(UserInfo.OrgNo)] = currentOrg.Id;
        return Admin.QueryUserDatasAsync(criteria);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    [Action] public void New() => Table.NewForm(Admin.SaveUserAsync, new UserDataInfo { OrgNo = currentOrg?.Id });

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Edit(UserDataInfo row) => Table.EditForm(Admin.SaveUserAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(UserDataInfo row) => Table.Delete(Admin.DeleteUsersAsync, row);

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

    private void OnChangeDepartment(List<UserDataInfo> rows)
    {
        OrganizationInfo node = null;
        var model = new DialogModel
        {
            Title = Language.ChangeDepartment,
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
                UI.Error(Language.TipSelectChangeOrganization);
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

class UserTabForm : BaseTabForm
{
    /// <summary>
    /// 取得或设置泛型表单组件模型实例。
    /// </summary>
    [Parameter] public FormModel<UserDataInfo> Model { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        Tab.AddTab(Language.BasicInfo, b => b.Component<UserForm>().Set(c => c.Model, Model).Build());
        foreach (var item in UIConfig.UserFormTabs.OrderBy(t => t.Value.Id))
        {
            if (item.Value.Parameters == null)
                item.Value.Parameters = [];
            item.Value.Parameters[nameof(UserFormTab.IsView)] = Model.IsView;
            item.Value.Parameters[nameof(UserFormTab.User)] = Model.Data;
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }
}

class UserForm : BaseForm<UserDataInfo>
{
    private string defaultPassword;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        if (!UIConfig.UserFormShowFooter)
        {
            SaveClose = UIConfig.UserFormTabs.Count == 0;
            ShowAction = UIConfig.UserFormTabs.Count > 0;
        }
        //Model.Header = b => b.Alert();
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds, c => c.Type = FieldType.CheckList);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var user = await Admin.GetUserDataAsync(Model.Data.Id);
            defaultPassword = user.DefaultPassword;
            if (Model.IsNew)
                Model.Data.Password = defaultPassword;
            var pwdTips = Language[Language.TipUserDefaultPwd].Replace("{password}", defaultPassword);
            Model.Field(f => f.Password).Tooltip(pwdTips);
            Model.Data.RoleIds = user.RoleIds;
            Model.Codes["Roles"] = user.Roles;
        }
    }
}