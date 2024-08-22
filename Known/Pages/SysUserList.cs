namespace Known.Pages;

[StreamRendering]
[Route("/sys/users")]
public class SysUserList : BasePage<SysUser>
{
    private ICompanyService Company;
    private IUserService Service;
    private List<SysOrganization> orgs;
    private SysOrganization currentOrg;
    private TreeModel Tree;
    private TableModel<SysUser> Table;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Company = await CreateServiceAsync<ICompanyService>();
        Service = await CreateServiceAsync<IUserService>();

        orgs = await Company.GetOrganizationsAsync();
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
                OnNodeClick = OnNodeClick,
                SelectedKeys = [currentOrg.Id]
            };

            Page.AddItem("kui-card", BuildTree);
        }

        Table = new TableModel<SysUser>(this)
        {
            FormType = typeof(UserForm),
            RowKey = r => r.Id,
            OnQuery = OnQueryUsersAsync
        };
        Table.Toolbar.ShowCount = 6;
        Table.Initialize(this);
        Table.Column(c => c.Gender).Template((b, r) => b.Tag(r.Gender));

        Page.AddItem(BuildTable);
    }

    public override Task RefreshAsync() => Table.RefreshAsync();

    private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, Tree));
    private void BuildTable(RenderTreeBuilder builder) => builder.Table(Table);

    private Task<PagingResult<SysUser>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(SysUser.OrgNo)] = currentOrg?.Id;
        return Service.QueryUsersAsync(criteria);
    }

    public void New() => Table.NewForm(Service.SaveUserAsync, new SysUser { OrgNo = currentOrg?.Id });
    public void Edit(SysUser row) => Table.EditForm(Service.SaveUserAsync, row);
    public void Delete(SysUser row) => Table.Delete(Service.DeleteUsersAsync, row);
    public void DeleteM() => Table.DeleteM(Service.DeleteUsersAsync);
    public void ResetPassword() => Table.SelectRows(Service.SetUserPwdsAsync, Language.Reset);
    public void ChangeDepartment() => Table.SelectRows(OnChangeDepartment);
    public void Enable() => Table.SelectRows(Service.EnableUsersAsync, Language.Enable);
    public void Disable() => Table.SelectRows(Service.DisableUsersAsync, Language.Disable);
    public async void Import() => await Table.ShowImportFormAsync();
    public async void Export() => await Table.ExportDataAsync();

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
            var result = await Service.ChangeDepartmentAsync(rows);
            UI.Result(result, async () =>
            {
                await model.CloseAsync();
                await Table.RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }

    private async void OnNodeClick(MenuInfo item)
    {
        currentOrg = item.Data as SysOrganization;
        await Table.RefreshAsync();
    }
}

class UserForm : BaseForm<SysUser>
{
    private IUserService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IUserService>();

        Model.Initialize();
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var user = await Service.GetUserDataAsync(Model.Data.Id);
            Model.Data.RoleIds = user.RoleIds;
            Model.Codes["Roles"] = user.Roles;
        }
    }
}