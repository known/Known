namespace Known.Pages;

[Route("/sys/users")]
//[Menu(Constants.System, "用户管理", "user", 3)]
[PagePlugin("用户管理", "user", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 6)]
public class SysUserList : BaseTablePage<UserDataInfo>
{
    private IOrganizationService Organize;
    private List<OrganizationInfo> orgs;
    private MenuInfo current;
    private OrganizationInfo currentOrg;
    private TreeModel Tree;
    private bool HasOrg => orgs != null && orgs.Count > 1;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Organize = await CreateServiceAsync<IOrganizationService>();

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
        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.Column(c => c.Gender).Tag();
    }

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
        }
    }

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

    [Action] public void New() => Table.NewForm(Admin.SaveUserAsync, new UserDataInfo { OrgNo = currentOrg?.Id });
    [Action] public void Edit(UserDataInfo row) => Table.EditForm(Admin.SaveUserAsync, row);
    [Action] public void Delete(UserDataInfo row) => Table.Delete(Admin.DeleteUsersAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteUsersAsync);
    [Action] public void ResetPassword() => Table.SelectRows(Admin.SetUserPwdsAsync, Language.Reset);
    [Action] public void ChangeDepartment() => Table.SelectRows(OnChangeDepartment);
    [Action] public void Enable() => Table.SelectRows(Admin.EnableUsersAsync, Language.Enable);
    [Action] public void Disable() => Table.SelectRows(Admin.DisableUsersAsync, Language.Disable);
    [Action] public Task Import() => Table.ShowImportAsync();
    [Action] public Task Export() => Table.ExportDataAsync();

    private void OnChangeDepartment(List<UserDataInfo> rows)
    {
        OrganizationInfo node = null;
        var model = new DialogModel
        {
            Title = AdminLanguage.ChangeDepartment,
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
                UI.Error(AdminLanguage.TipSelectChangeOrganization);
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
        current = item;
        currentOrg = item.Data as OrganizationInfo;
        await Table.RefreshAsync();
        await StateChangedAsync();
    }
}

class UserTabForm : BaseTabForm
{
    [Parameter] public FormModel<UserDataInfo> Model { get; set; }

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
            var pwdTips = Language[AdminLanguage.TipUserDefaultPwd].Replace("{password}", defaultPassword);
            Model.Field(f => f.Password).Tooltip(pwdTips);
            Model.Data.RoleIds = user.RoleIds;
            Model.Codes["Roles"] = user.Roles;
        }
    }
}