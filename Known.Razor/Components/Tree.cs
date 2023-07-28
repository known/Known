namespace Known.Razor.Components;

public class Tree<TValue> : BaseComponent
{
    private TreeItem<TValue> CurrentSelected;
    private IEnumerable<TreeItem<TValue>> dataView = new List<TreeItem<TValue>>();
    private readonly HashSet<TreeItem<TValue>> SelectedList = new();

    [Parameter] public bool IsAccordion { get; set; }
    [Parameter] public bool ExpandOnClickNode { get; set; } = true;
    [Parameter] public bool CheckOnClickNode { get; set; }
    [Parameter] public bool IsIsolated { get; set; }
    [Parameter] public IEnumerable<TreeItem<TValue>> Data { get; set; }
    [Parameter] public bool ShowCheckBox { get; set; }
    [Parameter] public RenderFragment<TreeItem<TValue>> ItemSlot { get; set; }
    [Parameter] public EventCallback<TreeItem<TValue>> OnItemClick { get; set; }
    [Parameter] public EventCallback<TreeItem<TValue>> OnCheckClick { get; set; }
    [Parameter] public EventCallback<Tree<TValue>> OnChange { get; set; }
    [Parameter] public HashSet<TValue> Values { get; set; }
    [Parameter] public EventCallback<HashSet<TValue>> ValuesChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("tree", attr =>
        {
            builder.Div("tree-root", attr =>
            {
                foreach (var item in dataView)
                {
                    BuildItem(builder, item);
                    if (item.Children.Any())
                    {
                        BuildChildren(builder, item);
                    }
                }
            });
        });
    }

    private void BuildItem(RenderTreeBuilder builder, TreeItem<TValue> node)
    {
        var className = GetItemCls(node);
        builder.Div(className, attr =>
        {
            builder.Span("icon", attr =>
            {
                attr.OnClick(Callback(async e => await ItemClick(node, true)));
                if (node.Children.Any())
                {
                    var icon = node.IsExpanded ? "fa-caret-down" : "fa-caret-right";
                    builder.Icon($"fa {icon}");
                }
            });

            if (ShowCheckBox)
            {
                var cked = GetItemChecked(node);
                builder.Component<TreeCheckBox>()
                       .Set(c => c.Style, GetCheckCls(node))
                       .Set(c => c.ReadOnly, ReadOnly)
                       .Set(c => c.Checked, cked)
                       .Set(c => c.OnChange, Callback<bool>(async v => await Check(node, v)))
                       .Build();
            }
            if (ItemSlot != null)
            {
                builder.Div("flex-grow-1", attr => ItemSlot(node));
            }
            else
            {
                builder.Div("tree-content", attr =>
                {
                    attr.OnClick(Callback(async e => await ItemClick(node, false)));
                    builder.IconName(node.Icon, node.Text);
                });
            }
        });
    }

    private void BuildChildren(RenderTreeBuilder builder, TreeItem<TValue> node)
    {
        var className = GetListCls(node);
        builder.Div(className, attr =>
        {
            foreach (var item in node.Children)
            {
                BuildItem(builder, item);
                if (item.Children.Any())
                {
                    BuildChildren(builder, item);
                }
            }
        });
    }

    private static string GetItemCls(TreeItem<TValue> item)
    {
        return CssBuilder.Default("tree-item")
                         .AddClass("active", item.IsSelected)
                         .AddClass("disabled", item.Disabled)
                         .AddClass("clickable", !item.Disabled)
                         .Build();
    }

    private static string GetListCls(TreeItem<TValue> item)
    {
        return CssBuilder.Default("tree-list")
                         .AddClass("show", item.IsExpanded)
                         .Build();
    }

    private static string GetCheckCls(TreeItem<TValue> item)
    {
        return CssBuilder.Default("cb")
                         .AddClass("active", item.GetHasChildrenChecked())
                         .Build();
    }

    private bool? GetItemChecked(TreeItem<TValue> item)
    {
        if (!IsIsolated && item.Children.Any())
            return item.GetCheckedStatus();

        return item.IsChecked;
    }

    //触发事件
    private async Task Fire()
    {
        Values = new HashSet<TValue>(SelectedList.Select(x => x.Value));
        await ValuesChanged.InvokeAsync(Values);
        await OnChange.InvokeAsync(this);
    }

    //选中值
    private void CheckValues(TreeItem<TValue> item, HashSet<TValue> values, int level)
    {
        item.IsChecked = false;
        if (values.Contains(item.Value))
        {
            item.IsChecked = true;
            if (IsIsolated || item.Children.Count == 0)
            {
                SelectedList.Add(item);
            }
        }

        item.Level = level;
        foreach (var subItem in item.Children)
        {
            CheckValues(subItem, values, level + 1);
        }
    }

    //初始化数据
    private void InitData()
    {
        if (Data == null) return;
        SelectedList.Clear();
        if (Values != null && Values.Any())
        {
            var values = Values;
            foreach (var item in Data)
            {
                CheckValues(item, values, 0);
            }
        }

        dataView = Data;
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        InitData();
    }

    //单击节点
    private async Task ItemClick(TreeItem<TValue> item, bool isIconClick)
    {
        if (item.Disabled)
            return;
        if (CurrentSelected != null)
            CurrentSelected.IsSelected = false;

        CurrentSelected = item;

        //手风琴模式
        if (IsAccordion)
        {
            var list = item.Parent != null ? item.Parent.Children : Data;
            foreach (var treeNode in list)
            {
                if (treeNode != item)
                {
                    treeNode.IsExpanded = false;
                }
            }
        }

        if (isIconClick)
            item.IsExpanded = !item.IsExpanded;

        item.IsSelected = true;

        await OnItemClick.InvokeAsync(item);
    }

    //选中子节点
    private void CheckChildren(TreeItem<TValue> item)
    {
        if (item.Disabled) return;
        if (item.Children.Count == 0)
        {
            item.IsChecked = true;
            SelectedList.Add(item);
        }
        else
        {
            foreach (var subItem in item.Children)
            {
                CheckChildren(subItem);
            }
        }
    }

    //取消选中子节点
    private void UnCheckChildren(TreeItem<TValue> item)
    {
        if (item.Disabled) return;
        item.IsChecked = false;
        SelectedList.Remove(item);

        foreach (var subItem in item.Children)
        {
            UnCheckChildren(subItem);
        }
    }

    //选中CheckBox
    private async Task Check(TreeItem<TValue> item, bool cked)
    {
        if (item.Disabled) return;
        if (IsIsolated || item.Children.Count == 0)
        {
            //直接处理自己
            if (item.IsChecked)
            {
                SelectedList.Remove(item);
                item.IsChecked = false;
            }
            else
            {
                SelectedList.Add(item);
                item.IsChecked = true;
            }
        }
        else
        {
            //级联多选
            if (cked)
                CheckChildren(item);//全选
            else
                UnCheckChildren(item);//取消
        }

        await OnCheckClick.InvokeAsync(item);
        await Fire();
    }

    public async Task ClearSelectedNodes()
    {
        foreach (var item in SelectedList)
        {
            item.IsSelected = false;
            item.IsChecked = false;
        }

        SelectedList.Clear();
        await Fire();
    }

    public HashSet<TreeItem<TValue>> GetSelectedNodes() => SelectedList;

    // public void Remove(TreeItem<TValue> item)
    // {
    //     SelectedList.Remove(item);
    //     if (item.Parent != null)
    //     {
    //         item.Parent.Children.Remove(item);
    //         item.Parent = null;
    //     }
    //     else
    //     {
    //     }
    // }
    // insertBefore
    // insertAfter

    //Expand/collapse 

    public static List<TreeItem<TValue>> BuildDataFromJson(string json)
    {
        var list = Utils.FromJson<List<TreeItem<TValue>>>(json);
        foreach (var item in list)
        {
            item.SetParent();
        }
        return list;
    }
}

