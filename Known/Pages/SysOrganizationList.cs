namespace Known.Pages;

/// <summary>
/// 组织架构页面组件类。
/// </summary>
[Route("/sys/organizations")]
[Menu(Constants.BaseData, "组织架构", "partition", 3)]
//[PagePlugin("组织架构", "partition", PagePluginType.Module, Language.BaseData, Sort = 3)]
public class SysOrganizationList : BaseTablePage<SysOrganization>
{
    private IOrganizationService Service;
    private MenuInfo current;
    private TreeModel Tree;

    /// <inheritdoc />
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

        Table = new TableModel<SysOrganization>(this)
        {
            FormTitle = row => $"{PageName} - {row.ParentName}",
            Form = new FormInfo { SmallLabel = true },
            RowKey = r => r.Id,
            EnableFilter = false,
            ShowPager = false,
            OnQuery = OnQueryOrganizationsAsync
        };
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KTreeTable<SysOrganization>>()
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
    /// 新增组织架构。
    /// </summary>
    [Action]
    public void New()
    {
        if (current == null)
        {
            UI.Error(Language.TipSelectParentOrganization);
            return;
        }

        Table.NewForm(Service.SaveOrganizationAsync, new SysOrganization { ParentId = current?.Id, ParentName = current?.Name });
    }

    /// <summary>
    /// 编辑组织架构。
    /// </summary>
    /// <param name="row">组织架构信息。</param>
    [Action] public void Edit(SysOrganization row) => Table.EditForm(Service.SaveOrganizationAsync, row);

    /// <summary>
    /// 删除组织架构。
    /// </summary>
    /// <param name="row">组织架构信息。</param>
    [Action] public void Delete(SysOrganization row) => Table.Delete(Service.DeleteOrganizationsAsync, row);

    /// <summary>
    /// 批量删除组织架构。
    /// </summary>
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

    private Task<PagingResult<SysOrganization>> OnQueryOrganizationsAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => c.DataAs<SysOrganization>()).ToList();
        var result = new PagingResult<SysOrganization>(data);
        return Task.FromResult(result);
    }
}