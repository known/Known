namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/organizations")]
public class SysOrganizationList : BasePage<SysOrganization>
{
    private ICompanyService companyService;
    private MenuInfo current;
    private TreeModel tree;
    private TableModel<SysOrganization> table;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        companyService = await CreateServiceAsync<ICompanyService>();

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card", BuildTree);
        Page.AddItem(BuildTable);

        tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClick,
            OnModelChanged = OnTreeModelChanged
        };

        table = new TableModel<SysOrganization>(this)
        {
            FormTitle = row => $"{PageName} - {row.ParentName}",
            RowKey = r => r.Id,
            ShowPager = false,
            OnQuery = OnQueryOrganizationsAsync
        };
        table.Initialize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await tree.RefreshAsync();
    }

    public override async Task RefreshAsync()
    {
        await tree.RefreshAsync();
        await table.RefreshAsync();
    }

    private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
    private void BuildTable(RenderTreeBuilder builder) => builder.BuildTable(table);

    private Task<PagingResult<SysOrganization>> OnQueryOrganizationsAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (SysOrganization)c.Data).ToList();
        var result = new PagingResult<SysOrganization>(data);
        return Task.FromResult(result);
    }

    public void New()
    {
        if (current == null)
        {
            UI.Error(Language["Tip.SelectParentOrganization"]);
            return;
        }

        table.NewForm(companyService.SaveOrganizationAsync, new SysOrganization { ParentId = current?.Id, ParentName = current?.Name });
    }

    public void Edit(SysOrganization row) => table.EditForm(companyService.SaveOrganizationAsync, row);
    public void Delete(SysOrganization row) => table.Delete(companyService.DeleteOrganizationsAsync, row);
    public void DeleteM() => table.DeleteM(companyService.DeleteOrganizationsAsync);

    private async void OnNodeClick(MenuInfo item)
    {
        current = item;
        await table.RefreshAsync();
    }

    private async Task<TreeModel> OnTreeModelChanged()
    {
        var datas = await companyService.GetOrganizationsAsync();
        if (datas != null && datas.Count > 0)
        {
            tree.Data = datas.ToMenuItems(ref current);
            tree.SelectedKeys = [current.Id];
            await table.RefreshAsync();
        }
        return tree;
    }
}