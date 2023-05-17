using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysModuleList : DataGrid<SysModule, SysModuleForm>
{
    private readonly List<TreeItem<string>> data = new();
    private TreeItem<string> current;
    private List<SysModule> modules;

    public SysModuleList()
    {
        Name = string.Empty;
        Style = "sysModule";
        IsSort = false;
        ShowPager = false;
    }

    protected override async Task OnInitializedAsync()
    {
        modules = await Platform.Module.GetModulesAsync();
        InitTreeNode();
        await base.OnInitializedAsync();
    }

    protected override Task<PagingResult<SysModule>> OnQueryData(PagingCriteria criteria)
    {
        var result = new PagingResult<SysModule> { PageData = Data };
        return Task.FromResult(result);
    }

    protected override void FormatColumns()
    {
        Column(c => c.Name).Template((b, r) => b.IconName(r.Icon, r.Name));
    }

    protected override void BuildOther(RenderTreeBuilder builder)
    {
        builder.Div("left-view", attr =>
        {
            builder.Component<Tree<string>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<TreeItem<string>>(OnTreeItemClick))
                   .Build();
        });
    }

    public void New() => ShowForm();
    public void Copy() => SelectItems(OnCopy);
    public void DeleteM() => SelectItems(OnDeleteData);
    public void Move() => SelectItems(OnMove);
    public void Edit(SysModule row) => ShowForm(row);
    public void Delete(SysModule row) => OnDeleteData(new List<SysModule> { row });
    public void MoveUp(SysModule row) => OnMove(row, true);
    public void MoveDown(SysModule row) => OnMove(row, false);

    private void OnCopy(List<SysModule> models)
    {
        TreeItem<string> node = null;
        UI.Prompt("复制到", new(300, 300), builder =>
        {
            builder.Component<Tree<string>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<TreeItem<string>>(n => node = n))
                   .Build();
        }, async model =>
        {
            models.ForEach(m => m.ParentId = node.Value);
            var result = await Platform.Module.CopyModulesAsync(models);
            UI.Result(result, () =>
            {
                RefreshTreeNode(node, true);
                RefreshData();
                StateChanged();
                UI.CloseDialog();
            });
        });
    }

    private void OnDeleteData(List<SysModule> models)
    {
        UI.Confirm("确定要删除？", async () =>
        {
            var result = await Platform.Module.DeleteModulesAsync(models);
            UI.Result(result, () =>
            {
                foreach (var item in models)
                {
                    modules.Remove(item);
                }
                OnDataChanged();
            });
        });
    }

    private void OnMove(List<SysModule> models)
    {
        TreeItem<string> node = null;
        UI.Prompt("移动到", new(300, 300), builder =>
        {
            builder.Component<Tree<string>>()
                   .Set(c => c.Data, data)
                   .Set(c => c.OnItemClick, Callback<TreeItem<string>>(n => node = n))
                   .Build();
        }, async model =>
        {
            models.ForEach(m => m.ParentId = node.Value);
            var result = await Platform.Module.MoveModulesAsync(models);
            UI.Result(result, () =>
            {
                RefreshTreeNode(node, true);
                RefreshData();
                StateChanged();
                UI.CloseDialog();
            });
        });
    }

    private void OnMove(SysModule model, bool isMoveUp)
    {
        model.IsMoveUp = isMoveUp;
        MoveRow(model, isMoveUp, Platform.Module.MoveModuleAsync, (item1, item2) =>
        {
            (item2.Sort, item1.Sort) = (item1.Sort, item2.Sort);
            OnDataChanged();
        });
    }

    protected override void ShowForm(SysModule model = null)
    {
        model ??= new SysModule { ParentId = current.Value, Enabled = true, Sort = Data.Count + 1 };
        var action = model.IsNew ? "新增" : "编辑";
        UI.ShowForm<SysModuleForm>($"{action}系统模块", model, result =>
        {
            UI.CloseDialog();
            var data = result.DataAs<SysModule>();
            if (!model.IsNew)
                modules.Remove(model);
            modules.Add(data);
            OnDataChanged();
        });
    }

    private void OnTreeItemClick(TreeItem<string> item)
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

    private void RefreshData() => Data = modules.Where(m => m.ParentId == current.Value).OrderBy(m => m.Sort).ToList();

    private void InitTreeNode()
    {
        data.Clear();
        var root = new TreeItem<string> { Value = "0", Text = Config.AppName, Icon = "fa fa-tv", IsExpanded = true };
        data.Add(root);
        current = root;
        RefreshTreeNode(root);
        RefreshData();
    }

    private async void RefreshTreeNode(TreeItem<string> item, bool reload = false)
    {
        if (reload)
            modules = await Platform.Module.GetModulesAsync();

        item.Children.Clear();
        if (modules == null || modules.Count == 0)
            return;

        var children = modules.Where(m => m.ParentId == item.Value).OrderBy(m => m.Sort).ToList();
        if (children == null || children.Count == 0)
            return;

        foreach (var child in children)
        {
            var sub = new TreeItem<string> { Value = child.Id, Text = child.Name, Icon = child.Icon };
            item.AddChild(sub);
            RefreshTreeNode(sub);
        }
    }
}