namespace Known.Pages;

class UserPage : IUserPage
{
    private IOrganizationService Organize;
    private SysUserList page;
    private TreeModel Tree;
    private MenuInfo current;
    private List<SysOrganization> orgs;
    private bool HasOrg => orgs != null && orgs.Count > 1;

    public async Task OnInitAsync(SysUserList list)
    {
        page = list;
        Organize = await list.CreateServiceAsync<IOrganizationService>();

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync
        };
    }

    public async Task OnAfterRenderAsync()
    {
        orgs = await Organize.GetOrganizationsAsync();
        if (HasOrg)
        {
            Tree.Data = orgs.ToMenuItems(ref current);
            Tree.SelectedKeys = [current.Id];
            await OnNodeClickAsync(current);
        }
    }

    public bool BuildPage(RenderTreeBuilder builder)
    {
        if (!HasOrg)
            return false;

        builder.Component<KTreeTable<SysUser>>()
               .Set(c => c.Tree, Tree)
               .Set(c => c.Table, page.Table)
               .Build();
        return true;
    }

    public void OnChangeDepartment(Func<List<SysUser>, Task<Result>> onChange, List<SysUser> rows)
    {
        SysOrganization node = null;
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
                page.UI.Error(AdminLanguage.TipSelectChangeOrganization);
                return;
            }

            rows.ForEach(m => m.OrgNo = node.Id);
            var result = await onChange.Invoke(rows);
            page.UI.Result(result, async () =>
            {
                await model.CloseAsync();
                await page.RefreshAsync();
            });
        };
        page.UI.ShowDialog(model);
    }

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        var org = item.DataAs<SysOrganization>();
        page.CurrentOrg = org?.Id;
        await page.RefreshAsync();
        await page.StateChangedAsync();
    }
}