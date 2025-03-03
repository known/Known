namespace Known.Pages;

/// <summary>
/// 组织架构模块页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/organizations")]
[Menu(Constants.BaseData, "组织架构", "partition", 3)]
public class SysOrganizationList : BaseTablePage<OrganizationInfo>
{
    private MenuInfo current;
    private TreeModel Tree;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Tree = new TreeModel
        {
            ExpandRoot = true,
            OnNodeClick = OnNodeClickAsync,
            OnModelChanged = OnTreeModelChangedAsync
        };

        Table = new TableModel<OrganizationInfo>(this)
        {
            FormTitle = row => $"{PageName} - {row.ParentName}",
            RowKey = r => r.Id,
            ShowPager = false,
            OnQuery = OnQueryOrganizationsAsync
        };
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KTreeTable<OrganizationInfo>>()
               .Set(c => c.Tree, Tree)
               .Set(c => c.Table, Table)
               .Build();
    }

    /// <inheritdoc />
    public override async Task RefreshAsync()
    {
        await Tree.RefreshAsync();
        await Table.RefreshAsync();
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    [Action]
    public void New()
    {
        if (current == null)
        {
            UI.Error(Language["Tip.SelectParentOrganization"]);
            return;
        }

        Table.NewForm(Admin.SaveOrganizationAsync, new OrganizationInfo { ParentId = current?.Id, ParentName = current?.Name });
    }

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Edit(OrganizationInfo row) => Table.EditForm(Admin.SaveOrganizationAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(OrganizationInfo row) => Table.Delete(Admin.DeleteOrganizationsAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteOrganizationsAsync);

    private async Task OnNodeClickAsync(MenuInfo item)
    {
        current = item;
        await Table.RefreshAsync();
    }

    private async Task<TreeModel> OnTreeModelChangedAsync()
    {
        var datas = await Admin.GetOrganizationsAsync();
        if (datas != null && datas.Count > 0)
        {
            Tree.Data = datas.ToMenuItems(ref current);
            Tree.SelectedKeys = [current.Id];
            await Table.RefreshAsync();
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