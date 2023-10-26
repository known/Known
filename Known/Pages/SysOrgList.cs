namespace Known.Pages;

[Route("/organizations")]
public class SysOrgList : KDataGrid<SysOrganization, SysOrgForm>
{
    private readonly List<KTreeItem<SysOrganization>> data = new();
    private KTreeItem<SysOrganization> current;
    private List<SysOrganization> datas;

    public SysOrgList()
    {
        IsSort = false;
        ShowPager = false;
    }

    protected override async Task InitPageAsync()
    {
        Column(c => c.Code).Template((b, r) => b.Link(r.Code, Callback(() => View(r))));

        datas = await Platform.Company.GetOrganizationsAsync();
        InitTreeNode();
        await base.InitPageAsync();
    }

    protected override Task<PagingResult<SysOrganization>> OnQueryDataAsync(PagingCriteria criteria)
    {
        var result = new PagingResult<SysOrganization> { PageData = Data };
        return Task.FromResult(result);
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.ViewLR(left =>
        {
            builder.Component<KTree<SysOrganization>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<KTreeItem<SysOrganization>>(OnTreeItemClick))
                   .Build();
        }, right => base.BuildPage(builder));
    }

    public void New() => ShowForm();
    public void DeleteM() => SelectRows(OnDeleteData);
    public void Edit(SysOrganization row) => ShowForm(row);
    public void Delete(SysOrganization row) => OnDeleteData(new List<SysOrganization> { row });

    public override void View(SysOrganization row)
    {
        row.ParentName = current.Value.FullName;
        base.View(row);
    }

    protected override void ShowForm(SysOrganization model = null)
    {
        if (current == null)
        {
            UI.Toast("请先选择上级组织架构！");
            return;
        }

        model ??= new SysOrganization { ParentId = current.Value.Id };
        model.ParentName = current.Value.FullName;
        var action = model.IsNew ? "新增" : "编辑";
        UI.ShowForm<SysOrgForm>($"{action}{Name}", model, result =>
        {
            UI.CloseDialog();
            var data = result.DataAs<SysOrganization>();
            if (!model.IsNew)
                datas.Remove(model);
            datas.Add(data);
            OnDataChanged();
        });
    }

    private void OnDeleteData(List<SysOrganization> models)
    {
        UI.Confirm("确定要删除？", async () =>
        {
            var result = await Platform.Company.DeleteOrganizationsAsync(models);
            UI.Result(result, () =>
            {
                foreach (var item in models)
                {
                    datas.Remove(item);
                }
                OnDataChanged();
            });
        });
    }

    private void OnTreeItemClick(KTreeItem<SysOrganization> item)
    {
        current = item;
        RefreshGridData();
    }

    private void OnDataChanged()
    {
        RefreshTreeNode(current);
        RefreshGridData();
        StateChanged();
    }

    private void RefreshGridData() => Data = datas.Where(m => m.ParentId == current.Value.Id).ToList();

    private void InitTreeNode()
    {
        data.Clear();
        var org = datas.FirstOrDefault(d => d.ParentId == "0");
        var root = new KTreeItem<SysOrganization> { Value = org, Text = org.Name, IsExpanded = true };
        data.Add(root);
        current = root;
        RefreshTreeNode(root);
        RefreshGridData();
    }

    private async void RefreshTreeNode(KTreeItem<SysOrganization> item, bool reload = false)
    {
        if (reload)
            datas = await Platform.Company.GetOrganizationsAsync();

        item.Children.Clear();
        if (datas == null || datas.Count == 0)
            return;

        var children = datas.Where(m => m.ParentId == item.Value.Id).ToList();
        if (children == null || children.Count == 0)
            return;

        foreach (var child in children)
        {
            var sub = new KTreeItem<SysOrganization> { Value = child, Text = child.Name };
            item.AddChild(sub);
            RefreshTreeNode(sub);
        }
    }
}