class TreeCheckBox : BaseComponent
{
    string Classes => CssBuilder.Default("icon").AddClass("checked", Checked != false).Build();
    string IconClass => Checked == null ? "fa fa-minus-square" : Checked.Value ? "fa fa-check-square" : "fa fa-square-o";

    [Parameter] public bool? Checked { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public EventCallback<bool> OnChange { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("icon-check-radio").AddClass(Style).AddClass(Classes).Build();
        builder.Span(css, attr =>
        {
            if (!ReadOnly)
                attr.OnClick(Callback(async e => await ClickHandle()));
            builder.Icon(IconClass);
            ChildContent?.Invoke(builder);
        });
    }

    private async Task ClickHandle()
    {
        Checked = Checked != true;
        await CheckedChanged.InvokeAsync(Checked.Value);
        await OnChange.InvokeAsync(Checked.Value);
    }
}

public class TreeItem<TValue>
{
    public TValue Value { get; set; }
    public string Text { get; set; }
    public string Icon { get; set; }
    internal bool IsSelected { get; set; }
    public bool IsExpanded { get; set; }
    internal bool IsChecked { get; set; }
    internal int Level { get; set; }
    public bool Disabled { get; set; }
    public TreeItem<TValue> Parent { get; set; }
    public List<TreeItem<TValue>> Children { get; set; } = new();

    public void AddChild(TreeItem<TValue> item)
    {
        item.Parent = this;
        Children.Add(item);
    }

    public void AddChild(TValue value, string text)
    {
        var item = new TreeItem<TValue>() { Value = value, Text = text };
        AddChild(item);
    }

    public void SetParent()
    {
        foreach (var sub in Children)
        {
            sub.Parent = this;
            sub.SetParent();
        }
    }

    internal bool? GetCheckedStatus()
    {
        if (Children.Count == 0)
            return IsChecked;
        if (Children.All(x => x.Disabled || x.GetCheckedStatus() == true))
            return true;
        if (Children.All(x => x.Disabled || x.GetCheckedStatus() == false))
            return false;
        return null;
    }

    internal bool GetHasChildrenChecked() => IsChecked || Children.Any(x => x.GetHasChildrenChecked());
}