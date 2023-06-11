using System.Reflection;
using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysOrgList : DataGrid<SysOrganization, SysOrgForm>
{
    private readonly List<TreeItem<SysOrganization>> data = new();
    private TreeItem<SysOrganization> current;
    private List<SysOrganization> datas;

    public SysOrgList()
    {
        Style = "left-tree";
        IsSort = false;
        ShowPager = false;
    }

    protected override async Task OnInitializedAsync()
    {
        datas = await Platform.Company.GetOrganizationsAsync();
        InitTreeNode();
        await base.OnInitializedAsync();
    }

    protected override Task<PagingResult<SysOrganization>> OnQueryData(PagingCriteria criteria)
    {
        var result = new PagingResult<SysOrganization> { PageData = Data };
        return Task.FromResult(result);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("left-view", attr =>
        {
            builder.Component<Tree<SysOrganization>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<TreeItem<SysOrganization>>(OnTreeItemClick))
                   .Build();
        });
        base.BuildRenderTree(builder);
    }

    public void New() => ShowForm();
    public void DeleteM() => SelectItems(OnDeleteData);
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
            UI.Tips("请先选择上级组织架构！");
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

    private void OnTreeItemClick(TreeItem<SysOrganization> item)
    {
        current = item;
        RefreshData();
    }

    private void OnDataChanged()
    {
        RefreshTreeNode(current);
        RefreshData();
        StateChanged();
    }

    private void RefreshData() => Data = datas.Where(m => m.ParentId == current.Value.Id).ToList();

    private void InitTreeNode()
    {
        data.Clear();
        var org = datas.FirstOrDefault(d => d.ParentId == "0");
        var root = new TreeItem<SysOrganization> { Value = org, Text = org.Name, IsExpanded = true };
        data.Add(root);
        current = root;
        RefreshTreeNode(root);
        RefreshData();
    }

    private async void RefreshTreeNode(TreeItem<SysOrganization> item, bool reload = false)
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
            var sub = new TreeItem<SysOrganization> { Value = child, Text = child.Name };
            item.AddChild(sub);
            RefreshTreeNode(sub);
        }
    }
}