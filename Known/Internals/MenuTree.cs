using AntDesign;

namespace Known.Internals;

class MenuTree : BaseComponent
{
    private TreeModel tree;
    private MenuInfo root;
    private MenuInfo current = new();

    [Parameter] public List<MenuInfo> Menus { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        root = Config.App.GetRootMenu();
        root.Children.AddRange(Menus);
        tree = new TreeModel
        {
            ExpandRoot = true,
            Data = [root],
            OnNodeClick = OnTreeClickAsync,
            OnModelChanged = OnTreeChangedAsync
        };
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-edit-menu", () =>
        {
            builder.Div(() => BuildTree(builder));
            builder.Div(() => BuildForm(builder));
        });
    }

    internal static void AddMenuRow(FormModel<MenuInfo> model)
    {
        model.AddRow().AddColumn(c => c.Name, c =>
        {
            c.Name = "名称";
            c.Required = true;
        });
        model.AddRow().AddColumn(c => c.Icon, c =>
        {
            c.Name = "图标";
            c.Required = true;
            c.CustomField = nameof(IconPicker);
        });
        if (model.Data?.Type == nameof(MenuType.Link))
        {
            model.AddRow().AddColumn(c => c.Url, c => c.Required = true);
            model.AddRow().AddColumn(c => c.Target, c =>
            {
                c.Name = "目标";
                c.Type = FieldType.RadioList;
                c.Category = nameof(LinkTarget);
            });
        }
    }

    private void BuildTree(RenderTreeBuilder builder)
    {
        builder.Component<AntTree>()
               .Set(c => c.Model, tree)
               //.Set(c => c.Draggable, true)
               //.Set(c => c.OnDrop, this.Callback<TreeEventArgs<MenuInfo>>(OnTreeDrop))
               //.Set(c => c.OnDragEnd, this.Callback<TreeEventArgs<MenuInfo>>(OnTreeDropEnd))
               //.Set(c => c.TitleTemplate, this.BuildTree<TreeNode<MenuInfo>>(BuildTreeItem))
               .Build();
    }

    private void BuildTreeItem(RenderTreeBuilder builder, TreeNode<MenuInfo> node)
    {
        var model = new DropdownModel
        {
            TriggerType = nameof(Trigger.ContextMenu),
            OnItemClick = info => OnItemClick(info, node.DataItem),
            Items = [
                new ActionInfo { Id = "Delete", Icon = "close", Name = "删除" }
            ]
        };
        builder.Component<AntDropdown>()
               .Set(c => c.Model, model)
               .Set(c => c.ChildContent, b => b.Span(node.DataItem.Name))
               .Build();
    }

    private void BuildForm(RenderTreeBuilder builder)
    {
        if (current?.Id == root.Id)
            return;

        var model = new FormModel<MenuInfo>(this)
        {
            SmallLabel = true,
            Data = current,
            OnFieldChanged = async f => await SaveMenuAsync(current)
        };
        model.AddRow().AddColumn("类型", b => BuildMenuType(b, current));
        AddMenuRow(model);
        model.AddRow().AddColumn(c => c.Sort, c =>
        {
            c.Name = "排序";
            c.Required = true;
        });
        builder.Form(model);
    }

    private void BuildMenuType(RenderTreeBuilder builder, MenuInfo item)
    {
        builder.Div("type", () =>
        {
            builder.Tag(item.Type);
            builder.Div(() =>
            {
                builder.Span().Class("kui-danger")
                       .Child(() => builder.Icon("delete", "删除", this.Callback<MouseEventArgs>(e => DeleteMenu(item))));
            });
        });
    }

    private Task OnTreeClickAsync(MenuInfo item)
    {
        current = item;
        return StateChangedAsync();
    }

    private Task<TreeModel> OnTreeChangedAsync()
    {
        if (current != null && current.Parent != null)
            current.Parent.Children = [.. current.Parent.Children.OrderBy(m => m.Sort)];
        tree.Data = tree.Data;
        return Task.FromResult(tree);
    }

    private Task OnTreeDrop(TreeEventArgs<MenuInfo> args)
    {
        var node = args.Node.DataItem;
        return Task.CompletedTask;
    }

    private Task OnTreeDropEnd(TreeEventArgs<MenuInfo> args)
    {
        var node = args.Node.DataItem;
        return Task.CompletedTask;
    }

    private Task OnItemClick(ActionInfo info, MenuInfo item)
    {
        UI.Alert(item.Name);
        return Task.CompletedTask;
    }

    private async Task SaveMenuAsync(MenuInfo item)
    {
        var result = await Platform.SaveMenuAsync(item);
        UI.Result(result, tree.RefreshAsync);
    }

    private void DeleteMenu(MenuInfo item)
    {
        if (item.Children != null && item.Children.Count > 0)
        {
            UI.Error("存在子菜单，不能删除！");
            return;
        }

        UI.Confirm($"确定要删除菜单[{item.Name}]？", async () =>
        {
            var result = await Platform.DeleteMenuAsync(item);
            UI.Result(result, () =>
            {
                item.Parent.Children.Remove(item);
                item.Parent.Children.Resort();
                return tree.RefreshAsync();
            });
        });
    }
}