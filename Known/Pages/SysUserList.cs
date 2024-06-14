namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/users")]
public class SysUserList : BasePage<SysUser>
{
    private ICompanyService companyService;
    private IUserService userService;
    private List<SysOrganization> orgs;
    private SysOrganization currentOrg;
    private TreeModel tree;
    private TableModel<SysUser> table;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        companyService = await CreateServiceAsync<ICompanyService>();
        userService = await CreateServiceAsync<IUserService>();

        orgs = await companyService.GetOrganizationsAsync();
        var hasOrg = orgs != null && orgs.Count > 1;
        if (hasOrg)
        {
            Page.Type = PageType.Column;
            Page.Spans = "28";

            currentOrg = orgs[0];
            tree = new TreeModel
            {
                ExpandRoot = true,
                Data = orgs.ToMenuItems(),
                OnNodeClick = OnNodeClick,
                SelectedKeys = [currentOrg.Id]
            };

            Page.AddItem("kui-card", BuildTree);
        }

        table = new TableModel<SysUser>(this)
        {
            FormType = typeof(UserForm),
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync
        };
        Page.AddItem(BuildTable);
    }

    protected override async Task OnPageChangeAsync()
    {
        table.Initialize(this);
        await base.OnPageChangeAsync();
        table.Column(c => c.Gender).Template(BuildGender);
    }

    public override Task RefreshAsync() => table.RefreshAsync();

    private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
    private void BuildTable(RenderTreeBuilder builder) => builder.BuildTable(table);

    private Task<PagingResult<SysUser>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(SysUser.OrgNo)] = currentOrg?.Id;
        return userService.QueryUsersAsync(criteria);
    }

    public void New() => table.NewForm(userService.SaveUserAsync, new SysUser { OrgNo = currentOrg?.Id });
    public void Edit(SysUser row) => table.EditForm(userService.SaveUserAsync, row);
    public void Delete(SysUser row) => table.Delete(userService.DeleteUsersAsync, row);
    public void DeleteM() => table.DeleteM(userService.DeleteUsersAsync);
    public void ResetPassword() => table.SelectRows(userService.SetUserPwdsAsync, Language.Reset);
    public void ChangeDepartment() => table.SelectRows(OnChangeDepartment);
    public void Enable() => table.SelectRows(userService.EnableUsersAsync, Language.Enable);
    public void Disable() => table.SelectRows(userService.DisableUsersAsync, Language.Disable);

    private void BuildGender(RenderTreeBuilder builder, SysUser row) => UI.BuildTag(builder, row.Gender);

    private void OnChangeDepartment(List<SysUser> rows)
    {
        SysOrganization node = null;
        var model = new DialogModel
        {
            Title = Language["Title.ChangeDepartment"],
            Content = builder =>
            {
                UI.BuildTree(builder, new TreeModel
                {
                    ExpandRoot = true,
                    Data = orgs.ToMenuItems(),
                    OnNodeClick = n => node = n.Data as SysOrganization
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
            var result = await userService.ChangeDepartmentAsync(rows);
            UI.Result(result, async () =>
            {
                //TODO：更换部门后，部门名称未刷新问题
                await model.CloseAsync();
                await table.RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }

    private async void OnNodeClick(MenuInfo item)
    {
        currentOrg = item.Data as SysOrganization;
        await table.RefreshAsync();
    }
}

class UserForm : BaseForm<SysUser>
{
    private IUserService userService;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        userService = await CreateServiceAsync<IUserService>();

        Model.Initialize();
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var user = await userService.GetUserDataAsync(Model.Data.Id);
            Model.Data.RoleIds = user.RoleIds;
            Model.Codes["Roles"] = user.Roles;
        }
    }
}