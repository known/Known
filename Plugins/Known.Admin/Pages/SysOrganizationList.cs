namespace Known.Pages;

[Route("/sys/organizations")]
[Menu(Constants.BaseData, "组织架构", "partition", 3)]
//[PagePlugin("组织架构", "partition", PagePluginType.Module, AdminLanguage.BaseData, Sort = 3)]
public class SysOrganizationList : BaseTablePage<OrganizationInfo>
{
    private IOrganizationService Service;
    private MenuInfo current;
    private TreeModel Tree;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IOrganizationService>();

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync,
            OnModelChanged = OnTreeModelChangedAsync
        };

        Table = new TableModel<OrganizationInfo>(this)
        {
            FormTitle = row => $"{PageName} - {row.ParentName}",
            Form = new FormInfo { SmallLabel = true },
            RowKey = r => r.Id,
            EnableFilter = false,
            ShowPager = false,
            OnQuery = OnQueryOrganizationsAsync
        };
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KTreeTable<OrganizationInfo>>()
               .Set(c => c.Tree, Tree)
               .Set(c => c.Table, Table)
               .Build();
    }

    public override async Task RefreshAsync()
    {
        await Tree.RefreshAsync();
        await Table.RefreshAsync();
    }

    [Action]
    public void New()
    {
        if (current == null)
        {
            UI.Error(AdminLanguage.TipSelectParentOrganization);
            return;
        }

        Table.NewForm(Service.SaveOrganizationAsync, new OrganizationInfo { ParentId = current?.Id, ParentName = current?.Name });
    }

    [Action] public void Edit(OrganizationInfo row) => Table.EditForm(Service.SaveOrganizationAsync, row);
    [Action] public void Delete(OrganizationInfo row) => Table.Delete(Service.DeleteOrganizationsAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteOrganizationsAsync);

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        await Table.RefreshAsync();
        await StateChangedAsync();
    }

    private async Task<TreeModel> OnTreeModelChangedAsync()
    {
        var datas = await Service.GetOrganizationsAsync();
        if (datas != null && datas.Count > 0)
        {
            Tree.Data = datas.ToMenuItems(ref current);
            Tree.SelectedKeys = [current.Id];
            await OnNodeClickAsync(current);
        }
        return Tree;
    }

    private Task<PagingResult<OrganizationInfo>> OnQueryOrganizationsAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (OrganizationInfo)c.Data).ToList();
        var result = new PagingResult<OrganizationInfo>(data);
        return Task.FromResult(result);
    }
}