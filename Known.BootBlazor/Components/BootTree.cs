namespace Known.BootBlazor.Components;

class BootTree : Tree
{
    [Parameter] public TreeModel Model { get; set; }

    protected override void OnInitialized()
    {
        ShowIcon = true;
        ShowCheckbox = Model.Checkable;
        Items = ToTreeItems(Model.Data);
        OnTreeItemClick = OnTreeClick;
        OnTreeItemChecked = OnTreeCheck;
        base.OnInitialized();
    }

    private List<TreeItem> ToTreeItems(List<MenuInfo> items)
    {
        var nodes = new List<TreeItem>();
        foreach (var item in items)
        {
            AddTreeItem(nodes, item, !Model.ExpandRoot);
        }
        return nodes;
    }

    private void AddTreeItem(List<TreeItem> nodes, MenuInfo item, bool isCollapsed = true)
    {
        var node = new TreeItem
        {
            Id = item.Id,
            Text = item.Name,
            Icon = item.Icon,
            Tag = item,
            IsCollapsed = isCollapsed,
            Items = []
        };
        if (item.Children.Count > 0)
        {
            foreach (var sub in item.Children)
            {
                AddTreeItem(node.Items, sub);
            }
        }
        nodes.Add(node);
    }

    private Task OnTreeClick(TreeItem node)
    {
        var item = node.Tag as MenuInfo;
        item.Checked = node.Checked;
        Model.OnNodeClick?.Invoke(item);
        return Task.CompletedTask;
    }

    private Task OnTreeCheck(List<TreeItem> nodes)
    {
        foreach (var node in nodes)
        {
            var item = node.Tag as MenuInfo;
            item.Checked = node.Checked;
            Model.OnNodeCheck?.Invoke(item);
        }
        return Task.CompletedTask;
    }
}