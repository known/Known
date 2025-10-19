namespace Known.Pages;

/// <summary>
/// 用户管理页面组件类。
/// </summary>
[Route("/sys/users")]
[Menu(Constants.System, "用户管理", "user", 3)]
//[PagePlugin("用户管理", "user", PagePluginType.Module, Language.SystemManage, Sort = 6)]
public class SysUserList : BaseTablePage<SysUser>
{
    private IUserService Service;
    private IOrganizationService Organize;
    private TreeModel Tree;
    private MenuInfo current;
    private List<SysOrganization> orgs;
    private bool HasOrg => orgs != null && orgs.Count > 1;
    private string CurrentOrg { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IUserService>();
        Organize = await CreateServiceAsync<IOrganizationService>();

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync
        };

        Table = new TableModel<SysUser>(this)
        {
            FormType = UIConfig.UserFormTabs.Count > 0 ? typeof(UserTabForm) : typeof(UserForm),
            Form = new FormInfo { Width = 800, SmallLabel = true, ShowFooter = UIConfig.UserFormShowFooter },
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync
        };
        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.Column(c => c.Gender).Tag();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            orgs = await Organize.GetOrganizationsAsync();
            if (HasOrg)
            {
                Tree.Data = orgs.ToMenuItems(ref current);
                Tree.SelectedKeys = [current.Id];
                await OnNodeClickAsync(current);
            }
            else
            {
                var changeDpt = Table.Toolbar.Items.FirstOrDefault(d => d.Id == nameof(ChangeDepartment));
                if (changeDpt != null)
                {
                    Table.Toolbar.Items.Remove(changeDpt);
                    await StateChangedAsync();
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (HasOrg)
            builder.Component<KTreeTable<SysUser>>().Set(c => c.Tree, Tree).Set(c => c.Table, Table).Build();
        else
            base.BuildPage(builder);
    }

    private Task<PagingResult<SysUser>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(SysUser.OrgNo)] = CurrentOrg;
        return Service.QueryUserDatasAsync(criteria);
    }

    /// <summary>
    /// 新增用户。
    /// </summary>
    [Action] public void New() => Table.NewForm(Service.SaveUserAsync, new SysUser { OrgNo = CurrentOrg });

    /// <summary>
    /// 编辑用户。
    /// </summary>
    /// <param name="row">用户信息。</param>
    [Action] public void Edit(SysUser row) => Table.EditForm(Service.SaveUserAsync, row);

    /// <summary>
    /// 删除用户。
    /// </summary>
    /// <param name="row">用户信息。</param>
    [Action] public void Delete(SysUser row) => Table.Delete(Service.DeleteUsersAsync, row);

    /// <summary>
    /// 批量删除用户。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteUsersAsync);

    /// <summary>
    /// 重置默认密码。
    /// </summary>
    [Action] public void ResetPassword() => Table.SelectRows(Service.SetUserPwdsAsync, Language.Reset);

    /// <summary>
    /// 改变用户部门。
    /// </summary>
    [Action] public void ChangeDepartment() => Table.SelectRows(rows => OnChangeDepartment(Service.ChangeDepartmentAsync, rows));

    /// <summary>
    /// 启用用户。
    /// </summary>
    [Action] public void Enable() => Table.SelectRows(Service.EnableUsersAsync, Language.Enable);

    /// <summary>
    /// 禁用用户。
    /// </summary>
    [Action] public void Disable() => Table.SelectRows(Service.DisableUsersAsync, Language.Disable);

    /// <summary>
    /// 导入用户。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Import() => Table.ShowImportAsync();

    /// <summary>
    /// 导出用户。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Export() => Table.ExportDataAsync();

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        var org = item.DataAs<SysOrganization>();
        CurrentOrg = org?.Id;
        await RefreshAsync();
        await StateChangedAsync();
    }

    private void OnChangeDepartment(Func<List<SysUser>, Task<Result>> onChange, List<SysUser> rows)
    {
        SysOrganization node = null;
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
                        node = n.DataAs<SysOrganization>();
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
            var result = await onChange.Invoke(rows);
            UI.Result(result, async () =>
            {
                await model.CloseAsync();
                await RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }
}

class UserTabForm : BaseTabForm
{
    [Parameter] public FormModel<SysUser> Model { get; set; }

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

class UserForm : BaseForm<SysUser>
{
    private IUserService Service;
    private string defaultPassword;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IUserService>();

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
            var user = await Service.GetUserDataAsync(Model.Data.Id);
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